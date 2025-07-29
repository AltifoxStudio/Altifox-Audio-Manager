using UnityEngine;
using UnityEditor; // Required for all editor scripting
using AltifoxTools;

// This attribute tells Unity that this script is a custom editor for the AltifoxPersistentPlayer component.
[CustomEditor(typeof(AltifoxOneShotPlayer))]
public class AltifoxOneShotPlayerEditor : Editor // Note: It inherits from 'Editor', not 'MonoBehaviour'.
{
    // This method is called by Unity to draw the inspector GUI.
    public override void OnInspectorGUI()
    {
        // 1. Draw the default inspector fields (like 'altifoxMusicSO' and 'playOnAwake').
        DrawDefaultInspector();

        // Get a reference to the script we are inspecting.
        AltifoxOneShotPlayer sfxPlayer = (AltifoxOneShotPlayer)target;

        // Add some space for better layout.
        EditorGUILayout.Space(10); 
        
        // Add a bold title for our custom controls.
        EditorGUILayout.LabelField("Live Controls", EditorStyles.boldLabel);

        // 2. Check if the game is running. These controls only work in Play Mode.
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to control layers.", MessageType.Info);
            return; // Stop drawing the rest of the GUI if not in play mode.
        }
        
        // 3. Check if the ScriptableObject with layer data is assigned.
        if (sfxPlayer.altifoxSFX == null)
        {
            EditorGUILayout.HelpBox("Assign an altifoxSFX ScriptableObject", MessageType.Warning);
            return;
        }
        if (GUILayout.Button("Play"))
        {
            sfxPlayer.Play();
        }
    }
}