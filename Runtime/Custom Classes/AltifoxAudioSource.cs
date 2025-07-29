using UnityEngine;

namespace AltifoxStudio.AltifoxAudioManager
{
    public abstract class AltifoxAudioSourceBase : MonoBehaviour
    {
        // These are the "active" and "passive" sources respectively.
        public abstract AudioSource GetAudioSource();
        public abstract AudioSource GetSecondaryAudioSource();
        public abstract void Flip();

        public abstract void Pause();
        public abstract void UnPause();
        public abstract void Stop();
        public abstract void Play();
        public abstract void PrepareNextSource(float time);

        // Define abstract properties with get and set accessors.
        public abstract float volume { get; set; }
        public abstract float pitch { get; set; }
        public abstract float time { get; set; }
        public abstract float spatialBlend { get; set; }

        public abstract bool mute { get; set; }
        public abstract bool spatialize { get; set; }
        public abstract bool isPlaying { get; }

        public abstract AudioClip clip { get; set; }

    }

    //--- CORRECTED SIMPLE SOURCE ---
    // Best practice: Use RequireComponent to ensure the source exists.
    [RequireComponent(typeof(AudioSource))]
    public class AltifoxAudioSource : AltifoxAudioSourceBase
    {
        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public override AudioSource GetAudioSource()
        {
            return audioSource;
        }

        public override void PrepareNextSource(global::System.Single time)
        {
            // On passe
        }

        public override AudioSource GetSecondaryAudioSource()
        {
            return null;
        }

        public override AudioClip clip
        {
            get => audioSource.clip;
            set => audioSource.clip = value;
        }

        public override bool isPlaying
        {
            get => audioSource.isPlaying;
        }

        public override bool mute
        {
            get => audioSource.mute;
            set => audioSource.mute = value;
        }

        public override bool spatialize
        {
            get => audioSource.spatialize;
            set => audioSource.spatialize = value;
        }

        public override float time
        {
            get => audioSource.time;
            set => audioSource.time = value;
        }

        public override float spatialBlend
        {
            get => audioSource.spatialBlend;
            set => audioSource.spatialBlend = value;
        }

        public override float volume
        {
            get => audioSource.volume;
            set => audioSource.volume = value;
        }

        public override float pitch
        {
            get => audioSource.pitch;
            set => audioSource.pitch = value;
        }
        public override void Flip()
        {
            // On passe
        }

        public override void Pause()
        {
            audioSource.Pause();
        }

        public override void UnPause()
        {
            audioSource.UnPause();
        }

        public override void Stop()
        {
            audioSource.Stop();
        }

        public override void Play()
        {
            audioSource.Play();
        }
    }

    //--- CORRECTED DOUBLE BUFFER SOURCE ---
    public class AltifoxDoubleBufferAudioSource : AltifoxAudioSourceBase
    {
        private AudioSource[] audioSources = new AudioSource[2];
        private int flipper = 0;

        private void Awake()
        {
            audioSources[0] = gameObject.AddComponent<AudioSource>();
            audioSources[1] = gameObject.AddComponent<AudioSource>();
        }

        public override AudioClip clip
        {
            get => audioSources[flipper].clip;
            set
            {
                audioSources[0].clip = value;
                audioSources[1].clip = value;
            }
        }

        // Implement the volume property.
        public override float volume
        {
            get => audioSources[flipper].volume;
            set
            {
                audioSources[0].volume = value;
                audioSources[1].volume = value;
            }
        }

        public override float time
        {
            get => audioSources[flipper].time;
            set => audioSources[flipper].time = value;
        }

        public override float spatialBlend
        {
            get => audioSources[flipper].spatialBlend;
            set
            {
                audioSources[0].spatialBlend = value;
                audioSources[1].spatialBlend = value;
            }
        }

        public override bool isPlaying
        {
            get => audioSources[flipper].isPlaying;
        }

        public override bool mute
        {
            get => audioSources[flipper].mute;
            set
            {
                audioSources[0].mute = value;
                audioSources[1].mute = value;
            }
        }

        public override bool spatialize
        {
            get => audioSources[flipper].spatialize;
            set
            {
                audioSources[0].spatialize = value;
                audioSources[1].spatialize = value;
            }
        }

        public override float pitch
        {
            get => audioSources[flipper].pitch;
            set
            {
                audioSources[0].pitch = value;
                audioSources[1].pitch = value;
            }
        }

        public override void Flip()
        {
            audioSources[flipper].Stop();
            audioSources[(flipper + 1) % 2].Play();
            flipper = (flipper + 1) % 2;
        }

        public override AudioSource GetAudioSource()
        {
            return audioSources[flipper];
        }

        public override AudioSource GetSecondaryAudioSource()
        {
            return audioSources[(flipper + 1) % 2];
        }

        public override void Pause()
        {
            audioSources[flipper].Pause();
        }

        public override void UnPause()
        {
            audioSources[flipper].UnPause();
        }

        public override void Stop()
        {
            audioSources[flipper].Stop();
        }

        public override void Play()
        {
            audioSources[flipper].Play();
        }

        public override void PrepareNextSource(float time)
        {
            audioSources[(flipper + 1) % 2].time = time;
        }

    }
}