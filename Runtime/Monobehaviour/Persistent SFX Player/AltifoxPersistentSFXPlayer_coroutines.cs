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

        private IEnumerator CR_ManageLoopRegion(float loopStart = NO_CUSTOM_LOOP_START, float loopEnd = NO_CUSTOM_LOOP_END)
        {
            if (audioSource.clip == null)
            {
                Debug.LogError("Reference source or clip is missing!");
                yield break;
            }

            float loopEndTime = (loopEnd != NO_CUSTOM_LOOP_END) ? loopEnd : audioSource.clip.length;
            
            // SÉCURITÉ : Valider que les temps de boucle sont logiques
            if (loopStart >= loopEndTime)
            {
                Debug.LogError("Loop start time must be less than loop end time.");
                yield break;
            }

            if (useDoubleBuffering)
            {
                audioSource.PrepareNextSource(loopStart);
            }

            // Pas besoin de toutes ces variables, on peut simplifier
            // float loopDuration = loopEndTime - loopStart; 
            // float firstLoopDuration = loopEndTime;
            float targetlooptime = loopEndTime;

            while (looping)
            {
                // SÉCURITÉ : La condition de la boucle vérifie maintenant si l'audio est en lecture.
                // C'est une façon plus propre d'attendre que d'utiliser une boucle while manuelle.
                //yield return new WaitUntil(() => audioSource.time >= targetlooptime || !audioSource.isPlaying);

                // Si on sort de l'attente parce que la lecture s'est arrêtée (et non parce qu'on a atteint la fin de la boucle),
                // on quitte la coroutine pour éviter une boucle infinie.
                // if (!audioSource.isPlaying && audioSource.time < targetlooptime)
                // {
                //     Debug.LogWarning("AudioSource stopped playing unexpectedly. Exiting loop coroutine.");
                //     yield break;
                // }

                if (audioSource.time < targetlooptime)
                {
                    //Debug.LogWarning("AudioSource stopped playing unexpectedly. Exiting loop coroutine.");
                    yield break;
                }


                float restartTime = loopStart;

                if (!useDoubleBuffering)
                {
                    audioSource.time = restartTime;
                    // SÉCURITÉ : Utiliser Play() est plus robuste que UnPause()
                    audioSource.Play();
                }
                else
                {
                    audioSource.Flip();
                    audioSource.PrepareNextSource(loopStart);
                }

                // On ajuste la cible pour les boucles suivantes
                targetlooptime = loopEndTime; 
            }
        }

    }
}