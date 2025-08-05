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
    public class MusicLoop
    {
        // Determines how the loop points are calculated.
        public enum DefinitionType { Time, MeasuresAndBeats }

        [Tooltip("Defines whether to use direct time values or musical notation.")]
        public DefinitionType LoopDefinitionType = DefinitionType.MeasuresAndBeats;

        // --- State Properties ---
        public bool IsOnFirstPlaythrough { get; private set; } = true;
        public void Reset() => IsOnFirstPlaythrough = true;
        public void AdvanceToLoop() => IsOnFirstPlaythrough = false;

        // --- Time-Based Definition ---
        [Header("Time Definition (in seconds)")]
        [Tooltip("The playback start time for the intro.")]
        public float IntroStartTimeSeconds;
        [Tooltip("The playback start time for the loop.")]
        public float LoopStartTimeSeconds;
        [Tooltip("The playback end time for the loop.")]
        public float LoopEndTimeSeconds;

        // --- Musical-Based Definition ---
        [Header("Musical Definition")]
        public int BeatsPerMeasure = 4;
        public float BeatsPerMinute = 120;

        [Tooltip("The intro starts at this point.")]
        public MusicalTimepoint IntroStartPoint = new MusicalTimepoint(1, 1);
        [Tooltip("The main loop starts at this point.")]
        public MusicalTimepoint LoopStartPoint = new MusicalTimepoint(2, 1);
        [Tooltip("The loop ends at this point.")]
        public MusicalTimepoint LoopEndPoint = new MusicalTimepoint(3, 1);

        // --- Public Accessors ---

        /// <summary>
        /// Gets the correct start time, accounting for the intro on the first playthrough.
        /// </summary>
        public float StartTime => IsOnFirstPlaythrough ? GetIntroStartTime() : GetLoopStartTime();

        /// <summary>
        /// Gets the loop's end time.
        /// </summary>
        public float EndTime => GetLoopEndTime();

        /// <summary>
        /// Gets the duration of the current segment (intro or loop).
        /// </summary>
        public float Duration => EndTime - StartTime;

        // --- Private Calculation Methods ---

        private float GetIntroStartTime()
        {
            return LoopDefinitionType == DefinitionType.Time
                ? IntroStartTimeSeconds
                : ConvertMusicalTimepointToSeconds(IntroStartPoint);
        }

        private float GetLoopStartTime()
        {
            return LoopDefinitionType == DefinitionType.Time
                ? LoopStartTimeSeconds
                : ConvertMusicalTimepointToSeconds(LoopStartPoint);
        }

        private float GetLoopEndTime()
        {
            return LoopDefinitionType == DefinitionType.Time
                ? LoopEndTimeSeconds
                : ConvertMusicalTimepointToSeconds(LoopEndPoint);
        }

        /// <summary>
        /// Converts a measure and beat into a total time in seconds.
        /// Assumes measures and beats are 1-based.
        /// </summary>
        private float ConvertMusicalTimepointToSeconds(MusicalTimepoint point)
        {
            if (BeatsPerMinute <= 0) return 0f;
            
            float secondsPerBeat = 60.0f / BeatsPerMinute;
            // (Measure - 1) and (Beat - 1) convert from 1-based musical notation to 0-based index.
            float totalBeats = (point.Measure - 1) * BeatsPerMeasure + (point.Beat - 1);
            return totalBeats * secondsPerBeat;
        }
    }

    /// <summary>
    /// A helper struct to hold a musical timepoint. Cleaner than separate int fields.
    /// </summary>
    [Serializable]
    public struct MusicalTimepoint
    {
        public int Measure;
        public int Beat;

        public MusicalTimepoint(int measure, int beat)
        {
            Measure = measure;
            Beat = beat;
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