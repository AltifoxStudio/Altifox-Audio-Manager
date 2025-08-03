using UnityEngine;
using System.Collections.Generic;


namespace AltifoxStudio.AltifoxAudioManager
{
    public class PlayConfig
    {
        public Dictionary<string, AltifoxAudioSourceBase> musicLayers = new Dictionary<string, AltifoxAudioSourceBase>(); // The layers in the music
        public Dictionary<string, float> layerPlayVolume = new Dictionary<string, float>(); // This is the default volume of each layer
        public Dictionary<string, Coroutine> activeTransitions = new Dictionary<string, Coroutine>(); // This tracks the transitions to avoid conflicts
        public Dictionary<string, bool> layerIsActive = new Dictionary<string, bool>(); // and this final one tracks if a layer is active
        public bool isSet = false;
    }
    /// <summary>
    /// The persistent player should be used to play loops such as musics and soundscapes in a lesser way
    /// For soundscapes, I'm going to implement a specific object that takes into account the animals and the background noise
    /// </summary>
    public partial class AltifoxPersistentPlayer : MonoBehaviour
    {

        public AltifoxPlaylist playlist;

        private bool swappedOnce = false;

        // I hide this in inspector cause it's just a container that will be loaded with the 
        // currently playing music
        [HideInInspector]
        public AltifoxMusic altifoxMusicSO;
        public string currentPlayingTrack;
        private AltifoxMusic altifoxMusicSO_Temp;
        public static AltifoxPersistentPlayer Instance { get; private set; }

        public Dictionary<string, AltifoxMusic> playlistTracks = new Dictionary<string, AltifoxMusic>(); // this one just lists the tracks in the playlist
        public Dictionary<string, PlayConfig> tracksConfig = new Dictionary<string, PlayConfig>();
            

        // ========================================================================
        // Dictionaires 
        // ========================================================================
        public bool playOnAwake;
        public bool useDoubleBuffering = true;
        private bool isPlaying;
        private double dspTimeAtPlay;
        public bool looping;
        private const float NO_CUSTOM_LOOP_START = 0f;
        private const float NO_CUSTOM_LOOP_END = -1f;
        private LoopRegion[] loopRegions;
        public bool exitLoopFlag = false;
        public bool stopLoopingFullFlag = false;
        public int aimForLoopID = -1;
        public int currentLoopRegion = 0;
        private Coroutine loopTracking;
        /// <summary>
        /// Very simple awake, with singleton pattern
        /// the idea is to populate the playlist from the playlist Scriptable Object
        /// to avoid modifying it directly
        /// </summary>
        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            Instance = this;
            //DontDestroyOnLoad(this.gameObject);

            // initialisation du dictionaire pour la playlist
            for (int i = 0; i < playlist.Items.Length; i++)
            {
                PlayConfig playConfig = new PlayConfig();
                tracksConfig.Add(playlist.Items[i].name, playConfig);
                playlistTracks.Add(playlist.Items[i].name, playlist.Items[i].altifoxMusic);
            }
            currentPlayingTrack = playlist.Items[0].name;
            // Puis on récupere le default depuis le ScriptableObject
            altifoxMusicSO = playlistTracks[playlist.defaultMusic];
        }

        public void ChangeActiveTrackTo(string trackName, bool fadeOut = true)
        {
            if (fadeOut)
            {
                string[] layersToFade = { "All" };
                Coroutine CRfadeOut = StartCoroutine(CR_FadeOutLayers(layersToFade, 2, altifoxMusicSO.transitions, true, tracksConfig[currentPlayingTrack]));
                try
                {
                    StopCoroutine(loopTracking);
                }
                catch (System.Exception)
                {
                    // pass
                }
            }

            if (playlistTracks.TryGetValue(trackName, out AltifoxMusic nextTrack))
            {
                altifoxMusicSO = nextTrack;
                loopRegions = altifoxMusicSO.loopRegions;
                currentPlayingTrack = trackName;
            }
            isPlaying = false;
            Play();

        }

        private void initPlayer()
        {
            tracksConfig[currentPlayingTrack].layerPlayVolume.Clear();
            tracksConfig[currentPlayingTrack].layerIsActive.Clear();
            for (int i = 0; i < altifoxMusicSO.musicLayers.Length; i++)
            {
                MusicLayer layerConfig = altifoxMusicSO.musicLayers[i];
                tracksConfig[currentPlayingTrack].layerPlayVolume.Add(layerConfig.name, layerConfig.activeVolume);
                tracksConfig[currentPlayingTrack].layerIsActive.Add(layerConfig.name, false);
            }
            loopRegions = altifoxMusicSO.loopRegions;
        }

        /// <summary>
        /// When reaching start, we gather the data from the different layers of the
        /// current music and call Play() if the user clicked on PlayOnAwake
        /// </summary>
        private void Start()
        {
            initPlayer();

            if (playOnAwake)
            {
                Play();
            }
        }

