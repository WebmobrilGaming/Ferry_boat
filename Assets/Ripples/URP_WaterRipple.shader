Shader "Custom/URP_WaterRipple"
{
    Properties
    {
        _ShallowColor ("Shallow Color", Color) = (0.25, 0.55, 0.6, 0.75)
        _DeepColor ("Deep Color", Color) = (0.0, 0.12, 0.22, 0.92)
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalTiling ("Normal Tiling (xy)", Vector) = (2, 2, 0, 0)
        _NormalSpeed ("Normal Scroll Speed (uv1.xy, uv2.zw)", Vector) = (0.05, 0.03, -0.04, 0.02)

        _WaveAmplitude ("Base Wave Amplitude", Float) = 0.15
        _WaveFrequency ("Base Wave Frequency", Float) = 0.3
        _WaveSpeed ("Base Wave Speed", Float) = 1.0

        _RippleHeightScale ("Ripple Height Scale", Float) = 2.0
        _RippleNormalScale ("Ripple Normal Influence", Float) = 1.0

        _FresnelPower ("Fresnel Power", Float) = 3.0
        _Smoothness ("Smoothness", Range(0,1)) = 0.8

        _RefractionStrength ("Refraction Strength", Range(0, 0.1)) = 0.03

        _FoamColor ("Foam Color", Color) = (1, 1, 1, 1)
        _FoamThreshold ("Foam Threshold", Range(0, 1)) = 0.15
        _FoamSharpness ("Foam Sharpness", Range(0.5, 10)) = 3.0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode"="UniversalForward" }
            Cull Back
            // No hardware Blend/ZWrite override here — we manually composite the refracted
            // background in the fragment shader below, so this pass writes full opaque color.

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_fog

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareOpaqueTexture.hlsl"

            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

            // Pushed globally by RippleSimulation.cs — no manual assignment needed on the material.
            TEXTURE2D(_RippleHeightTex); SAMPLER(sampler_RippleHeightTex);
            float4 _RippleWorldParams; // xy = sim center (world XZ), z = worldSize, w = 1/resolution

            CBUFFER_START(UnityPerMaterial)
                float4 _ShallowColor;
                float4 _DeepColor;
                float4 _NormalTiling;
                float4 _NormalSpeed;
                float _WaveAmplitude;
                float _WaveFrequency;
                float _WaveSpeed;
                float _RippleHeightScale;
                float _RippleNormalScale;
                float _FresnelPower;
                float _Smoothness;
                float _RefractionStrength;
                float4 _FoamColor;
                float _FoamThreshold;
                float _FoamSharpness;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float4 tangentOS  : TANGENT;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float3 positionWS : TEXCOORD0;
                float3 normalWS   : TEXCOORD1;
                float4 tangentWS  : TEXCOORD2;
                float2 uv         : TEXCOORD3;
                float  fogCoord   : TEXCOORD4;
                float4 screenPos  : TEXCOORD5;
            };

            float SampleRippleHeight(float2 worldXZ)
            {
                float2 uv = (worldXZ - _RippleWorldParams.xy) / _RippleWorldParams.z + 0.5;
                return SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv, 0).r;
            }

            float3 SampleRippleNormal(float2 worldXZ)
            {
                float2 uv = (worldXZ - _RippleWorldParams.xy) / _RippleWorldParams.z + 0.5;
                float texel = _RippleWorldParams.w;

                float hL = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv - float2(texel, 0), 0).r;
                float hR = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv + float2(texel, 0), 0).r;
                float hD = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv - float2(0, texel), 0).r;
                float hU = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv + float2(0, texel), 0).r;

                return normalize(float3((hL - hR) * _RippleNormalScale * 8.0, 2.0, (hD - hU) * _RippleNormalScale * 8.0));
            }

            // Steepness of the ripple surface (how fast height changes across neighboring texels).
            // High steepness = wave crest / splash edge, exactly where foam naturally forms.
            float SampleRippleSteepness(float2 worldXZ)
            {
                float2 uv = (worldXZ - _RippleWorldParams.xy) / _RippleWorldParams.z + 0.5;
                float texel = _RippleWorldParams.w;

                float hC = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv, 0).r;
                float hL = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv - float2(texel, 0), 0).r;
                float hR = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv + float2(texel, 0), 0).r;
                float hD = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv - float2(0, texel), 0).r;
                float hU = SAMPLE_TEXTURE2D_LOD(_RippleHeightTex, sampler_RippleHeightTex, uv + float2(0, texel), 0).r;

                float gradMag = abs(hL - hC) + abs(hR - hC) + abs(hD - hC) + abs(hU - hC);
                return gradMag;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);

                // Base procedural swell (independent of the ripple sim — gives the water
                // some motion even with nothing touching it).
                float wave = sin(positionWS.x * _WaveFrequency + _Time.y * _WaveSpeed) *
                             cos(positionWS.z * _WaveFrequency * 0.8 + _Time.y * _WaveSpeed * 0.7);
                positionWS.y += wave * _WaveAmplitude;

                // Ripple contribution driven by RippleSimulation / WaterRippleZone.
                float rippleH = SampleRippleHeight(positionWS.xz);
                positionWS.y += rippleH * _RippleHeightScale;

                OUT.positionWS = positionWS;
                OUT.positionCS = TransformWorldToHClip(positionWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.tangentWS = float4(TransformObjectToWorldDir(IN.tangentOS.xyz), IN.tangentOS.w);
                OUT.uv = IN.uv;
                OUT.fogCoord = ComputeFogFactor(OUT.positionCS.z);
                OUT.screenPos = ComputeScreenPos(OUT.positionCS);
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                // Two scrolling normal-map layers at different speeds/tiling for surface detail.
                float2 uv1 = IN.uv * _NormalTiling.xy + _Time.y * _NormalSpeed.xy;
                float2 uv2 = IN.uv * _NormalTiling.xy * 1.7 + _Time.y * _NormalSpeed.zw;

                float3 n1 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv1));
                float3 n2 = UnpackNormal(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, uv2));
                float3 blendedNormalTS = normalize(float3(n1.xy + n2.xy, n1.z * n2.z));

                float3 bitangentWS = cross(IN.normalWS, IN.tangentWS.xyz) * IN.tangentWS.w;
                float3x3 TBN = float3x3(IN.tangentWS.xyz, bitangentWS, IN.normalWS);
                float3 normalWS = normalize(mul(blendedNormalTS, TBN));

                // Blend in the ripple-derived normal so wakes/splashes visibly perturb shading.
                float3 rippleNormalWS = SampleRippleNormal(IN.positionWS.xz);
                normalWS = normalize(normalWS + rippleNormalWS - float3(0, 1, 0));

                float3 viewDirWS = normalize(GetCameraPositionWS() - IN.positionWS);
                Light mainLight = GetMainLight();

                float fresnel = pow(1.0 - saturate(dot(normalWS, viewDirWS)), _FresnelPower);
                float4 baseColor = lerp(_DeepColor, _ShallowColor, fresnel);

                float NdotL = saturate(dot(normalWS, mainLight.direction));
                float3 diffuse = baseColor.rgb * mainLight.color * (NdotL * 0.5 + 0.5);

                float3 halfVec = normalize(mainLight.direction + viewDirWS);
                float spec = pow(saturate(dot(normalWS, halfVec)), lerp(8, 128, _Smoothness));
                float3 specular = mainLight.color * spec;

                // --- Refraction: bend the background (opaque scene behind the water) by the
                // surface normal, so ripples/wake visibly distort whatever's underneath instead
                // of just tinting a flat color. Requires "Opaque Texture" enabled on your URP Asset.
                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                float2 distortion = normalWS.xz * _RefractionStrength;
                float3 refractedColor = SampleSceneColor(screenUV + distortion);
                float3 underwaterColor = refractedColor * baseColor.rgb;

                // Blend refraction (what's beneath) with the lit surface color (reflection/specular
                // stand-in) using alpha as a depth/opacity control — shallow water shows more of
                // what's underneath, deep water shows more of the surface-lit color.
                float3 color = lerp(underwaterColor, diffuse, baseColor.a) + specular;

                // --- Foam: forms where the ripple surface is steep (splash edges, wake crests).
                float steepness = SampleRippleSteepness(IN.positionWS.xz);
                float foam = saturate((steepness - _FoamThreshold) * _FoamSharpness);
                color = lerp(color, _FoamColor.rgb, foam * _FoamColor.a);

                color = MixFog(color, IN.fogCoord);

                return half4(color, 1.0);
            }
            ENDHLSL
        }
    }

    FallBack "Universal Render Pipeline/Lit"
}