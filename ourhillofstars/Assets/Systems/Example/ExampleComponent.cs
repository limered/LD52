using SystemBase.Core;
using UnityEngine;

namespace Systems.Example
{
    [RequireComponent(typeof(Rigidbody))]
    public class ExampleComponent : GameComponent
    {
        public float speed;
    }
}