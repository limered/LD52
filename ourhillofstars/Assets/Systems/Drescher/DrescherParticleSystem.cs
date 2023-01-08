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
                    particleSystem.transform.position = new Vector3(msg.coord.x, particleSystem.transform.position.y, msg.coord.y);
                    particleSystem.Emit(40);
                })
                .AddTo(component);
        }
    }
}