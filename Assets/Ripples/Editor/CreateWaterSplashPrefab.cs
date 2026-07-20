using UnityEngine;
using UnityEditor;

public class CreateBoatSplashPrefab
{
    [MenuItem("Tools/Create Boat Splash Prefab")]
    static void CreatePrefab()
    {
        GameObject root = new GameObject("BoatSplash");

        CreateSplash(root.transform, "BowSplash_Left",
            new Vector3(-0.7f, 0f, 2.2f),
            new Vector3(0, -35, 0));

        CreateSplash(root.transform, "BowSplash_Right",
            new Vector3(0.7f, 0f, 2.2f),
            new Vector3(0, 35, 0));

        CreateMist(root.transform);

        if (!AssetDatabase.IsValidFolder("Assets/Prefabs"))
            AssetDatabase.CreateFolder("Assets", "Prefabs");

        PrefabUtility.SaveAsPrefabAsset(root, "Assets/Prefabs/BoatSplash.prefab");

        UnityEngine.Object.DestroyImmediate(root); 

        AssetDatabase.Refresh();
    }

    static void CreateSplash(Transform parent, string name, Vector3 pos, Vector3 rot)
    {
        GameObject go = new GameObject(name);
        go.transform.SetParent(parent);
        go.transform.localPosition = pos;
        go.transform.localEulerAngles = rot;

        ParticleSystem ps = go.AddComponent<ParticleSystem>();

        //---------------------------------------
        // Main
        //---------------------------------------

        var main = ps.main;
        main.loop = true;
        main.duration = 1f;

        main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 0.9f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(10f, 14f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.25f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0, Mathf.PI * 2);

        main.gravityModifier = 0.15f;

        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.maxParticles = 2000;

        //---------------------------------------
        // Emission
        //---------------------------------------

        var emission = ps.emission;
        emission.rateOverTime = 450;

        //---------------------------------------
        // Shape
        //---------------------------------------

        var shape = ps.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 22;
        shape.radius = 0.1f;

        //---------------------------------------
        // Velocity Over Lifetime
        //---------------------------------------

        var velocity = ps.velocityOverLifetime;
        velocity.enabled = true;
        velocity.space = ParticleSystemSimulationSpace.Local;

        velocity.x = new ParticleSystem.MinMaxCurve(
            name.Contains("Left") ? -3.5f : 3.5f);

        velocity.y = new ParticleSystem.MinMaxCurve(2.5f);
        velocity.z = new ParticleSystem.MinMaxCurve(-7f);

        //---------------------------------------
        // Limit Velocity
        //---------------------------------------

        var limit = ps.limitVelocityOverLifetime;
        limit.enabled = true;
        limit.limit = 6;
        limit.dampen = .35f;

        //---------------------------------------
        // Size Over Lifetime
        //---------------------------------------

        var size = ps.sizeOverLifetime;
        size.enabled = true;

        AnimationCurve sizeCurve = new AnimationCurve();

        sizeCurve.AddKey(0f, .25f);
        sizeCurve.AddKey(.15f, 1f);
        sizeCurve.AddKey(.65f, .6f);
        sizeCurve.AddKey(1f, 0);

        size.size = new ParticleSystem.MinMaxCurve(1, sizeCurve);

        //---------------------------------------
        // Color
        //---------------------------------------

        var color = ps.colorOverLifetime;
        color.enabled = true;

        Gradient g = new Gradient();

        g.SetKeys(
            new GradientColorKey[]
            {
            new GradientColorKey(Color.white,0),
            new GradientColorKey(new Color(.9f,.95f,1f),.5f),
            new GradientColorKey(Color.white,1)
            },

            new GradientAlphaKey[]
            {
            new GradientAlphaKey(1,0),
            new GradientAlphaKey(.75f,.35f),
            new GradientAlphaKey(.4f,.7f),
            new GradientAlphaKey(0,1)
            });

        color.color = new ParticleSystem.MinMaxGradient(g);

        //---------------------------------------
        // Noise
        //---------------------------------------

        var noise = ps.noise;
        noise.enabled = true;
        noise.strength = .6f;
        noise.frequency = 1.2f;
        noise.scrollSpeed = .4f;
        noise.damping = true;

        //---------------------------------------
        // Trails
        //---------------------------------------

        var trails = ps.trails;
        trails.enabled = true;
        trails.mode = ParticleSystemTrailMode.PerParticle;
        trails.lifetime = .15f;
        trails.ribbonCount = 1;
        trails.widthOverTrail = new ParticleSystem.MinMaxCurve(.05f);

        //---------------------------------------
        // Renderer
        //---------------------------------------

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;

        Shader shader =
            Shader.Find("Universal Render Pipeline/Particles/Unlit") ??
            Shader.Find("Particles/Standard Unlit");

        Material mat = new Material(shader);
        renderer.sharedMaterial = mat;
    }

    static void CreateMist(Transform parent)
    {
        GameObject go = new GameObject("Mist");

        go.transform.SetParent(parent);
        go.transform.localPosition = new Vector3(0, .2f, 1.8f);

        var ps = go.AddComponent<ParticleSystem>();

        var main = ps.main;
        main.loop = true;
        main.startLifetime = 1.5f;
        main.startSpeed = 2;
        main.startSize = 0.8f;
        main.maxParticles = 500;
        main.simulationSpace = ParticleSystemSimulationSpace.Local;

        var emission = ps.emission;
        emission.rateOverTime = 80;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Cone;
        shape.angle = 35;
        shape.radius = .2f;

        var color = ps.colorOverLifetime;
        color.enabled = true;

        Gradient g = new Gradient();

        g.SetKeys(
            new GradientColorKey[]
            {
            new GradientColorKey(Color.white,0),
            new GradientColorKey(Color.white,1)
            },
            new GradientAlphaKey[]
            {
            new GradientAlphaKey(.25f,0),
            new GradientAlphaKey(0,1)
            });

        color.color = g;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
    }
}