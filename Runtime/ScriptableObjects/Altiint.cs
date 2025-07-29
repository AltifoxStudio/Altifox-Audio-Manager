using UnityEngine;
using System;
namespace AltifoxStudio.AltifoxAudioManager
{
    [CreateAssetMenu(fileName = "Altiint", menuName = "AudioParameters/Altiint", order = 0)]
    public class Altiint : AltifoxParameter<int>
    {
        public override float ValueAsFloat => (float)_value;
    }
}