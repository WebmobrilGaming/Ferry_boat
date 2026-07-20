using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Attach to a trigger collider covering the water surface (e.g. a thin BoxCollider
/// matching your water plane's bounds, isTrigger = true, positioned at water level).
///
/// Any collider that enters is automatically tracked — no per-object setup required.
/// Works with or without a Rigidbody (velocity is derived from position delta, so it
/// also handles kinematic controllers, character rigs, etc).
///
/// - OnTriggerEnter: one-shot splash impulse, scaled by entry (downward) speed.
/// - OnTriggerStay:  continuous ripple/wake, scaled by planar movement speed.
/// </summary>
[RequireComponent(typeof(Collider))]
public class WaterRippleZone : MonoBehaviour
{
    [Header("Continuous Wake (while submerged/touching)")]
    public float wakeBaseStrength = 0.05f;
    public float wakeSpeedMultiplier = 0.04f;
    public float minSpeedForWake = 0.05f;

    [Header("Entry Splash")]
    public bool splashOnEnter = true;
    public float splashStrengthMultiplier = 0.08f;
    public float minEntrySpeedForSplash = 0.3f;

    [Header("Radius (in WORLD UNITS, auto-converted using RippleSimulation.worldSize)")]
    public float minRadiusWorldUnits = 1.5f;
    public float maxRadiusWorldUnits = 15f;
    public float radiusPerObjectSize = 4f;      // ripple radius = object footprint * this
    public float entrySplashRadiusBoost = 1.6f; // entry splash reads bigger than the continuous wake

    [Header("Clamping")]
    public float maxStrength = 1.5f;

    [Header("Splash Particles (optional)")]
    [Tooltip("The root GameObject of the splash prefab (particle systems live on its children). " +
             "Leave empty to auto-generate a basic splash effect at runtime.")]
    public GameObject splashPrefab;
    public float splashScalePerSpeed = 0.05f;
    public float splashMinScale = 0.5f;
    public float splashMaxScale = 4f;
    public float splashMinSpeed = 1f; // don't spawn particles for tiny taps

    GameObject _autoSplashTemplate;

    // Tracks last known world position per collider so we can derive velocity
    // without requiring a Rigidbody (handles kinematic/character-controller cases).
    readonly Dictionary<Collider, Vector3> _lastPositions = new Dictionary<Collider, Vector3>();

    void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        Vector3 pos = other.attachedRigidbody != null ? other.attachedRigidbody.position : other.transform.position;
        _lastPositions[other] = pos;

        if (!splashOnEnter || RippleSimulation.Instance == null) return;

        float entrySpeed = 0f;
        if (other.attachedRigidbody != null)
            entrySpeed = other.attachedRigidbody.linearVelocity.magnitude; // use .velocity on pre-Unity 6

        if (entrySpeed < minEntrySpeedForSplash) return;

        float radius = ComputeRadius(other) * entrySplashRadiusBoost;
        float strength = Mathf.Clamp(entrySpeed * splashStrengthMultiplier, 0f, maxStrength);
        RippleSimulation.Instance.AddDisturbance(pos, strength, radius);

        if (entrySpeed >= splashMinSpeed)
            SpawnSplashVFX(pos, entrySpeed);
    }

    void OnTriggerStay(Collider other)
    {
        if (RippleSimulation.Instance == null) return;

        Vector3 pos = other.attachedRigidbody != null ? other.attachedRigidbody.position : other.transform.position;

        if (!_lastPositions.TryGetValue(other, out Vector3 lastPos))
        {
            _lastPositions[other] = pos;
            return;
        }

        // Planar speed (XZ) reads more naturally for wake than full 3D speed.
        Vector3 delta = pos - lastPos;
        float speed = new Vector2(delta.x, delta.z).magnitude / Mathf.Max(Time.fixedDeltaTime, 0.0001f);
        _lastPositions[other] = pos;

        if (speed < minSpeedForWake) return;

        float radius = ComputeRadius(other);
        float strength = Mathf.Clamp(wakeBaseStrength + speed * wakeSpeedMultiplier, 0f, maxStrength);
        RippleSimulation.Instance.AddDisturbance(pos, strength, radius);
    }

    void OnTriggerExit(Collider other)
    {
        _lastPositions.Remove(other);
    }

    /// Returns a UV-space radius (0..1), correctly scaled by the sim's actual world size —
    /// this is the piece that was missing before, which is why ripples looked tiny regardless
    /// of the multiplier used.
    float ComputeRadius(Collider other)
    {
        float worldSize = RippleSimulation.Instance != null ? RippleSimulation.Instance.worldSize : 100f;

        Vector3 ext = other.bounds.extents;
        float footprint = Mathf.Max(ext.x, ext.z); // horizontal size of the object, world units

        float radiusWorldUnits = Mathf.Clamp(footprint * radiusPerObjectSize, minRadiusWorldUnits, maxRadiusWorldUnits);
        return radiusWorldUnits / worldSize; // convert to UV-space fraction
    }

    // ------------------------------------------------------------------
    // Splash VFX — spawns/plays a burst of droplet particles on impact.
    // Assign `splashPrefab` for a fully authored effect; otherwise a
    // basic particle system is generated automatically at runtime.
    // ------------------------------------------------------------------
    void SpawnSplashVFX(Vector3 pos, float entrySpeed)
    {
        GameObject template = splashPrefab != null ? splashPrefab : GetOrCreateAutoSplashTemplate();
        if (template == null) return;

        float scale = Mathf.Clamp(entrySpeed * splashScalePerSpeed, splashMinScale, splashMaxScale);

        GameObject fx = Instantiate(template, pos, Quaternion.identity);
        fx.transform.localScale = Vector3.one * scale;
        fx.SetActive(true);

        ParticleSystem[] systems = fx.GetComponentsInChildren<ParticleSystem>();
        float maxLife = 0.5f;
        foreach (var ps in systems)
        {
            ps.Play();
            var main = ps.main;
            maxLife = Mathf.Max(maxLife, main.duration + main.startLifetime.constantMax);
        }

        Destroy(fx, maxLife);
    }

    GameObject GetOrCreateAutoSplashTemplate()
    {
        if (_autoSplashTemplate != null) return _autoSplashTemplate;

        var root = new GameObject("AutoSplashTemplate");
        root.SetActive(false);
        root.transform.SetParent(transform, false);

        var child = new GameObject("Droplets");
        child.transform.SetParent(root.transform, false);

        var ps = child.AddComponent<ParticleSystem>();
        var main = ps.main;
        main.duration = 0.6f;
        main.loop = false;
        main.startLifetime = 0.5f;
        main.startSpeed = 4f;
        main.startSize = 0.15f;
        main.startColor = new Color(0.85f, 0.95f, 1f, 0.9f);
        main.gravityModifier = 1f;
        main.simulationSpace = ParticleSystemSimulationSpace.World;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 20, 30, 1, 0.01f) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 25f;
        shape.radius = 0.1f;
        shape.rotation = new Vector3(-90f, 0f, 0f); // cone pointing up

        var renderer = ps.GetComponent<ParticleSystemRenderer>();
        Shader s = Shader.Find("Universal Render Pipeline/Particles/Unlit") ?? Shader.Find("Particles/Standard Unlit");
        renderer.material = new Material(s);

        _autoSplashTemplate = root;
        return root;
    }
}