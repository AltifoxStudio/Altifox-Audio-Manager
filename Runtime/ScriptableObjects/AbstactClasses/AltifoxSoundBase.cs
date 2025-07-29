using UnityEngine;
using AltifoxTools;

public abstract class AltifoxSoundBase : ScriptableObject
{
    public abstract float GetVolume();
    public abstract float GetPitch();
    public abstract float GetSpatialBlend();
    public abstract bool CanPlayNow();
    public abstract void TagPlayTime();
    public abstract AudioClip GetAudioClip();
    public abstract AltifoxSFX GetSFXObject();
}