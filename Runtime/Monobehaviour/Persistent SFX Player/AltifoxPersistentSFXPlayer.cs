using UnityEngine;
using System.Collections.Generic;


namespace AltifoxStudio.AltifoxAudioManager
{
    /// <summary>
    /// The persistent player should be used to play loops such as musics and soundscapes in a lesser way
    /// For soundscapes, I'm going to implement a specific object that takes into account the animals and the background noise
    /// </summary>
    public partial class AltifoxPersistentSFXPlayer : MonoBehaviour
    {


        // I hide this in inspector cause it's just a container that will be loaded with the 
        // currently playing music
        public AltifoxSFX altifoxSFX;

        // ========================================================================
        // Dictionaires 
        // ========================================================================
        [HideInInspector]
        public AltifoxAudioSourceBase audioSource; // The layers in the music
        public bool playOnAwake;
        public bool useDoubleBuffering = true;
        private bool isPlaying;
        private double dspTimeAtPlay;
        public bool looping;
        public float loopStartTime = 0;
        public float loopEndTime = -1f;
        private const float NO_CUSTOM_LOOP_START = 0f;
        private const float NO_CUSTOM_LOOP_END = -1f;

        /// <summary>
        /// When reaching start, we gather the data from the different layers of the
        /// current music and call Play() if the user clicked on PlayOnAwake
        /// </summary>
        private void Start()
        {

            altifoxSFX.volume.SampleValue();

            if (playOnAwake)
            {
                Play();
            }
        }

        /// <summary>
        /// Plays the track with the proper configuration of the different layers
        /// </summary>
        public void Play()
        {
            if (isPlaying)
            {
                return;
            }
        
            if (!useDoubleBuffering)
            {
                audioSource = AltifoxAudioManager.Instance.RequestSBAltifoxAudioSource();
            }
            else
            {
                audioSource = AltifoxAudioManager.Instance.RequestDBAltifoxAudioSource();
            }

            audioSource.clip = altifoxSFX.GetAudioClip();
            audioSource.spatialize = altifoxSFX.GetSpatializeBool();
            audioSource.spatialBlend = altifoxSFX.GetSpatialBlend();
            audioSource.maxDistance = altifoxSFX.GetMaxSpatialDistance();
            audioSource.minDistance = altifoxSFX.GetMinSpatialDistance();


            audioSource.Play();
            audioSource.gameObject.transform.SetParent(this.transform);
            audioSource.gameObject.transform.position = this.transform.position;

            dspTimeAtPlay = AudioSettings.dspTime;
            Coroutine loopTracking = StartCoroutine(CR_ManageLoopRegion(loopStartTime, loopEndTime));
            isPlaying = true;
        }

        public void Pause()
        {
            audioSource.Pause();
            isPlaying = false;
        }

        public void UnPause()
        {
            audioSource.UnPause();
            isPlaying = true;
        }

        /// <summary>
        /// Just a wrapper for the CR_FadeOutLayers coroutine
        /// </summary>
        /// <param name="layersToFade"></param>
        /// <param name="duration"></param>
        /// <param name="transition"></param>
        /// <param name="releaseSources"></param>
        public void FadeOut(float duration, InterpolationType transition, bool releaseSources = true)
        {
            Coroutine newTransition = StartCoroutine(CR_FadeOut(duration, InterpolationType.Smooth, releaseSources));
        }

    }
}