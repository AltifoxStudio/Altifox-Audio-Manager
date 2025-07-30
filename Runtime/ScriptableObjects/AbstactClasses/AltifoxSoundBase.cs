using UnityEngine;

namespace AltifoxStudio.AltifoxAudioManager
{
    public abstract class AltifoxSoundBase : ScriptableObject
    {
        public abstract float GetVolume();
        public abstract float GetPitch();
        public abstract float GetSpatialBlend();
        public abstract bool CanPlayNow();
        public abstract void TagPlayTime();
        public abstract AudioClip GetAudioClip();
        public abstract AltifoxSFX GetSFXObject();

        public abstract bool GetSpatializeBool();

        public abstract float GetMaxSpatialDistance();
        public abstract float GetMinSpatialDistance();
    }
}