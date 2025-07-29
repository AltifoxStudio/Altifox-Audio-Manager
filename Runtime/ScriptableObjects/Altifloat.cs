using UnityEngine;
using System;

[CreateAssetMenu(fileName = "Altifloat", menuName = "AudioParameters/Altifloat", order = 0)]
public class Altifloat : AltifoxParameter<float>
{
    public override float ValueAsFloat => (float)_value;
}