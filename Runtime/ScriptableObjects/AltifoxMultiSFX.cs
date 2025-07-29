using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AltifoxStudio.AltifoxAudioManager
{
    [Serializable]
    public class SFXContainer
    {
        [Header("First entry is the default one")]
        public string key = "Enter Key here";
        public AltifoxSFX altifoxSFX;
    }


    [CreateAssetMenu(fileName = "AltifoxMultiSFX", menuName = "AltifoxMultiSFX", order = 0)]
    public class AltifoxMultiSFX : AltifoxSoundBase
    {
        [Header("SFX Mapping")]
        [Tooltip("List of sound effects mapped by a string key (e.g., 'Grass', 'Wood').")]
        public List<SFXContainer> containers;

        [Header("Fallback")]
        [Tooltip("The SFX to play if the provided key is not found.")]

        private AltifoxSFX defaultSFX;
        private string currentKey;

        private void OnEnable()
        {
            try
            {
                currentKey = containers[0].key;
                defaultSFX = containers[0].altifoxSFX;
            }
            catch (System.Exception)
            {
                Debug.LogError($"Cannot initialize multi SFX {this.name} no entry has been defined in the container");
                throw;
            }

        }

        public void SetKey(string key)
        {
            currentKey = key;
        }

        public override bool CanPlayNow()
        {
            AltifoxSFX SFX = GetAltifoxSFX();
            return SFX.CanPlayNow();
        }

        public override void TagPlayTime()
        {
            AltifoxSFX SFX = GetAltifoxSFX();
            SFX.TagPlayTime();
        }

        public override AudioClip GetAudioClip()
        {
            return GetAltifoxSFX().GetAudioClip();
        }


        public override AltifoxSFX GetSFXObject()
        {
            return GetAltifoxSFX();
        }

        private AltifoxSFX GetAltifoxSFX()
        {
            SFXContainer container = containers.FirstOrDefault(e => e.key == currentKey);
            if (container != null && container.altifoxSFX != null)
            {
                //Debug.Log($"Returning AltifoxSFX: {container.altifoxSFX.name}");
                return container.altifoxSFX;
            }

            if (container == null)
            {
                Debug.LogWarning($"[{this.name}] SFX key '{currentKey}' not found, and no default SFX is set!", this);
            }

            return defaultSFX;
        }

        public override float GetVolume()
        {
            return GetAltifoxSFX().volume.SampleValue();
        }

        public override float GetPitch()
        {
            return GetAltifoxSFX().pitch.SampleValue();
        }

        public override float GetSpatialBlend()
        {
            return GetAltifoxSFX().spatialBlend;
        }

    }
}