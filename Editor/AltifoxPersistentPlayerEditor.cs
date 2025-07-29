using UnityEngine;
using UnityEditor; // Required for all editor scripting
using AltifoxTools;

// This attribute tells Unity that this script is a custom editor for the AltifoxPersistentPlayer component.
[CustomEditor(typeof(AltifoxPersistentPlayer))]
public class AltifoxPersistentPlayerEditor : Editor // Note: It inherits from 'Editor', not 'MonoBehaviour'.
{
    // This method is called by Unity to draw the inspector GUI.
    public override void OnInspectorGUI()
    {
        // 1. Draw the default inspector fields (like 'altifoxMusicSO' and 'playOnAwake').
        DrawDefaultInspector();

        // Get a reference to the script we are inspecting.
        AltifoxPersistentPlayer musicPlayer = (AltifoxPersistentPlayer)target;

        // Add some space for better layout.
        EditorGUILayout.Space(10); 
        
        // Add a bold title for our custom controls.
        EditorGUILayout.LabelField("Live Layer Controls", EditorStyles.boldLabel);

        // 2. Check if the game is running. These controls only work in Play Mode.
        if (!Application.isPlaying)
        {
            EditorGUILayout.HelpBox("Enter Play Mode to control layers.", MessageType.Info);
            return; // Stop drawing the rest of the GUI if not in play mode.
        }
        
        // 3. Check if the ScriptableObject with layer data is assigned.
        if (musicPlayer.altifoxMusicSO == null || musicPlayer.altifoxMusicSO.musicLayers.Length == 0)
        {
            EditorGUILayout.HelpBox("Assign an AltifoxMusic ScriptableObject with defined layers.", MessageType.Warning);
            return;
        }

        string[] layersToFade = { "All" };
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Fade Out All Layers"))
        {
            musicPlayer.FadeOutLayers(layersToFade, 1f, InterpolationType.Smooth);
        }
        if (GUILayout.Button("Play"))
        {
            musicPlayer.Play();
        }
        EditorGUILayout.EndHorizontal();

        // Create an "Pause" button.
        if (GUILayout.Button("Pause"))
        {
            // When clicked, call the public method on the component.
            musicPlayer.Pause();
        }

        // Create an "Pause" button.
        if (GUILayout.Button("UnPause"))
        {
            // When clicked, call the public method on the component.
            musicPlayer.UnPause();
        }


        // 4. Loop through each music layer and create buttons for it.
        foreach (var layer in musicPlayer.altifoxMusicSO.musicLayers)
        {
            // Begin a horizontal group to place the label and buttons on the same line.
            EditorGUILayout.BeginHorizontal();

            // Display the layer name.
            EditorGUILayout.LabelField(layer.name);

            // Create an "Activate" button.
            if (GUILayout.Button("Activate"))
            {
                // When clicked, call the public method on the component.
                musicPlayer.SetLayerActive(layer.name, true);
            }

            // Create a "Deactivate" button.
            if (GUILayout.Button("Deactivate"))
            {
                // When clicked, call the public method on the component.
                musicPlayer.SetLayerActive(layer.name, false);
            }

            // End the horizontal group.
            EditorGUILayout.EndHorizontal();
        }
    }
}