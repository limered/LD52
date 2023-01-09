using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Theme;
using UniRx;
using UnityEngine;

namespace Systems.Drescher
{
    [GameSystem]
    public class DrescherParticleSystem : GameSystem<DrescherParticleComponent>
    {
        public override void Register(DrescherParticleComponent component)
        {
            // this is only called when selecting level from overview
            MessageBroker.Default.Receive<SpawnPlayerMessage>()
                .Subscribe(_ =>
                {
                    var particleSystemRenderer = component.GetComponentInChildren<ParticleSystemRenderer>();
                    var currentLevelComponent = IoC.Game.GetComponent<CurrentLevelComponent>();
                    var theme = IoC.Game.GetComponent<ThemeComponent>().harvestParticleThemes[currentLevelComponent.Level.themeFile];
                    particleSystemRenderer.material.mainTexture = theme.texture;
                })
                .AddTo(component);

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