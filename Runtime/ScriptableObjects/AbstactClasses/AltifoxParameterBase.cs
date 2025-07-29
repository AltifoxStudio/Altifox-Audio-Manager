using UnityEngine;
using System;
using System.Collections.Generic;

namespace AltifoxStudio.AltifoxAudioManager
{
    public abstract class AltifoxParameterBase : ScriptableObject
    {
        public abstract float ValueAsFloat { get; }
        public abstract event Action<float> OnValueChangedAsFloat;
    }

    public abstract class AltifoxParameter<T> : AltifoxParameterBase
    {
        public event Action<T> OnValueChanged;
        public override event Action<float> OnValueChangedAsFloat;

        [SerializeField]
        protected T _value;

        public virtual T Value
        {
            get => _value;
            set
            {
                if (EqualityComparer<T>.Default.Equals(_value, value)) return;

                _value = value;
                OnValueChanged?.Invoke(_value);
                OnValueChangedAsFloat?.Invoke(ValueAsFloat);
            }
        }
    }
}