        public void SetForPlay(string track = "none")
        {
            string trackName;
            if (track == "none")
            {
                altifoxMusicSO_Temp = altifoxMusicSO;
                trackName = currentPlayingTrack;
            }
            else
            {
                trackName = track;
            }
            PlayConfig localPlayConfig = tracksConfig[trackName];
            bool instantPause = false;
            if (playlistTracks.TryGetValue(track, out AltifoxMusic nextTrack))
            {
                altifoxMusicSO_Temp = nextTrack;
                instantPause = true;
            }

            for (int i = 0; i < altifoxMusicSO_Temp.musicLayers.Length; i++)
            {
                AltifoxAudioSourceBase newAS;
                MusicLayer layerConfig = altifoxMusicSO_Temp.musicLayers[i];
                if (!useDoubleBuffering)
                {
                    newAS = AltifoxAudioManager.Instance.RequestSBAltifoxAudioSource();
                }
                else
                {
                    newAS = AltifoxAudioManager.Instance.RequestDBAltifoxAudioSource();
                }

                newAS.clip = layerConfig.audioClip;
                newAS.spatialize = layerConfig.spatialize;
                newAS.spatialBlend = layerConfig.spatialBlend;
                newAS.outputAudioMixerGroup = layerConfig.targetMixer;

                if (layerConfig.activeByDefault)
                {
                    newAS.volume = 1f;
                }
                else
                {
                    newAS.volume = 0f;
                }

                localPlayConfig.musicLayers[layerConfig.name] = newAS;
                localPlayConfig.layerIsActive[layerConfig.name] = true;
            }
            localPlayConfig.isSet = true;

            foreach (AltifoxAudioSourceBase layer in localPlayConfig.musicLayers.Values)
            {
                layer.Play();
                layer.Pause();
            }
        }

        // public void SwapPlayConfig()
        // {
        //     float t0 = Time.time;
        //     Debug.Log("Start Swapping");
        //     tempPlayConfig = tracksConfig[currentPlayingTrack];
        //     float t1 = Time.time;
        //     tracksConfig[currentPlayingTrack] = playConfigNext;
        //     float t2 = Time.time;
        //     playConfigNext = tempPlayConfig;
        //     float t3 = Time.time;
        //     loopRegions = altifoxMusicSO.loopRegions;
        //     float t4 = Time.time;
        //     isPlaying = false;
        //     Play();
        //     float t5 = Time.time;
        //     Debug.Log($"times: t1: {t1-t0}, t2: {t2-t0}, t3: {t3-t0}, t4: {t4-t0}, t5: {t5-t0}");
        // }

        /// <summary>
        /// Plays the track with the proper configuration of the different layers
        /// </summary>
        public void Play()
        {
            if (isPlaying)
            {
                return;
            }

            foreach (AltifoxAudioSourceBase layer in tracksConfig[currentPlayingTrack].musicLayers.Values)
            {
                layer.UnPause();
            }
            dspTimeAtPlay = AudioSettings.dspTime;
            currentLoopRegion = 0;
            if (altifoxMusicSO.loopRegions.Length > 0)
            {
                loopTracking = StartCoroutine(CR_ManageLoopRegion());
            }
            isPlaying = true;
        }

        public void Pause()
        {
            foreach (KeyValuePair<string, AltifoxAudioSourceBase> layer in tracksConfig[currentPlayingTrack].musicLayers)
            {
                string layerName = layer.Key;
                AltifoxAudioSourceBase audioSource = layer.Value;
                audioSource.Pause();
            }
            isPlaying = false;
        }

        public void UnPause()
        {
            foreach (KeyValuePair<string, AltifoxAudioSourceBase> layer in tracksConfig[currentPlayingTrack].musicLayers)
            {
                string layerName = layer.Key;
                AltifoxAudioSourceBase audioSource = layer.Value;
                audioSource.UnPause();
            }
            isPlaying = true;
        }

        /// <summary>
        /// Just a wrapper for the CR_FadeOutLayers coroutine
        /// </summary>
        /// <param name="layersToFade"></param>
        /// <param name="duration"></param>
        /// <param name="transition"></param>
        /// <param name="releaseSources"></param>
        public void FadeOutLayers(string[] layersToFade, float duration, InterpolationType transition, bool releaseSources = true)
        {
            Coroutine newTransition = StartCoroutine(CR_FadeOutLayers(layersToFade, duration, altifoxMusicSO.transitions, releaseSources));
        }

        public void FadeOutAllLayers()
        {
            string[] layersToFade = { "All" };
            Coroutine fadeOut = StartCoroutine(CR_FadeOutLayers(layersToFade, 2, altifoxMusicSO.transitions, true));
        }

        public void SetLayerActive(string layerName, bool active, bool fade = true)
        {
            if (!isPlaying)
            {
                Play();
            }
            if (tracksConfig[currentPlayingTrack].musicLayers.TryGetValue(layerName, out AltifoxAudioSourceBase AS))
            {
                if (tracksConfig[currentPlayingTrack].activeTransitions.TryGetValue(layerName, out Coroutine runningTransition))
                {
                    if (runningTransition != null)
                    {
                        StopCoroutine(runningTransition);
                    }
                }

                if (!fade)
                {
                    AS.volume = active ? 1.0f : 0.0f;
                }
                float targetVolume = active ? 1.0f : 0.0f;
                float duration = altifoxMusicSO.transitionTime;

                tracksConfig[currentPlayingTrack].layerPlayVolume[layerName] = active ? tracksConfig[currentPlayingTrack].layerPlayVolume[layerName] : AS.volume;
                tracksConfig[currentPlayingTrack].layerIsActive[layerName] = active;

                Coroutine newTransition = StartCoroutine(CR_TransitionLayer(AS, targetVolume * tracksConfig[currentPlayingTrack].layerPlayVolume[layerName], duration, altifoxMusicSO.transitions));
                tracksConfig[currentPlayingTrack].activeTransitions[layerName] = newTransition;
            }

            else
            {
                Debug.LogWarning($"Warning: trying to activate layer '{layerName}' but this layer does not exist");
            }

        }

        public bool checkLayerState(string layer)
        {
            return tracksConfig[currentPlayingTrack].layerIsActive[layer];
        }
    }
}