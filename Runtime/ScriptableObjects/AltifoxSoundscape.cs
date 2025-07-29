using UnityEngine;
using System;

namespace AltifoxStudio.AltifoxAudioManager
{
    using UnityEngine;

    public enum SoundscapeShape
    {
        Sphere,
        Box,
        Cylinder,
    }

    public enum SFXType
    {
        RandomThreeDimensions,
        RandomTwoDimensions,
        surroundOnce,
        surroundPersistent
    }

    [Serializable]
    public struct SoundScapeComponent
    {
        public string name;
        public SFXType type;
        public AltifoxSoundBase sfx;
        public float averageWaitTime;
        public float stdDevWaitTime;
    }

    [CreateAssetMenu(fileName = "AltifoxSoundscape", menuName = "Altifox Soundscape", order = 0)]
    public class AltifoxSoundscape : ScriptableObject
    {
        public SoundscapeShape shape;
        public float minSpawnDistance;
        [Header("Used for the sphere & cylinder Radius as well as the box X axis size")]
        public float xAxisMaxDistance;
        [Header("Used for the cylinder height as well as the box Y axis size")]
        public float yAxisMaxDistance;
        [Header("Only used by the box Z axis size")]
        public float zAxisMaxDistance;

        [Header("Other Options")]
        public bool onlySpawnAboveListener = false;
        public bool soundFollowListener = false;

        public SoundScapeComponent[] soundScapeComponents;

        public Vector3 GenerateAnSFXPostion()
        {
            float distanceX = Random.Range(minSpawnDistance, xAxisMaxDistance);
            float distanceY = Random.Range(minSpawnDistance, yAxisMaxDistance);
            float distanceZ = Random.Range(minSpawnDistance, zAxisMaxDistance);

            float positiveX = (Random.value < 0.5f) ? -1.0f : 1.0f;
            float positiveY = (Random.value < 0.5f) ? -1.0f : 1.0f;
            float positiveZ = (Random.value < 0.5f) ? -1.0f : 1.0f;
            Vector3 direction = new Vector3(0,0,0);
            switch (shape)
            {
                case SoundscapeShape.Sphere:
                    direction = Random.insideUnitSphere.normalized * distanceX;
                    break;
                case SoundscapeShape.Box:
                    direction.x = distanceX * positiveX;
                    direction.y = distanceY * positiveY;
                    direction.z = distanceZ * positiveZ;
                    break;

                case SoundscapeShape.Cylinder:
                    Vector2 xzDirection = Random.insideUnitCircle.normalized; 
                    Vector2 xzPoint = xzDirection * distanceX;

                    direction.x = xzPoint.x;
                    direction.y = distanceY * positiveY;
                    direction.z = xzPoint.y;

                    break;
                default:
                    break;
            }
            if (onlySpawnAboveListener)
            {
                direction.y = Mathf.Abs(direction.y);
            }
            return direction;
        }
    }
}