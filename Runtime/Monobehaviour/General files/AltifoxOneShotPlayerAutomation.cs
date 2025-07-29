using UnityEngine;
using System;


namespace AltifoxStudio.AltifoxAudioManager
{
    public class AltifoxOneShotPlayerAutomation : MonoBehaviour
    {
        public AltifoxParameterBase parameter;
        public Automation[] automations;

        public AltifoxOneShotPlayer altifoxPlayer;

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
                float valueToSet = interpolationFunction(automation.startKey, automation.endKey, v);
                switch (automation.audioSourceParameter)
                {
                    case AudioSourceParameter.volume:
                        altifoxPlayer.SetParameterBuffer(AudioSourceParameter.volume, valueToSet);
                        break;
                    case AudioSourceParameter.pitch:
                        altifoxPlayer.SetParameterBuffer(AudioSourceParameter.pitch, valueToSet);
                        break;
                    case AudioSourceParameter.semitones:
                        altifoxPlayer.SetParameterBuffer(AudioSourceParameter.semitones, valueToSet);
                        break;
                    default:
                        break;
                }
            }


        }
    }
}