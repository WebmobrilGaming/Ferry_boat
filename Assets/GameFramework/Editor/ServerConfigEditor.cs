#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
namespace Netconfig
{
    [CustomEditor(typeof(ServerConfig))]
    public class ServerConfigEditor : Editor
    {
        private SerializedProperty serverEntriesProperty;
        private SerializedProperty selectedConfigIndexProperty;
        SerializedProperty apiListProperty;
        // Key strings for EditorPrefs
        private const string ConfigNameKey = "ServerConfigEditor_ConfigName";
        private const string BaseURLKey = "ServerConfigEditor_BaseURL";
        private const string SocketURLKey = "ServerConfigEditor_SocketURL";

        private void OnEnable()
        {
            serverEntriesProperty = serializedObject.FindProperty("serverEntries");
            selectedConfigIndexProperty = serializedObject.FindProperty("selectedConfigIndex");
            apiListProperty = serializedObject.FindProperty("apiList");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // Display existing configurations in a dropdown
            DrawServerConfigDropdown();

            EditorGUILayout.Space();

            // Add new configuration
            DrawAddServerEntry();

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("API List", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(apiListProperty, new GUIContent("APIs"), true);

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawServerConfigDropdown()
        {
            EditorGUILayout.LabelField("Server Configurations", EditorStyles.boldLabel);

            string[] configNames = new string[serverEntriesProperty.arraySize];
            for (int i = 0; i < serverEntriesProperty.arraySize; i++)
            {
                SerializedProperty entry = serverEntriesProperty.GetArrayElementAtIndex(i);
                SerializedProperty configName = entry.FindPropertyRelative("configName");
                configNames[i] = configName.stringValue;
            }

            // Display dropdown with current selection
            int selectedConfigIndex = EditorGUILayout.Popup("Selected Configuration", selectedConfigIndexProperty.intValue, configNames);

            // Update the selectedConfigIndex property
            selectedConfigIndexProperty.intValue = selectedConfigIndex;
        }

        private void DrawAddServerEntry()
        {
            EditorGUILayout.LabelField("Add New Configuration", EditorStyles.boldLabel);

            // Config Name text field on a new line
            string newConfigName = EditorPrefs.GetString(ConfigNameKey, "Config Name");
            EditorGUILayout.LabelField("Config Name", EditorStyles.boldLabel);
            newConfigName = EditorGUILayout.TextField(newConfigName, GUILayout.Width(200), GUILayout.Height(20));

            // Server URL text field on a new line
            string newBaseURL = EditorPrefs.GetString(BaseURLKey, "Base URL");
            EditorGUILayout.LabelField("Base URL", EditorStyles.boldLabel);
            newBaseURL = EditorGUILayout.TextField(newBaseURL, GUILayout.Width(200), GUILayout.Height(20));

            string newSocketURL = EditorPrefs.GetString(SocketURLKey, "Socket URL");
            EditorGUILayout.LabelField("Socket URL", EditorStyles.boldLabel);
            newSocketURL = EditorGUILayout.TextField(newSocketURL, GUILayout.Width(200), GUILayout.Height(20));

            EditorGUILayout.Space(); // Add some space between the text fields and the button

            EditorGUILayout.BeginHorizontal();

            // Add button with a specific width and height
            if (GUILayout.Button("Add", GUILayout.Width(150), GUILayout.Height(40)))
            {
                // Record undo for editor integration
                Undo.RecordObject(target, "Add Server Entry");

                (target as ServerConfig).AddServerEntry(newConfigName, newBaseURL, newSocketURL);

                // Mark the object dirty so Unity knows it changed
                EditorUtility.SetDirty(target);
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();

                // Clear EditorPrefs
                EditorPrefs.SetString(ConfigNameKey, "");
                EditorPrefs.SetString(BaseURLKey, "");
                EditorPrefs.SetString(SocketURLKey, "");
            }
            if (GUILayout.Button("Remove Current Config", GUILayout.Width(150), GUILayout.Height(40)))
            {
                Undo.RecordObject(target, "Remove Server Entry");

                (target as ServerConfig).RemoveServerEntry(selectedConfigIndexProperty.intValue);

                EditorUtility.SetDirty(target);
                serializedObject.Update();
                serializedObject.ApplyModifiedProperties();
            }
            EditorGUILayout.EndHorizontal();

            // Save the entered values to EditorPrefs for persistence
            EditorPrefs.SetString(ConfigNameKey, newConfigName);
            EditorPrefs.SetString(BaseURLKey, newBaseURL);
            EditorPrefs.SetString(SocketURLKey, newSocketURL);
        }
    }
}
#endif