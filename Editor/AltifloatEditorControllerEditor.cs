using UnityEngine;
using UnityEditor; // Required for custom editors

// This attribute tells Unity that this class is a custom editor for AltifoxFloatController
[CustomEditor(typeof(AltifloatEditorController))]
public class AltifloatEditorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields (for automations, etc.)
        base.OnInspectorGUI();

        // Get a reference to the script we're inspecting
        AltifloatEditorController controller = (AltifloatEditorController)target;

        // Ensure the altifloat field isn't empty
        if (controller.altifloat != null)
        {
            // Begin a check to see if the user changes the value
            EditorGUI.BeginChangeCheck();

            // Create a new float field in the inspector
            float newValue = EditorGUILayout.FloatField("Altifloat Value", controller.altifloat.Value);

            // If the check detects a change...
            if (EditorGUI.EndChangeCheck())
            {
                // Assign the new value through the property. This will trigger the OnValueChanged event! ✅
                controller.altifloat.Value = newValue;
                
                // Mark the ScriptableObject as "dirty" to ensure the change is saved
                EditorUtility.SetDirty(controller.altifloat);
            }
        }
    }
}