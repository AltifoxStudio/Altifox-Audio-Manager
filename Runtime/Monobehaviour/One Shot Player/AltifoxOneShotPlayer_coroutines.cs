using UnityEngine;
using System.Collections;

namespace AltifoxStudio.AltifoxAudioManager
{
    public partial class AltifoxOneShotPlayer : MonoBehaviour
    {
        private IEnumerator CR_TrackNRelease(int audioSourceID)
        {
            if (assignedAudioSources.TryGetValue(audioSourceID, out AltifoxAudioSourceBase assignedAudioSource))
            {
                if (assignedAudioSource.clip != null)
                {
                    //Debug.Log($"Clip Lenght is {assignedAudioSource.clip.length}, pitch is {assignedAudioSource.pitch}");
                    float duration = assignedAudioSource.clip.length / assignedAudioSource.pitch;
                    //Debug.Log($"duration is {duration}");
                    yield return new WaitForSeconds(duration);

                    AltifoxAudioManager.Instance.ReleaseAltifoxAudioSource(assignedAudioSource);
                    assignedAudioSources.Remove(audioSourceID);
                    AltifoxAudioManager.Instance.SubstractReferenceInCount(altifoxSFX.GetSFXObject());
                }
            }

        }
    }
}