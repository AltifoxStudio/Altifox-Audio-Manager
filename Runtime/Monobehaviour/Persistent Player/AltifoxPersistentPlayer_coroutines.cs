using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace AltifoxStudio.AltifoxAudioManager
{
    public partial class AltifoxPersistentPlayer : MonoBehaviour
    {
        private IEnumerator CR_FadeOutLayers(string[] layersToFade, float duration, InterpolationType transition, bool releaseSources = true)
        {
            AltifoxAudioSourceBase[] audioSources;
            List<Vector2> startPoints = new List<Vector2>();
            Vector2 zeroPoint = new Vector2(duration, 0f);

            if (layersToFade[0] == "All")
            {
                audioSources = musicLayers.Values.ToArray();
            }
            else
            {
                audioSources = layersToFade.Select(key => musicLayers[key]).ToArray();
            }

            if (duration <= 0f)
            {
                foreach (AltifoxAudioSourceBase AS in audioSources)
                {
                    AS.volume = 0f;
                }
                yield break;
            }
            else
            {
                foreach (AltifoxAudioSourceBase AS in audioSources)
                {
                    startPoints.Add(new Vector2(0f, AS.volume));
                }
            }

            System.Func<Vector2, Vector2, float, float> interpolationFunction = Interpolations.GetInterpolationFuncRef(transition);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                for (int i = 0; i < audioSources.Length; i++)
                {
                    audioSources[i].volume = interpolationFunction(startPoints[i], zeroPoint, elapsedTime);
                }
                yield return null;
            }

            if (layersToFade[0] == "All")
            {
                foreach (AltifoxAudioSourceBase AS in audioSources)
                {
                    AS.volume = 0f;
                    if (releaseSources)
                    {
                        AltifoxAudioManager.Instance.ReleaseAltifoxAudioSource(AS);
                    }
                }
            }
            isPlaying = false;

        }



        private IEnumerator CR_ManageLoopRegion(float loopStart = NO_CUSTOM_LOOP_START, float loopEnd = NO_CUSTOM_LOOP_END)
        {
            AltifoxAudioSourceBase referenceSource = musicLayers.Values.FirstOrDefault();
            if (referenceSource == null || referenceSource.GetAudioSource().clip == null)
            {
                Debug.LogError("Reference source or clip is missing!");
                yield break;
            }

            foreach (AltifoxAudioSourceBase source in musicLayers.Values)
            {
                if (useDoubleBuffering)
                {
                    source.PrepareNextSource(loopStart);
                }

            }
            float loopEndTime = (loopEnd != NO_CUSTOM_LOOP_END) ? loopEnd : referenceSource.GetAudioSource().clip.length;
            float loopDuration = loopEndTime - loopStart;
            float firstLoopDuration  = loopEndTime;
            float targetlooptime = firstLoopDuration;

            while (looping)
            {
                double lastDspTime = AudioSettings.dspTime;
                float progressInAudioSeconds = 0;

                while (progressInAudioSeconds < targetlooptime)
                {
                    //Debug.Log($"Looping for now: target: {targetlooptime}");
                    yield return null;
                    progressInAudioSeconds = referenceSource.time;
                }
                //targetlooptime = loopDuration;
                for (int i = 0; i < altifoxMusicSO.musicLayers.Length; i++)
                {
                    MusicLayer layerConfig = altifoxMusicSO.musicLayers[i];
                    if (layerConfig.deactivateOnLoop)
                    {
                        musicLayers[layerConfig.name].mute = true;
                    }
                }

                float restartTime = loopStart;
                foreach (AltifoxAudioSourceBase source in musicLayers.Values)
                {
                    if (!useDoubleBuffering)
                    {
                        source.time = restartTime;
                        if (!source.isPlaying)
                        {
                            source.UnPause();
                        }
                    }
                    else
                    {
                        source.Flip();
                        source.PrepareNextSource(loopStart);
                    }

                }
                //yield return new WaitUntil(() => referenceSource.time >= targetTime || !referenceSource.isPlaying);
            }
            yield break;
        }

        private IEnumerator CR_TransitionLayer(AltifoxAudioSourceBase audioSource, float targetVolume, float duration, InterpolationType transition)
        {
            if (audioSource.mute)
            {
                audioSource.mute = false;
                if (audioSource.volume > 0f)
                {
                    targetVolume = audioSource.volume;
                    audioSource.volume = 0f;
                }
            }
            if (duration <= 0f)
            {
                audioSource.volume = targetVolume;
                if (targetVolume == 0)
                {
                    audioSource.mute = true;
                }
                yield break; // this stops the coroutine
            }
            System.Func<Vector2, Vector2, float, float> interpolationFunction = Interpolations.GetInterpolationFuncRef(transition);

            float elapsedTime = 0f;
            Vector2 startVolume = new Vector2(0f, audioSource.volume);
            Vector2 stopVolume = new Vector2(duration, targetVolume);
            float currentVolume = startVolume.y;
            while (elapsedTime < duration)
            {

                elapsedTime += Time.deltaTime;
                audioSource.volume = interpolationFunction(startVolume, stopVolume, elapsedTime); ;
                yield return null;
            }
            audioSource.volume = targetVolume;
            if (targetVolume == 0)
            {
                audioSource.mute = true;
            }
        }
    }
}