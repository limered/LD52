using SystemBase.Core;
using UniRx;
using UnityEngine;

namespace Systems.Drescher
{
    [GameSystem]
    public class DrescherParticleSystem : GameSystem<DrescherParticleComponent>
    {
        public override void Register(DrescherParticleComponent component)
        {
            var particleSystem = component.GetComponentInChildren<ParticleSystem>();
            
            MessageBroker.Default.Receive<HarvestedMsg>()
                .Subscribe(msg =>
                {
                    particleSystem.Emit(40);
                })
                .AddTo(component);
        }
    }
}