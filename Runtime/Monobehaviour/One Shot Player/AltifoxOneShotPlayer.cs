using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace AltifoxStudio.AltifoxAudioManager
{
    public struct ParameterBuffer
    {
        public AudioSourceParameter parameter;
        public float value;
    }

    /// <summary>
    /// Defines events that can trigger the sound to play.
    /// The [Flags] attribute allows selecting multiple events in the Inspector.
    /// </summary>
    [System.Flags]
    public enum PlayOn
    {
        // Use powers of 2 for values, which is easily done with bit-shifting (1 << n)
        Enable = 1 << 1,  // 4
        Disable = 1 << 2,  // 8
        Destroy = 1 << 3,  // 16
        OnTriggerEnter = 1 << 4,  // 32
        OnTriggerExit = 1 << 5,  // 64
        OnTriggerStay = 1 << 6,  // 128
        OnCollisionEnter = 1 << 7,  // 256
        OnCollisionExit = 1 << 8,  // 512
        OnCollisionStay = 1 << 9, // 1024
        OnControllerColliderHit = 1 << 10, // 2048
        OnMouseDown = 1 << 11, // 4096
        OnMouseDrag = 1 << 12,
        OnMouseEnter = 1 << 13,
        OnMouseExit = 1 << 14,
        OnMouseOver = 1 << 15,
        OnMouseUp = 1 << 16,
        OnMouseUpAsButton = 1 << 17,
        OnParticleCollision = 1 << 18,
        OnParticleTrigger = 1 << 19,
        OnValidate = 1 << 20,
        OnBecameVisible = 1 << 21,
        OnBecameInvisible = 1 << 22,
        OnApplicationPause = 1 << 23,
        OnUpdate = 1 << 24,
        OnFixedUpdate = 1 << 25,
        OnPointerEnter = 1 << 26,
    }

    public partial class AltifoxOneShotPlayer : MonoBehaviour
    {
        [Header("Playback Settings")]
        [Tooltip("Select the event(s) that will trigger this sound.")]
        public PlayOn playOnEvents;

        [Header("Sound Definition")]
        public AltifoxSoundBase altifoxSFX;
        public bool Preload = false;

        public AltifoxAudioSourceBase LastAssignedAudioSource;

        private Dictionary<int, AltifoxAudioSourceBase> assignedAudioSources = new Dictionary<int, AltifoxAudioSourceBase>();
        private Dictionary<int, bool> loaded = new Dictionary<int, bool>();
        private int IDCount = 0;

        private ParameterBuffer parameterBuffer;

        const int CANNOT_PLAY = -2;
        const int INIT_STATE = -1;

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Section: Unity Event Message Handlers
        // Each method checks if its corresponding flag is set in 'playOnEvents'.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #region Unity Event Handlers
        void OnEnable()
        {
            if ((playOnEvents & PlayOn.Enable) != 0) PreloadAndPlay();
        }

        void OnDisable()
        {
            if ((playOnEvents & PlayOn.Disable) != 0) PreloadAndPlay();
        }

        void OnDestroy()
        {
            if ((playOnEvents & PlayOn.Destroy) != 0) PreloadAndPlay();
        }

        void OnTriggerEnter(Collider other)
        {
            if ((playOnEvents & PlayOn.OnTriggerEnter) != 0) PreloadAndPlay();
        }

        void OnTriggerExit(Collider other)
        {
            if ((playOnEvents & PlayOn.OnTriggerExit) != 0) PreloadAndPlay();
        }

        void OnTriggerStay(Collider other)
        {
            if ((playOnEvents & PlayOn.OnTriggerStay) != 0) PreloadAndPlay();
        }

        void OnCollisionEnter(Collision collision)
        {
            if ((playOnEvents & PlayOn.OnCollisionEnter) != 0) PreloadAndPlay();
        }

        void OnCollisionExit(Collision collision)
        {
            if ((playOnEvents & PlayOn.OnCollisionExit) != 0) PreloadAndPlay();
        }

        void OnCollisionStay(Collision collision)
        {
            if ((playOnEvents & PlayOn.OnCollisionStay) != 0) PreloadAndPlay();
        }

        void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if ((playOnEvents & PlayOn.OnControllerColliderHit) != 0) PreloadAndPlay();
        }

        void OnMouseDown()
        {
            if ((playOnEvents & PlayOn.OnMouseDown) != 0) PreloadAndPlay();
        }

        void OnMouseDrag()
        {
            if ((playOnEvents & PlayOn.OnMouseDrag) != 0) PreloadAndPlay();
        }

        void OnMouseEnter()
        {
            if ((playOnEvents & PlayOn.OnMouseEnter) != 0) PreloadAndPlay();
        }

        void OnMouseExit()
        {
            if ((playOnEvents & PlayOn.OnMouseExit) != 0) PreloadAndPlay();
        }

        void OnMouseOver()
        {
            if ((playOnEvents & PlayOn.OnMouseOver) != 0) PreloadAndPlay();
        }

        void OnMouseUp()
        {
            if ((playOnEvents & PlayOn.OnMouseUp) != 0) PreloadAndPlay();
        }

        void OnMouseUpAsButton()
        {
            if ((playOnEvents & PlayOn.OnMouseUpAsButton) != 0) PreloadAndPlay();
        }

        void OnParticleCollision(GameObject other)
        {
            if ((playOnEvents & PlayOn.OnParticleCollision) != 0) PreloadAndPlay();
        }

        void OnParticleTrigger()
        {
            if ((playOnEvents & PlayOn.OnParticleTrigger) != 0) PreloadAndPlay();
        }

        void OnValidate()
        {
            if ((playOnEvents & PlayOn.OnValidate) != 0) PreloadAndPlay();
        }

        void OnBecameVisible()
        {
            if ((playOnEvents & PlayOn.OnBecameVisible) != 0) PreloadAndPlay();
        }

        void OnBecameInvisible()
        {
            if ((playOnEvents & PlayOn.OnBecameInvisible) != 0) PreloadAndPlay();
        }

        void OnApplicationPause(bool pauseStatus)
        {
            // Note: This fires when the app is paused AND when it is resumed.
            if ((playOnEvents & PlayOn.OnApplicationPause) != 0) PreloadAndPlay();
        }

        void Update()
        {
            // Warning: This will attempt to play the sound every single frame.
            // Use with caution and ensure your altifoxSFX.CanPlayNow() has proper cooldown logic.
            if ((playOnEvents & PlayOn.OnUpdate) != 0) PreloadAndPlay();
        }

        void FixedUpdate()
        {
            // Warning: This will attempt to play the sound every fixed physics step.
            // Use with caution and ensure your altifoxSFX.CanPlayNow() has proper cooldown logic.
            if ((playOnEvents & PlayOn.OnFixedUpdate) != 0) PreloadAndPlay();
        }

        void OnPointerEnter()
        {
            if ((playOnEvents & PlayOn.OnPointerEnter) != 0) PreloadAndPlay();
        }
        #endregion

        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        // Section: Core Audio Logic
        // Your original methods for preloading, playing, and tracking audio.
        // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

        #region Core Logic

        private void Start()
        {
            parameterBuffer.parameter = AudioSourceParameter.nullParameter;
        }

        public void SetParameterBuffer(AudioSourceParameter paramToAutomate, float value)
        {
            //Debug.Log("Setting Parameter bu");
            parameterBuffer.parameter = paramToAutomate;
            parameterBuffer.value = value;
        }

        public void ResetParameterBuffer()
        {
            parameterBuffer.parameter = AudioSourceParameter.nullParameter;
        }

        private void ApplyParameterBuffer(AltifoxAudioSourceBase audioSource)
        {
            //Debug.Log("Applying Parameter buffer");
            switch (parameterBuffer.parameter)
            {
                case AudioSourceParameter.volume:
                    audioSource.volume = parameterBuffer.value;
                    break;
                case AudioSourceParameter.pitch:
                    audioSource.pitch = parameterBuffer.value;
                    break;
                case AudioSourceParameter.semitones:
                    audioSource.pitch = Tones.SemitonesToPitch(parameterBuffer.value);
                    break;
                default:
                    break;
            }
            //parameterBuffer.parameter = AudioSourceParameter.nullParameter;
        }

        /// <summary>
        /// Prepares an AudioSource with a clip and settings, ready to be played.
        /// </summary>
        /// <returns>The ID of the preloaded AudioSource.</returns>
        public int PreloadClip()
        {
            AltifoxAudioSourceBase assignedAudioSource = AltifoxAudioManager.Instance.RequestSBAltifoxAudioSource();
            if (assignedAudioSource == null)
            {
                return 0;
            }
            assignedAudioSource.gameObject.transform.SetParent(this.gameObject.transform);
            assignedAudioSource.gameObject.transform.position = this.gameObject.transform.position;
            if (assignedAudioSource != null)
            {
                assignedAudioSources.Add(IDCount, assignedAudioSource);
                LastAssignedAudioSource = assignedAudioSource;
                // Assign randomized properties from the SFX definition
                assignedAudioSource.clip = altifoxSFX.GetAudioClip();
                assignedAudioSource.volume = altifoxSFX.GetVolume();
                assignedAudioSource.pitch = altifoxSFX.GetPitch();

                // Assign constant properties
                assignedAudioSource.spatialBlend = altifoxSFX.GetSpatialBlend();
                assignedAudioSource.minDistance = altifoxSFX.GetMinSpatialDistance();
                assignedAudioSource.maxDistance = altifoxSFX.GetMaxSpatialDistance();
                assignedAudioSource.spatialize = altifoxSFX.GetSpatializeBool();
                assignedAudioSource.outputAudioMixerGroup = altifoxSFX.GetTargetMixerGroup();

                loaded.Add(IDCount, true);
                IDCount++;
                return IDCount - 1;
            }
            return CANNOT_PLAY;
        }

        public void PreloadAndPlay()
        {
            if (altifoxSFX.CanPlayNow())
            {
                int audioSourceID = PreloadClip();
                Play();
            }

        }

        /// <summary>
        /// Plays the sound. If not preloaded, it will load and play immediately.
        /// </summary>
        /// <param name="audioSourceID">Optional ID of a preloaded source. If -1, plays the most recently preloaded source.</param>
        public void Play(int audioSourceID = -1)
        {
            if (altifoxSFX != null && altifoxSFX.CanPlayNow())
            {
                if (!loaded[ID])
                {
                    // If not preloaded, load it now. This will become the source to play.
                    audioSourceID = PreloadClip();
                }
                else if (audioSourceID != CANNOT_PLAY)
                {
                    if (audioSourceID == INIT_STATE && IDCount > 0)
                    {
                        // If an ID wasn't specified, use the last one that was preloaded.
                        audioSourceID = IDCount - 1;
                    }

                    if (assignedAudioSources.ContainsKey(audioSourceID))
                    {
                        if (parameterBuffer.parameter != AudioSourceParameter.nullParameter)
                        {
                            ApplyParameterBuffer(assignedAudioSources[audioSourceID]);
                            //Debug.Log($"Audiosource values: {assignedAudioSources[audioSourceID].volume},{assignedAudioSources[audioSourceID].pitch}");
                        }
                        assignedAudioSources[audioSourceID].Play();
                        AltifoxAudioManager.Instance.AddReferenceInCount(altifoxSFX.GetSFXObject());
                        StartCoroutine(CR_TrackNRelease(audioSourceID));
                        altifoxSFX.TagPlayTime();
                        loaded[ID] = false; // The clip has been used, so it's no longer considered "loaded".
                    }
                }
            }
        }

        #endregion
    }
}