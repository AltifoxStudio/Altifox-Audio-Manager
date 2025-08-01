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

        // [Header("Looping")]
        // [Tooltip("If enabled, the music will loop between the specified start and end points.")]
        // public bool loop;

        public LoopRegion[] loopRegions;

        [Header("Transitions & Switching")]
        [Tooltip("The curve shape for fades and transitions between layers.")]
        public InterpolationType transitions;

        [Tooltip("Default duration for transitions in seconds.")]
        [Min(0f)]
        public float transitionTime = 1f;

        [Tooltip("Defines the musical moment when layer changes will occur: instantly, on the next beat, or at the start of the next measure.")]
        public MusicDivision switchOn;

        // --- METHODS (unchanged) ---
        public float GetLoopStartTime(int loopRegionIndex)
        {
            LoopRegion loopRegion = loopRegions[loopRegionIndex];
            if (loopRegion.sectionBeatsPerMinute <= 0) return 0f;
            float secondsPerBeat = 60.0f / loopRegion.sectionBeatsPerMinute;
            float totalBeats = (loopRegion.measureAtLoopStart) * loopRegion.sectionBeatsPerMeasure+ (loopRegion.beatAtLoopStart > 0 ? loopRegion.beatAtLoopStart - 1 : 0);
            return totalBeats * secondsPerBeat;
        }

        public float GetLoopEndTime(int loopRegionIndex)
        {
            LoopRegion loopRegion = loopRegions[loopRegionIndex];
            if (loopRegion.sectionBeatsPerMinute <= 0) return 0f;
            float secondsPerBeat = 60.0f /  loopRegion.sectionBeatsPerMinute;
            float totalBeats;
            if (loopRegion.beatAtLoopEnd > 0)
            {
                totalBeats = (loopRegion.measureAtLoopEnd - 1) * loopRegion.sectionBeatsPerMeasure + loopRegion.beatAtLoopEnd;
            }
            else
            {
                totalBeats = loopRegion.measureAtLoopEnd * loopRegion.sectionBeatsPerMeasure;
            }
            return totalBeats * secondsPerBeat;
        }
    }
}