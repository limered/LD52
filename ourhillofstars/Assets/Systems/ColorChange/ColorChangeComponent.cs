using System;
using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.ColorChange
{
    public class ColorChangeComponent : GameComponent
    {
        public Color color;

        [NonSerialized] public Renderer myRenderer;

        public ReactiveCommand<bool> ColorChangeCommand = new();
        public ReactiveProperty<bool> BoolReactiveProp = new ReactiveProperty<bool>(false);
    }
}