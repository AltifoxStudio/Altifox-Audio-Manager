using UnityEngine;
using System;
using UnityEngine.Audio;

namespace AltifoxStudio.AltifoxAudioManager
{
    [Serializable]
    public struct MusicLayer
    {
        // A header here can be repetitive in an array. It's often better to let the parent class's editor handle titles.
        public string name;
        public AudioClip audioClip;
        [Range(0f, 1f)]
        public float activeVolume;
        public bool activeByDefault;
        public bool deactivateOnLoop;
        public bool spatialize;
        public float spatialBlend;
        public AudioMixerGroup targetMixer;
    }
}