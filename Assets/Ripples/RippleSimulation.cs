using UnityEngine;

/// <summary>
/// Owns a ping-ponged RenderTexture pair that simulates a 2D wave equation
/// (R = height, G = velocity) over a fixed world-space square area.
/// Any source (boats, splashes, rain) can inject disturbances via AddDisturbance.
/// The resulting height texture + world-mapping params are pushed as globals
/// so any water shader in the scene can sample it without a manual reference.
/// </summary>
[ExecuteAlways]
public class RippleSimulation : MonoBehaviour
{
    [Header("Simulation Area (world space, XZ plane)")]
    public Vector3 center = Vector3.zero;
    public float worldSize = 100f;   // sim covers a worldSize x worldSize square centered on `center`
    public int resolution = 512;     // RT resolution; higher = crisper ripples, more cost

    [Header("Simulation Params")]
    [Range(0.9f, 1f)] public float damping = 0.995f;
    [Range(0.1f, 1f)] public float spread = 0.5f;
    [Range(1, 4)] public int stepsPerFrame = 1;

    [Header("Refs")]
    public Shader rippleShader;

    RenderTexture _rtA, _rtB;
    Material _mat;

    static readonly int StampUVID = Shader.PropertyToID("_StampUV");
    static readonly int StampRadiusID = Shader.PropertyToID("_StampRadius");
    static readonly int StampStrengthID = Shader.PropertyToID("_StampStrength");

    public static RippleSimulation Instance { get; private set; }

    void OnEnable()
    {
        Instance = this;
        if (rippleShader == null) rippleShader = Shader.Find("Hidden/RippleSim");
        _mat = new Material(rippleShader);
        AllocateRTs();
    }

    void OnDisable()
    {
        ReleaseRTs();
        if (Instance == this) Instance = null;
    }

    void AllocateRTs()
    {
        ReleaseRTs();

        var desc = new RenderTextureDescriptor(resolution, resolution, RenderTextureFormat.RGFloat, 0)
        {
            sRGB = false
        };

        _rtA = new RenderTexture(desc) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        _rtB = new RenderTexture(desc) { wrapMode = TextureWrapMode.Clamp, filterMode = FilterMode.Bilinear };
        _rtA.Create();
        _rtB.Create();

        RenderTexture.active = _rtA; GL.Clear(true, true, Color.clear);
        RenderTexture.active = _rtB; GL.Clear(true, true, Color.clear);
        RenderTexture.active = null;

        PushGlobals();
    }

    void ReleaseRTs()
    {
        if (_rtA != null) { _rtA.Release(); _rtA = null; }
        if (_rtB != null) { _rtB.Release(); _rtB = null; }
    }

    void PushGlobals()
    {
        Shader.SetGlobalTexture("_RippleHeightTex", _rtA);
        // xy = world center, z = world size, w = 1/resolution (texel size), used by the water shader
        Shader.SetGlobalVector("_RippleWorldParams", new Vector4(center.x, center.z, worldSize, 1f / resolution));
    }

    void Update()
    {
        if (_mat == null || _rtA == null) return;

        _mat.SetFloat("_Damping", damping);
        _mat.SetFloat("_Spread", spread);

        for (int i = 0; i < stepsPerFrame; i++)
        {
            Graphics.Blit(_rtA, _rtB, _mat, 0); // Propagate pass
            Swap();
        }

        PushGlobals();
    }

    void Swap() => (_rtA, _rtB) = (_rtB, _rtA);

    /// <summary>
    /// Injects a disturbance at a world position. Call from boats, splashes, etc.
    /// strength: height added at the center of the stamp.
    /// radius01: stamp radius in UV space (0..1 of worldSize). ~0.02-0.05 is a good range.
    /// </summary>
    public void AddDisturbance(Vector3 worldPos, float strength, float radius01)
    {
        if (_mat == null || _rtA == null) return;

        Vector2 uv = WorldToUV(worldPos);
        Debug.Log($"UV={uv}"); // TEMP — should be between 0 and 1
        if (uv.x < 0f || uv.x > 1f || uv.y < 0f || uv.y > 1f)
        {
            Debug.LogWarning("Disturbance outside sim area!"); // TEMP
            return;
        }

        _mat.SetVector(StampUVID, uv);
        _mat.SetFloat(StampRadiusID, radius01);
        _mat.SetFloat(StampStrengthID, strength);

        Graphics.Blit(_rtA, _rtB, _mat, 1); // Stamp pass
        Swap();
    }

    public Vector2 WorldToUV(Vector3 worldPos)
    {
        float u = (worldPos.x - center.x) / worldSize + 0.5f;
        float v = (worldPos.z - center.z) / worldSize + 0.5f;
        return new Vector2(u, v);
    }

    void OnValidate()
    {
        if (isActiveAndEnabled && _mat != null)
            AllocateRTs();
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.5f);
        Gizmos.DrawWireCube(center, new Vector3(worldSize, 0.1f, worldSize));
    }
}
