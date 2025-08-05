using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;


namespace AltifoxStudio.AltifoxAudioManager
{
    public partial class AltifoxPersistentSFXPlayer : MonoBehaviour
    {
        private IEnumerator CR_FadeOut(float duration, InterpolationType transition, bool releaseSources = true)
        {
            Vector2 startPoint;
            Vector2 zeroPoint = new Vector2(duration, 0f);


            if (duration <= 0f)
            {
                audioSource.volume = 0f;
                yield break;
            }
            else
            {

                startPoint = new Vector2(0f, audioSource.volume);

            }

            System.Func<Vector2, Vector2, float, float> interpolationFunction = Interpolations.GetInterpolationFuncRef(transition);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                elapsedTime += Time.deltaTime;
                audioSource.volume = interpolationFunction(startPoint, zeroPoint, elapsedTime);
                yield return null;
            }

            isPlaying = false;

        }

private IEnumerator CR_ManageLoopRegion()
        {
            //Debug.Log("Starting the loop");

            MusicLoop currentLoop = loop;

            ////Debug.Log($"Starting Loop ! from time = {loopStart} to time = {loopEnd}");

            if (audioSource == null || audioSource.GetAudioSource().clip == null)
            {
                //Debug.LogError("Error: Reference source or clip is missing!");
                yield break;
            }


            bool donePreparingLoop = false;
            while (!exitLoopFlag)
            {
                if (audioSource.time <= currentLoop.StartTime + 1.0f && donePreparingLoop)
                {
                    donePreparingLoop = false;
                }

                float currentInt = (int)audioSource.time;
                if ((int)audioSource.time > currentInt)
                {
                    currentInt = (int)audioSource.time;
                    //Debug.Log($"Next SFX loop in : {audioSource.time - currentLoop.EndTime}, first loop ? {currentLoop.IsOnFirstPlaythrough}");
                }

                if (audioSource.time + 1.0f > currentLoop.EndTime && !donePreparingLoop)
                {
                    //Debug.Log($"Prepare for resert SFX loop: ResetPoint: {currentLoop.StartTime}, {currentLoop.EndTime}");
                    double dspNow = AudioSettings.dspTime;
                    double NextLoopEventTime = dspNow + (currentLoop.EndTime - audioSource.time);
                    donePreparingLoop = true;

                    if (useDoubleBuffering)
                    {
                        audioSource.PrepareNextSource(currentLoop.StartTime, currentLoop.EndTime, NextLoopEventTime);
                        audioSource.Flip();
                        if (currentLoop.IsOnFirstPlaythrough)
                        {
                            currentLoop.AdvanceToLoop();
                        }
                    }

                    else
                    {
                        //Debug.LogError($"Error in SFX {this.name}: looping tracks require the use of double buffering");
                        yield break;
                    }



                }

                yield return null;
            }

            //Debug.Log("Stoppped Looping");
            yield break;
        }

    }
}