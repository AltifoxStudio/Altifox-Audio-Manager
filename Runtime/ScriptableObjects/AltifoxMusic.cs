using UnityEngine;
using System;
using UnityEngine.Audio;

namespace AltifoxStudio.AltifoxAudioManager
{
    [CreateAssetMenu(fileName = "AltifoxMusic", menuName = "AltifoxMusic", order = 0)]
    public class AltifoxMusic : ScriptableObject
    {
        [Header("Music Layers")]
        [Tooltip("The different audio tracks that compose the music.")]
        public MusicLayer[] musicLayers;

        [Header("Timing")]
        [Tooltip("Beats Per Minute (BPM) of the track. This is crucial for all timing calculations.")]
        [Min(1f)]
        public float trackBeatPerMinute = 120f;

        [Tooltip("Number of beats in a musical measure (e.g., 4 for a 4/4 time signature).")]
        [Min(1)]
        public int beatsPerMeasure = 4;

        [Header("Looping")]
        [Tooltip("If enabled, the music will loop between the specified start and end points.")]
        public bool loop;

        // These fields are only relevant if 'loop' is true. A custom editor can hide them conditionally.
        [Tooltip("The measure where the loop begins (1-indexed).")]
        [Min(1)]
        public int measureAtLoopStart = 1;

        [Tooltip("The beat within the start measure to begin the loop (1-indexed). Set to 0 to start at the beginning of the measure.")]
        [Min(0)]
        public int beatAtLoopStart = 0;

        [Space(5)] // Adds a little vertical space

        [Tooltip("The measure where the loop ends (inclusive).")]
        [Min(1)]
        public int measureAtLoopEnd = 8;

        [Tooltip("The beat within the end measure to end the loop (1-indexed). Set to 0 to end at the conclusion of the measure.")]
        [Min(0)]
        public int beatAtLoopEnd = 0;


        [Header("Transitions & Switching")]
        [Tooltip("The curve shape for fades and transitions between layers.")]
        public InterpolationType transitions;

        [Tooltip("Default duration for transitions in seconds.")]
        [Min(0f)]
        public float transitionTime = 1f;

        [Tooltip("Defines the musical moment when layer changes will occur: instantly, on the next beat, or at the start of the next measure.")]
        public MusicDivision switchOn;

        // --- METHODS (unchanged) ---
        public float GetLoopStartTime()
        {
            if (trackBeatPerMinute <= 0) return 0f;
            float secondsPerBeat = 60.0f / trackBeatPerMinute;
            float totalBeats = (measureAtLoopStart) * beatsPerMeasure + (beatAtLoopStart > 0 ? beatAtLoopStart - 1 : 0);
            return totalBeats * secondsPerBeat;
        }

        public float GetLoopEndTime()
        {
            if (trackBeatPerMinute <= 0) return 0f;
            float secondsPerBeat = 60.0f / trackBeatPerMinute;
            float totalBeats;
            if (beatAtLoopEnd > 0)
            {
                totalBeats = (measureAtLoopEnd - 1) * beatsPerMeasure + beatAtLoopEnd;
            }
            else
            {
                totalBeats = measureAtLoopEnd * beatsPerMeasure;
            }
            return totalBeats * secondsPerBeat;
        }
    }
}