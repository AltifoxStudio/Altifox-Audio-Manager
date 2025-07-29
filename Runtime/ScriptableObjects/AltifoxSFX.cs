using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine.Audio;

namespace AltifoxStudio.AltifoxAudioManager
{
    [CreateAssetMenu(fileName = "AltifoxSFX", menuName = "AltifoxSFX", order = 0)]
    public class AltifoxSFX : AltifoxSoundBase
    {
        [Header("== Audio files ==")]
        public AudioClip[] audioClips;

        [Header("== Playback configuration ==")]
        public RandomFloat volume;
        public RandomFloat pitch;

        public AudioMixerGroup targetMixer;

        public PlaybackTools.PlaybackType playbackType;
        //public bool loop = false;
        public float cooldown = 0f;

        public bool spatialize = true;

        [Header("== Game Engine Configuration ==")]
        public int maxInstances = int.MaxValue;

        [Range(0f, 1f)]
        public float spatialBlend = 1f;

        // -----------------------------------------
        private int lastPlayedClip = -1;
        private float lastTimePlayed = 0f;
        Stack<AudioClip> clipStack = new Stack<AudioClip>();

        // OnEnable is called when the object is loaded, e.g., when the game starts.
        private void OnEnable()
        {
            // We initialize lastTimePlayed to allow the first sound to play immediately,
            // respecting the cooldown.
            lastTimePlayed = -cooldown;
            //Debug.Log($"[{this.name}] SFX Asset Enabled/Loaded. Cooldown is {cooldown}s. Initial lastTimePlayed set to: {lastTimePlayed}", this);
        }

        public override bool CanPlayNow()
        {
            // Attention, CanPlayNow est global, c'est voulu, mais ça peut surprendre en débug.
            // l'objectif c'est que si on a l'armée des droides qui tire en même temps (exemple random)
            // on ne sature pas d'un coup.
            bool canPlay = (Time.time - lastTimePlayed) >= cooldown;
            //Debug.Log($"[{this.name}] CanPlayNow() check: (Current Time {Time.time} - Last Played {lastTimePlayed}) >= Cooldown {cooldown}. Result: {canPlay}", this);

            int count = AltifoxAudioManager.Instance.GetSFXInstanceCount(this);
            canPlay = canPlay && (AltifoxAudioManager.Instance.GetSFXInstanceCount(this) < maxInstances);
            //Debug.Log($"[{this.name}] CanPlayNow() secoond check: get instance count: {count}, max instance = {maxInstances} Result: {canPlay}", this);
            return canPlay;


        }

        public override float GetVolume()
        {
            return volume.SampleValue();
        }

        public override float GetPitch()
        {
            return pitch.SampleValue();
        }

        public override float GetSpatialBlend()
        {
            return spatialBlend;
        }

        public override AltifoxSFX GetSFXObject()
        {
            return this;
        }

        public override void TagPlayTime()
        {
            lastTimePlayed = Time.time;
            //Debug.Log($"[{this.name}] TagPlayTime() called. New lastTimePlayed: {lastTimePlayed}", this);
        }

        public override AudioClip GetAudioClip()
        {
            //Debug.Log($"[{this.name}] GetAudioClip() called. Requesting clip with playback type: {playbackType}", this);

            // Sécurité : s'assurer qu'il y a des clips à jouer
            if (audioClips == null || audioClips.Length == 0)
            {
                //Debug.LogError($"[{this.name}] GetAudioClip() FAILED: The 'audioClips' array is null or empty! Cannot provide a clip.", this);
                return null;
            }

            switch (playbackType)
            {
                case PlaybackTools.PlaybackType.Sequential:
                    int previousClip = lastPlayedClip;
                    lastPlayedClip = (lastPlayedClip + 1) % audioClips.Length;
                    //Debug.Log($"[{this.name}] Sequential mode: Previous index was {previousClip}. New index is {lastPlayedClip}. Playing '{audioClips[lastPlayedClip].name}'.", this);
                    return audioClips[lastPlayedClip];

                case PlaybackTools.PlaybackType.Random:
                    int randomIndex = UnityEngine.Random.Range(0, audioClips.Length);
                    AudioClip randomClip = audioClips[randomIndex];
                    //Debug.Log($"[{this.name}] Random mode: Selected random index {randomIndex}. Playing '{randomClip.name}'.", this);
                    return randomClip;

                case PlaybackTools.PlaybackType.Shuffle:
                    if (clipStack.Count == 0)
                    {
                        //Debug.Log($"[{this.name}] Shuffle mode: Clip stack is empty. Refilling and shuffling {audioClips.Length} clips.", this);

                        // La stack est vide, on la remplit avec une nouvelle séquence mélangée
                        var shuffledClips = audioClips.OrderBy(c => Guid.NewGuid()).ToList();
                        foreach (var clip in shuffledClips)
                        {
                            clipStack.Push(clip);
                        }
                        //Debug.Log($"[{this.name}] Shuffle mode: Stack refilled. New stack contains {clipStack.Count} clips.", this);
                    }

                    // Peek at the clip to log its name before it's removed
                    AudioClip poppedClip = clipStack.Peek();
                    //Debug.Log($"[{this.name}] Shuffle mode: Popping clip '{poppedClip.name}' from stack. {clipStack.Count - 1} clips will remain.", this);
                    return clipStack.Pop();

                default:
                    // Comportement par défaut si un type n'est pas géré
                    //Debug.LogWarning($"[{this.name}] Unhandled playbackType '{playbackType}'. Falling back to default behavior (playing first clip).", this);
                    return audioClips[0];
            }
        }
    }
}