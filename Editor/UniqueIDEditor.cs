using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UniqueID))]
public class UniqueIDEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector (so you keep the rest of the fields)
        DrawDefaultInspector();

        // Cast the target to UniqueID type
        UniqueID uniqueIDScript = (UniqueID)target;

        EditorGUILayout.Space();

        // If there is no unique ID, show "Generate" button
        if (string.IsNullOrEmpty(uniqueIDScript.ID))
        {
            if (GUILayout.Button("Generate"))
            {
                uniqueIDScript.GenerateUniqueID();
            }
        }
        else
        {
            // If a unique ID exists, show "Regenerate" button
            if (GUILayout.Button("Re-generate"))
            {
                uniqueIDScript.ClearUniqueID();
                uniqueIDScript.GenerateUniqueID();
            }
        }
    }
}
