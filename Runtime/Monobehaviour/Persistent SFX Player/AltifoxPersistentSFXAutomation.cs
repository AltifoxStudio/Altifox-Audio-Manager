using UnityEngine;
using System;

namespace AltifoxStudio.AltifoxAudioManager
{
    [Serializable]
    public struct SFXAutomation
    {
        public AudioSourceParameter audioSourceParameter;
        public Vector2 startKey;
        public Vector2 endKey;
        public InterpolationType interpolationType;
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(AltifoxPersistentPlayer))]
    public class AltifoxPersistentSFXAutomation : MonoBehaviour
    {
        public AltifoxParameterBase parameter;
        public SFXAutomation[] automations;

        private AltifoxPersistentSFXPlayer altifoxPlayer;

        private void Start()
        {
            altifoxPlayer = GetComponent<AltifoxPersistentSFXPlayer>();
        }

        private void OnEnable()
        {
            parameter.OnValueChangedAsFloat += UpdateParameters;
        }

        private void OnDisable()
        {
            parameter.OnValueChangedAsFloat -= UpdateParameters;
        }

        private void UpdateParameters(float v)
        {
            System.Func<Vector2, Vector2, float, float> interpolationFunction;
            foreach (Automation automation in automations)
            {
                interpolationFunction = Interpolations.GetInterpolationFuncRef(automation.interpolationType);
                switch (automation.audioSourceParameter)
                {
                    case AudioSourceParameter.volume:
                        altifoxPlayer.audioSource.volume = interpolationFunction(automation.startKey, automation.endKey, v);
                        break;
                    case AudioSourceParameter.pitch:
                        altifoxPlayer.audioSource.pitch = interpolationFunction(automation.startKey, automation.endKey, v);
                        break;
                    case AudioSourceParameter.semitones:
                        altifoxPlayer.audioSource.pitch = Tones.SemitonesToPitch(interpolationFunction(automation.startKey, automation.endKey, v));
                        break;
                    default:
                        break;
                }

            }
        }
    }
}