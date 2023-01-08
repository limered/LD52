using System;
using SystemBase.Core;
using UniRx;
using UnityEngine;
using UnityEngine.Serialization;


namespace Systems.Selector
{
    public class SelectorComponent : GameComponent
    {
        [NonSerialized] public Renderer myRenderer;
        [NonSerialized] public Texture availableSprite;
        public Texture notAvailableSprite;
        public Vector2Int targetCoord = new();
        public ReactiveProperty<bool> shouldBeInvisible = new();
        public ReactiveProperty<bool> shouldChangeTexture = new();
    }
}