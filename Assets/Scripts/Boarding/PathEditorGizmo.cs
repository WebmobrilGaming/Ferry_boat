#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// Attach this to any GameObject alongside PathFollower
// Lets you drag waypoints as handles directly in the Scene view
[RequireComponent(typeof(PathFollower))]
public class PathEditorGizmo : MonoBehaviour
{
    [SerializeField] public PathData Path;
    [SerializeField] public Color WaypointColor = Color.cyan;
    [SerializeField] public Color PathColor = new Color(0.3f, 1f, 0.6f, 0.9f);
    [SerializeField] public float HandleSize = 0.25f;
    [SerializeField] public bool ShowIndices = true;
}

[CustomEditor(typeof(PathEditorGizmo))]
public class PathEditorGizmoEditor : Editor
{
    private PathEditorGizmo _gizmo;
    private PathData _path;
    private const int DrawSamples = 60;

    private void OnEnable()
    {
        _gizmo = (PathEditorGizmo)target;
        _path = _gizmo.Path;
    }

    private void OnSceneGUI()
    {
        if (_path == null || _path.Count == 0) return;

        // Draw smooth path
        Handles.color = _gizmo.PathColor;
        Vector3 prev = _path.Evaluate(0f);
        for (int i = 1; i <= DrawSamples; i++)
        {
            Vector3 next = _path.Evaluate((float)i / DrawSamples);
            Handles.DrawLine(prev, next, 2f);
            prev = next;
        }

        // Draw + drag waypoint handles
        Handles.color = _gizmo.WaypointColor;
        for (int i = 0; i < _path.Count; i++)
        {
            Vector3 wp = _path.Waypoints[i];

            float size = HandleUtility.GetHandleSize(wp) * _gizmo.HandleSize;
            Handles.SphereHandleCap(0, wp, Quaternion.identity, size, EventType.Repaint);

            EditorGUI.BeginChangeCheck();
            Vector3 newPos = Handles.PositionHandle(wp, Quaternion.identity);
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_path, "Move Waypoint");
                _path.SetWaypoint(i, newPos);
                EditorUtility.SetDirty(_path);
            }

            if (_gizmo.ShowIndices)
                Handles.Label(wp + Vector3.up * (size * 1.5f), $"[{i}]",
                    new GUIStyle(GUI.skin.label) { normal = { textColor = _gizmo.WaypointColor } });
        }
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        EditorGUILayout.Space(8);

        if (_path == null) return;

        if (GUILayout.Button("Add Waypoint at Scene Center"))
        {
            Undo.RecordObject(_path, "Add Waypoint");
            Vector3 center = SceneView.lastActiveSceneView?.camera.transform.position ?? Vector3.zero;
            _path.AddWaypoint(center);
        }

        if (_path.Count > 2 && GUILayout.Button("Remove Last Waypoint"))
        {
            Undo.RecordObject(_path, "Remove Waypoint");
            _path.RemoveWaypoint(_path.Count - 1);
        }
    }
}
#endif