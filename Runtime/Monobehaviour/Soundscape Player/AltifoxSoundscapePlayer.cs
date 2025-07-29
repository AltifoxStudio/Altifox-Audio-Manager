using UnityEngine;
using System.Collections.Generic; // Required for the icosphere generator

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AltifoxStudio.AltifoxAudioManager
{
    public class AltifoxSoundscapePlayer : MonoBehaviour
    {

        [Tooltip("Assign your AltifoxSoundscape ScriptableObject here.")]
        public AltifoxSoundscape soundscape;
        public bool drawEditorGizmo = true;
        
#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (drawEditorGizmo == false)
            {
                return;
            }
            if (soundscape == null)
                return;

            Handles.matrix = transform.localToWorldMatrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            Color gizmoColor = new Color(0.2f, 0.8f, 1.0f, 0.85f);
            Gizmos.color = gizmoColor;
            Handles.color = gizmoColor;

            switch (soundscape.shape)
            {
                case SoundscapeShape.Sphere:
                    DrawSphereGizmo(); // Uses the new icosphere method
                    break;
                case SoundscapeShape.Box:
                    DrawBoxGizmo();
                    break;
                case SoundscapeShape.Cylinder:
                    DrawCylinderGizmo();
                    break;
            }
        }
            
            // --- GIZMO DRAWING METHODS ---

        private void DrawSphereGizmo()
        {
            // Draw outer sphere/hemisphere
                AltifoxGizmos.AltifoxIcosphereGizmo.IcosphereGenerator.DrawWireframe(
                soundscape.xAxisMaxDistance,
                soundscape.onlySpawnAboveListener
            );

            // Draw inner sphere/hemisphere
            if (soundscape.minSpawnDistance > 0)
            {
                    AltifoxGizmos.AltifoxIcosphereGizmo.IcosphereGenerator.DrawWireframe(
                    soundscape.minSpawnDistance,
                    soundscape.onlySpawnAboveListener
                );
            }
        }

        private void DrawBoxGizmo()
        {
            if (soundscape.onlySpawnAboveListener)
            {
                var outerSize = new Vector3(soundscape.xAxisMaxDistance * 2, soundscape.yAxisMaxDistance, soundscape.zAxisMaxDistance * 2);
                var outerCenter = new Vector3(0, soundscape.yAxisMaxDistance / 2, 0);
                Gizmos.DrawWireCube(outerCenter, outerSize);

                float min = soundscape.minSpawnDistance;
                if (min > 0)
                {
                    var innerSize = new Vector3(min * 2, min, min * 2);
                    var innerCenter = new Vector3(0, min / 2, 0);
                    Gizmos.DrawWireCube(innerCenter, innerSize);
                }
            }
            else
            {
                var outerSize = new Vector3(soundscape.xAxisMaxDistance * 2, soundscape.yAxisMaxDistance * 2, soundscape.zAxisMaxDistance * 2);
                Gizmos.DrawWireCube(Vector3.zero, outerSize);
                var innerSize = Vector3.one * soundscape.minSpawnDistance * 2;
                Gizmos.DrawWireCube(Vector3.zero, innerSize);
            }
        }

        private void DrawCylinderGizmo()
        {
            if (soundscape.onlySpawnAboveListener)
            {
                AltifoxGizmos.AltifoxCylinderGizmo.DrawHalf(Vector3.zero, soundscape.xAxisMaxDistance, soundscape.yAxisMaxDistance);
                AltifoxGizmos.AltifoxCylinderGizmo.DrawHalf(Vector3.zero, soundscape.minSpawnDistance, soundscape.minSpawnDistance);
            }
            else
            {
                AltifoxGizmos.AltifoxCylinderGizmo.Draw(Vector3.zero, soundscape.xAxisMaxDistance, soundscape.yAxisMaxDistance * 2);
                AltifoxGizmos.AltifoxCylinderGizmo.Draw(Vector3.zero, soundscape.minSpawnDistance, soundscape.minSpawnDistance * 2);
            }
        }
#endif
    }
}