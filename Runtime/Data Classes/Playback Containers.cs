using UnityEngine;
using System;

namespace AltifoxStudio.AltifoxAudioManager
{
    public enum PlaybackParams
    {
        volume,
        pitch,
        transposition
    }

    public struct MusicFlag
    {
        public string name;
        public int measure;
        public int beat;
    }

public class PlaybackTools
    {
        public enum PlaybackType
        {
            Sequential,
            Random,
            Shuffle,
        }

        [Serializable]
        public struct RandomnessSelector
        {
            [Range(0f, 1f)]
            public float min;
            [Range(0f, 1f)]
            public float max;
        }

    }

    [Serializable]
    public class RandomFloat
    {
        public float defaultValue;
        public float minDeviation;
        public float maxDeviation;
        public bool enableRandomness;

        public float SampleValue()
        {
            if (enableRandomness)
            {
                return UnityEngine.Random.Range(defaultValue - minDeviation, defaultValue + maxDeviation);
            }
            return defaultValue;
        }
    }

}