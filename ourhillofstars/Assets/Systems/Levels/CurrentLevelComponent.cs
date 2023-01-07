using SystemBase.Core;
using UniRx;

namespace Systems.Levels
{
    public class CurrentLevelComponent : GameComponent
    {
        public ReactiveProperty<int> rightArrow = new();

        public BoolReactiveProperty harvesterRunning = new();
        
    }
}