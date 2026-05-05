using System;
using UnityEngine;

public class PathFollower : MonoBehaviour
{
    [Header("Path")]
    [SerializeField] private PathData _path;

    [Header("Movement")]
    [SerializeField] private float _speed = 5f;
    [SerializeField] private bool _playOnAwake = true;

    [Header("Duration")]
    [SerializeField] private float _targetDuration = 5f; // editable in Inspector
    public float Duration =>_targetDuration;

    [Header("Easing (optional)")]
    [SerializeField] private AnimationCurve _easingCurve = AnimationCurve.Linear(0, 0, 1, 1);

    [Header("Orientation")]
    [SerializeField] private bool _flattenY = false;
    [SerializeField] private bool _orientToPath = true;
    [SerializeField] private float _rotationSpeed = 10f;

    public event Action<int> OnReachWaypoint;
    public event Action OnPathComplete;

    /// <summary>
    /// Get or set the target duration. Setting this does NOT apply it immediately —
    /// call ApplyTargetDuration() when ready.
    /// </summary>
    public float TargetDuration
    {
        get => _targetDuration;
        set => _targetDuration = Mathf.Max(0.001f, value); // guard against zero/negative
    }

    private float _t;
    private bool _isPlaying;
    private int _lastWaypointIndex = -1;

    private float _pathLength;
    private const int LengthSamples = 100;

#if UNITY_EDITOR
    private void Reset()
    {
        string dir = "Assets/Paths";
        if (!UnityEditor.AssetDatabase.IsValidFolder(dir))
            UnityEditor.AssetDatabase.CreateFolder("Assets", "Paths");

        string path = $"{dir}/{gameObject.name}_Path.asset";
        PathData existing = UnityEditor.AssetDatabase.LoadAssetAtPath<PathData>(path);
        if (existing != null) { _path = existing; return; }

        PathData data = ScriptableObject.CreateInstance<PathData>();
        Vector3 pos = transform.position;
        data.SetWaypoints(new Vector3[] { pos, pos + transform.forward * 3f });

        UnityEditor.AssetDatabase.CreateAsset(data, path);
        UnityEditor.AssetDatabase.SaveAssets();
        _path = data;
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"[PathFollower] Created path asset at {path}");
    }
#endif

    private void Awake()
    {
        if (_path != null) _pathLength = ComputeLength();
        if (_playOnAwake) ApplyTargetDuration(); // uses _targetDuration from Inspector
    }

    private void Update()
    {
        if (!_isPlaying || _path == null) return;

        float delta = (_pathLength > 0) ? (_speed * Time.deltaTime / _pathLength) : 0f;
        _t += delta;

        if (!_path.Loop && _t >= 1f)
        {
            _t = 1f;
            _isPlaying = false;
            transform.position = _path.Evaluate(1f);
            OrientToPath(1f);
            OnPathComplete?.Invoke();
            return;
        }

        float easedT = _easingCurve.Evaluate(_path.Loop ? Mathf.Repeat(_t, 1f) : _t);
        transform.position = _path.Evaluate(easedT);
        OrientToPath(easedT);
        CheckWaypointEvents();
    }

    public void Play() { _isPlaying = true; }
    public void Pause() { _isPlaying = false; }
    public void Stop() { _isPlaying = false; _t = 0f; }
    public void SetT(float t) { _t = Mathf.Clamp01(t); }

    // -------------------------------------------------------------------------
    // Duration API
    // -------------------------------------------------------------------------

    /// <summary>
    /// Apply the current TargetDuration from current _t to endT, then Play.
    /// Use this when you've already set TargetDuration via the property or Inspector.
    /// </summary>
    public void ApplyTargetDuration(float endT = 1f)
        => ApplyTargetDuration(_t, endT, _targetDuration);

    /// <summary>
    /// Set TargetDuration and apply it immediately from current _t to endT, then Play.
    /// </summary>
    public void ApplyTargetDuration(float endT, float durationSeconds)
        => ApplyTargetDuration(_t, endT, durationSeconds);

    /// <summary>
    /// Full control — set startT, endT, and duration explicitly, then Play.
    /// </summary>
    public void ApplyTargetDuration(float startT, float endT, float durationSeconds)
    {
        if (durationSeconds <= 0f)
        {
            Debug.LogWarning("[PathFollower] ApplyTargetDuration: durationSeconds must be > 0.");
            return;
        }

        _targetDuration = durationSeconds;         // keep property in sync
        _t = Mathf.Clamp01(startT);

        float segmentLength = ComputeSegmentLength(_t, Mathf.Clamp01(endT));
        _speed = segmentLength / _targetDuration;

        Play();
    }

    // -------------------------------------------------------------------------

    private void OrientToPath(float t)
    {
        if (!_orientToPath) return;

        float lookAheadT = t + 0.01f;
        if (!_path.Loop) lookAheadT = Mathf.Clamp(lookAheadT, 0f, 1f);

        Vector3 currentPos = _path.Evaluate(t);
        Vector3 lookAtPos = _path.Evaluate(lookAheadT);
        Vector3 direction = lookAtPos - currentPos;

        if (direction.sqrMagnitude < 0.0001f) return;
        if (_flattenY) direction.y = 0f;
        if (direction.sqrMagnitude < 0.0001f) return;

        Quaternion targetRot = Quaternion.LookRotation(direction.normalized, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, _rotationSpeed * Time.deltaTime);
    }

    private void CheckWaypointEvents()
    {
        int segCount = _path.Loop ? _path.Count : _path.Count - 1;
        int current = Mathf.FloorToInt(_t * segCount);
        if (current != _lastWaypointIndex)
        {
            _lastWaypointIndex = current;
            OnReachWaypoint?.Invoke(current);
        }
    }

    private float ComputeLength() => ComputeSegmentLength(0f, 1f);

    private float ComputeSegmentLength(float fromT, float toT)
    {
        float len = 0f;
        Vector3 prev = _path.Evaluate(fromT);
        for (int i = 1; i <= LengthSamples; i++)
        {
            float t = Mathf.Lerp(fromT, toT, (float)i / LengthSamples);
            Vector3 curr = _path.Evaluate(t);
            len += Vector3.Distance(prev, curr);
            prev = curr;
        }
        return len;
    }
}