using SystemBase.Core;
using Systems.Levels.Events;
using UniRx;

namespace Systems.UI.End
{
    [GameSystem]
    public class EndScreenSystem : GameSystem<EndScreenComponent>
    {
        public override void Register(EndScreenComponent component)
        {
            component.gameObject.SetActive(false);
            
            MessageBroker.Default.Receive<FinishLastLevelMsg>()
                .Subscribe(_ => RenderEndScreen(component))
                .AddTo(component);
        }

        private void RenderEndScreen(EndScreenComponent component)
        {
            component.gameObject.SetActive(true);
        }
    }
}