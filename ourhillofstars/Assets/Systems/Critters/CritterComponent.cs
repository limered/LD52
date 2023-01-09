using System;
using SystemBase.Core;
using UniRx;
using Unity.Mathematics;
using UnityEngine;

namespace Systems.Critters
{
    public class CritterComponent : GameComponent
    {
        public CritterType type;
        public ReactiveProperty<float2> movementDirection = new();
        public float speed = 0.5f;
        public float maxDistance = 1f;
        public Vector3 startPosition;
        public Texture2D[] images;
        
        [NonSerialized]public Rigidbody cachedBody;
        [NonSerialized]public Renderer cachedRenderer;
    }
}