using UnityEngine;

[CreateAssetMenu(menuName = "Paths/PathData", fileName = "NewPathData")]
public class PathData : ScriptableObject
{
    [SerializeField] private Vector3[] _waypoints = new Vector3[2];
    [SerializeField] private bool _loop = false;
    [SerializeField] private bool _useCatmullRom = true;

    public Vector3[] Waypoints => _waypoints;
    public bool Loop => _loop;
    public bool UseCatmullRom => _useCatmullRom;

    public int Count => _waypoints?.Length ?? 0;

    // Evaluate a world-space position at normalized t [0..1]
    public Vector3 Evaluate(float t)
    {
        if (_waypoints == null || _waypoints.Length < 2) return Vector3.zero;

        if (_loop) t = Mathf.Repeat(t, 1f);
        else t = Mathf.Clamp01(t);

        int segCount = _loop ? _waypoints.Length : _waypoints.Length - 1;
        float scaled = t * segCount;
        int seg = Mathf.FloorToInt(scaled);
        float local = scaled - seg;

        if (!_useCatmullRom) return Vector3.Lerp(GetPoint(seg), GetPoint(seg + 1), local);

        // Catmull-Rom spline
        Vector3 p0 = GetPoint(seg - 1);
        Vector3 p1 = GetPoint(seg);
        Vector3 p2 = GetPoint(seg + 1);
        Vector3 p3 = GetPoint(seg + 2);
        return CatmullRom(p0, p1, p2, p3, local);
    }

    // Returns tangent direction at t (useful for orientation)
    public Vector3 EvaluateTangent(float t, float epsilon = 0.001f)
    {
        Vector3 a = Evaluate(t - epsilon);
        Vector3 b = Evaluate(t + epsilon);
        return (b - a).normalized;
    }

    private Vector3 GetPoint(int index)
    {
        if (_loop) return _waypoints[((index % _waypoints.Length) + _waypoints.Length) % _waypoints.Length];
        return _waypoints[Mathf.Clamp(index, 0, _waypoints.Length - 1)];
    }

    private static Vector3 CatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t, t3 = t2 * t;
        return 0.5f * (
            2f * p1 +
            (-p0 + p2) * t +
            (2f * p0 - 5f * p1 + 4f * p2 - p3) * t2 +
            (-p0 + 3f * p1 - 3f * p2 + p3) * t3
        );
    }

    public void SetWaypoints(Vector3[] waypoints)
    {
        _waypoints = waypoints;
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

    // Called from editor to add/remove/move waypoints at runtime
    public void SetWaypoint(int index, Vector3 position) => _waypoints[index] = position;

    public void AddWaypoint(Vector3 position)
    {
        System.Array.Resize(ref _waypoints, _waypoints.Length + 1);
        _waypoints[^1] = position;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

    }

    public void RemoveWaypoint(int index)
    {
        var list = new System.Collections.Generic.List<Vector3>(_waypoints);
        list.RemoveAt(index);
        _waypoints = list.ToArray();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif
    }

}