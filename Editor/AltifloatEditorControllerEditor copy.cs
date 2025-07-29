using UnityEngine;
using UnityEditor; // Required for custom editors

// This attribute tells Unity that this class is a custom editor for AltifoxFloatController
[CustomEditor(typeof(AltiintEditorController))]
public class AltiintEditorControllerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Draw the default inspector fields (for automations, etc.)
        base.OnInspectorGUI();

        // Get a reference to the script we're inspecting
        AltiintEditorController controller = (AltiintEditorController)target;

        // Ensure the altifloat field isn't empty
        if (controller.altiint != null)
        {
            // Begin a check to see if the user changes the value
            EditorGUI.BeginChangeCheck();

            // Create a new float field in the inspector
            int newValue = EditorGUILayout.IntField("altiint Value", controller.altiint.Value);

            // If the check detects a change...
            if (EditorGUI.EndChangeCheck())
            {
                // Assign the new value through the property. This will trigger the OnValueChanged event! ✅
                controller.altiint.Value = newValue;
                
                // Mark the ScriptableObject as "dirty" to ensure the change is saved
                EditorUtility.SetDirty(controller.altiint);
            }
        }
    }
}