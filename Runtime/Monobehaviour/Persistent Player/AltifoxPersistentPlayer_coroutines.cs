using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace AltifoxStudio.AltifoxAudioManager
{
    public partial class AltifoxPersistentPlayer : MonoBehaviour
    {
        private IEnumerator CR_FadeOutLayers(string[] layersToFade, float duration, InterpolationType transition, bool releaseSources = true, PlayConfig playConfigTarget = null)
        {
            AltifoxAudioSourceBase[] audioSources;
            List<Vector2> startPoints = new List<Vector2>();
            Vector2 zeroPoint = new Vector2(duration, 0f);

            if (playConfigTarget != null)
            {
                playConfigTarget = tracksConfig[currentPlayingTrack];
            }

            if (layersToFade[0] == "All")
                {
                    audioSources = playConfigTarget.musicLayers.Values.ToArray();
                }
                else
                {
                    audioSources = layersToFade.Select(key => playConfigTarget.musicLayers[key]).ToArray();
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


        }

        private IEnumerator CR_ManageLoopRegion()
        {
            //Debug.Log("Starting the loop");
            aimForLoopID = -1;
            Debug.Log(currentLoopRegion);
            try
            {
                currentLoop = loopRegions[currentLoopRegion];
            }
            catch (System.Exception)
            {
                Debug.LogWarning($"Trying to access loop number {currentLoopRegion} in {currentPlayingTrack}");
            }
            

            ////Debug.Log($"Starting Loop ! from time = {loopStart} to time = {loopEnd}");
            AltifoxAudioSourceBase referenceSource = tracksConfig[currentPlayingTrack].musicLayers.Values.FirstOrDefault();
            bool ContinueCheck = false;
            while (!ContinueCheck)
            {
                if (referenceSource == null || referenceSource.GetAudioSource().clip == null)
                {
                    referenceSource = tracksConfig[currentPlayingTrack].musicLayers.Values.FirstOrDefault();
                    //Debug.LogWarning("Error: Reference source or clip is missing this is probably a init race condition ! trying again in 0.5 seconds");
                    yield return new WaitForSeconds(0.5f);

                }
                else
                {
                    ContinueCheck = true;
                }
            }
            

            while (!stopLoopingFullFlag)
                {
                    Debug.Log(referenceSource);
                    Debug.Log(currentLoop);

                    // while (!exitLoopFlag)
                    // {
                    //     if (stopLoopingFullFlag)
                    //     {
                    //         exitLoopFlag = true;
                    //     }

                    //     ////Debug.Log($"continuing Loop ! from time = {loopStart} to time = {loopEndTime}, time = {referenceSource.time}");
                    //     double lastDspTime = AudioSettings.dspTime;
                    //     float progressInAudioSeconds = 0;

                    //     while (progressInAudioSeconds < targetlooptime)
                    //     {
                    //         ////Debug.Log($"Looping for now: target: {targetlooptime}");
                    //         progressInAudioSeconds = referenceSource.time;
                    //         yield return null;
                    //     }
                    //     //targetlooptime = loopDuration;
                    //     for (int i = 0; i < altifoxMusicSO.musicLayers.Length; i++)
                    //     {
                    //         MusicLayer layerConfig = altifoxMusicSO.musicLayers[i];
                    //         if (layerConfig.deactivateOnLoop)
                    //         {
                    //             tracksConfig[currentPlayingTrack].musicLayers[layerConfig.name].mute = true;
                    //         }
                    //     }

                    //     float restartTime = loopStart;
                    //     foreach (AltifoxAudioSourceBase source in tracksConfig[currentPlayingTrack].musicLayers.Values)
                    //     {
                    //         if (!useDoubleBuffering)
                    //         {
                    //             source.time = restartTime;
                    //             if (!source.isPlaying)
                    //             {
                    //                 source.UnPause();
                    //             }
                    //         }
                    //         else
                    //         {
                    //             source.Flip();
                    //             source.PrepareNextSource(loopStart);
                    //         }

                    //     }
                    //     //yield return new WaitUntil(() => referenceSource.time >= targetTime || !referenceSource.isPlaying);
                    // }
                    bool donePreparingLoop = false;
                    while (!exitLoopFlag)
                    {
                        if (referenceSource.time <= currentLoop.StartTime + 1.0f && donePreparingLoop)
                        {
                            donePreparingLoop = false;
                        }
                        float currentInt = (int)referenceSource.time;
                        if ((int)referenceSource.time > currentInt)
                        {
                            currentInt = (int)referenceSource.time;
                            //Debug.Log($"Next loop in : {referenceSource.time - currentLoop.EndTime}, first loop ? {currentLoop.IsOnFirstPlaythrough}");
                        }

                        if (referenceSource.time + 1.0f > currentLoop.EndTime && !donePreparingLoop)
                        {
                            //Debug.Log($"Prepare for resert: ResetPoint: {currentLoop.StartTime}, {currentLoop.EndTime}");
                            double dspNow = AudioSettings.dspTime;
                            double NextLoopEventTime = dspNow + (currentLoop.EndTime - referenceSource.time);
                            donePreparingLoop = true;
                            foreach (AltifoxAudioSourceBase source in tracksConfig[currentPlayingTrack].musicLayers.Values)
                            {
                                if (useDoubleBuffering)
                                {
                                    source.PrepareNextSource(currentLoop.StartTime, currentLoop.EndTime, NextLoopEventTime);
                                    source.Flip();
                                    if (currentLoop.IsOnFirstPlaythrough)
                                    {
                                        currentLoop.AdvanceToLoop();
                                    }
                                }

                                else
                                {
                                    //Debug.LogError($"Error in track {currentPlayingTrack}: looping tracks require the use of double buffering");
                                    yield break;
                                }

                            }


                        }

                        yield return null;
                    }


                    if (aimForLoopID >= 0)
                    {
                        currentLoopRegion = aimForLoopID;
                        exitLoopFlag = false;
                        currentLoop = loopRegions[currentLoopRegion];
                    }

                    else
                    {
                        yield break;
                    }
                }
            //Debug.Log("Stoppped Looping");
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