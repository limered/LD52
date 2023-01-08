using SystemBase.Core;
using Systems.Grid;
using Systems.Levels;
using UniRx;

namespace Systems.UI
{
    [GameSystem]
    public class UIInfoSystem : GameSystem<UIInfoComponent>
    {
        public override void Register(UIInfoComponent component)
        {
            MessageBroker.Default.Receive<GridLoadMsg>()
                .Subscribe(msg => LoadGameInfoForUI(component, msg.Level))
                .AddTo(component);
        }

        private void LoadGameInfoForUI(UIInfoComponent component, LevelSo level)
        {
            component.levelName.text = level.name;
            component.vehicleImage.sprite = level.levelType == LevelType.Harvester ?
                component.vehicleSprites[0] : component.vehicleSprites[1];
            // component.grade.text =
        }
    }
}