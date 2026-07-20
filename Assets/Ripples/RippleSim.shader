Shader "Hidden/RippleSim"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "black" {}
    }
    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            Name "Propagate"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_TexelSize;
            float _Damping;
            float _Spread;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 frag (v2f i) : SV_Target
            {
                float2 texel = _MainTex_TexelSize.xy;

                float2 c  = tex2D(_MainTex, i.uv).rg;
                float hL = tex2D(_MainTex, i.uv - float2(texel.x, 0)).r;
                float hR = tex2D(_MainTex, i.uv + float2(texel.x, 0)).r;
                float hU = tex2D(_MainTex, i.uv + float2(0, texel.y)).r;
                float hD = tex2D(_MainTex, i.uv - float2(0, texel.y)).r;

                float laplacian = (hL + hR + hU + hD) - 4.0 * c.r;

                float vel = c.g + laplacian * _Spread;
                vel *= _Damping;
                float height = c.r + vel;

                return float2(height, vel);
            }
            ENDHLSL
        }

        Pass
        {
            Name "Stamp"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float2 _StampUV;
            float _StampRadius;
            float _StampStrength;

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f { float2 uv : TEXCOORD0; float4 vertex : SV_POSITION; };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 frag (v2f i) : SV_Target
            {
                float2 c = tex2D(_MainTex, i.uv).rg;
                float d = distance(i.uv, _StampUV);
                float influence = smoothstep(_StampRadius, 0.0, d) * _StampStrength;
                c.r += influence;
                return c;
            }
            ENDHLSL
        }
    }
}