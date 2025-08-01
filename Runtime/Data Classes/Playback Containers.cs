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

    [Serializable]
    public class LoopRegion
    {
        public string name;
        public int sectionBeatsPerMeasure;
        public float sectionBeatsPerMinute;

        [Header("Loop Start")]
        public int measureAtLoopStart;
        public int beatAtLoopStart;

        [Header("Loop End")]
        public int measureAtLoopEnd;
        public int beatAtLoopEnd;

        public MusicDivision exitLoopOn;

        public float GetLoopStartTime()
        {
            if (sectionBeatsPerMinute <= 0) return 0f;
            float secondsPerBeat = 60.0f / sectionBeatsPerMinute;
            float totalBeats = measureAtLoopStart * sectionBeatsPerMeasure+ (beatAtLoopStart > 0 ? beatAtLoopStart - 1 : 0);
            return totalBeats * secondsPerBeat;
        }

        public float GetLoopEndTime()
        {
            if (sectionBeatsPerMinute <= 0) return 0f;
            float secondsPerBeat = 60.0f / sectionBeatsPerMinute;
            float totalBeats;
            if (beatAtLoopEnd > 0)
            {
                totalBeats = (measureAtLoopEnd - 1) * sectionBeatsPerMeasure + beatAtLoopEnd;
            }
            else
            {
                totalBeats = measureAtLoopEnd * sectionBeatsPerMeasure;
            }
            return totalBeats * secondsPerBeat;
        }

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