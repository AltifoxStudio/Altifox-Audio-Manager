using UnityEngine;
using UnityEditor; // We need this namespace for editor scripts

[CustomEditor(typeof(AutomationPreviewer))]
public class AutomationPreviewerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default fields (sourceType, points, etc.)
        DrawDefaultInspector();

        // Get a reference to the script we're inspecting
        AutomationPreviewer generator = (AutomationPreviewer)target;

        // Add a space for visual separation
        EditorGUILayout.Space();

        // Add a button. If it's clicked...
        if (GUILayout.Button("Generate Curve"))
        {
            // ...call the public method from our generator script.
            generator.GenerateCurve();
        }
    }
}