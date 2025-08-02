using UnityEngine;
using System;

namespace AltifoxStudio.AltifoxAudioManager
{
    [Serializable]
    public struct Automation
    {
        public string affectedLayerName;
        public AudioSourceParameter audioSourceParameter;
        public Vector2 startKey;
        public Vector2 endKey;
        public InterpolationType interpolationType;
    }
    public enum AudioSourceParameter
    {
        volume,
        pitch,
        semitones,
        nullParameter,
    }

    /// <summary>
    /// 
    /// </summary>
    [RequireComponent(typeof(AltifoxPersistentPlayer))]
    public class AltifoxPersistentAutomation : MonoBehaviour
    {
        public AltifoxParameterBase parameter;
        public Automation[] automations;

        private AltifoxPersistentPlayer altifoxPlayer;

        private void Start()
        {
            altifoxPlayer = GetComponent<AltifoxPersistentPlayer>();
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
                if (altifoxPlayer.tracksConfig[altifoxPlayer.altifoxMusicSO.name].musicLayers.TryGetValue(automation.affectedLayerName, out AltifoxAudioSourceBase audioSource))
                {

                    interpolationFunction = Interpolations.GetInterpolationFuncRef(automation.interpolationType);
                    switch (automation.audioSourceParameter)
                    {
                        case AudioSourceParameter.volume:
                            audioSource.volume = interpolationFunction(automation.startKey, automation.endKey, v);
                            break;
                        case AudioSourceParameter.pitch:
                            audioSource.pitch = interpolationFunction(automation.startKey, automation.endKey, v);
                            break;
                        case AudioSourceParameter.semitones:
                            audioSource.pitch = Tones.SemitonesToPitch(interpolationFunction(automation.startKey, automation.endKey, v));
                            break;
                        default:
                            break;
                    }

                }
                else if (automation.affectedLayerName == "All")
                {
                    foreach (AltifoxAudioSourceBase AS in altifoxPlayer.tracksConfig[altifoxPlayer.altifoxMusicSO.name].musicLayers.Values)
                    {
                        interpolationFunction = Interpolations.GetInterpolationFuncRef(automation.interpolationType);
                        switch (automation.audioSourceParameter)
                        {
                            case AudioSourceParameter.volume:
                                AS.volume = interpolationFunction(automation.startKey, automation.endKey, v);
                                break;
                            case AudioSourceParameter.pitch:
                                AS.pitch = interpolationFunction(automation.startKey, automation.endKey, v);
                                break;
                            case AudioSourceParameter.semitones:
                                AS.pitch = Tones.SemitonesToPitch(interpolationFunction(automation.startKey, automation.endKey, v));
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }
    }
}