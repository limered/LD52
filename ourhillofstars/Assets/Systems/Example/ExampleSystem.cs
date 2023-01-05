using SystemBase.Core;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using Random = Unity.Mathematics.Random;

namespace Systems.Example
{
    [GameSystem]
    public class ExampleSystem : GameSystem<ExampleComponent>
    {
        private Random _rnd = new(123);
        
        public override void Register(ExampleComponent component)
        {
            component.UpdateAsObservable()
                .Where(_ => Time.frameCount % 20 == 0)
                .Subscribe(_ => Animate(component))
                .AddTo(component);
        }

        private void Animate(ExampleComponent component)
        {
            var dir = _rnd.NextFloat3() * component.speed;
            
            component.GetComponent<Rigidbody>().AddForce(dir);
        }
    }
}