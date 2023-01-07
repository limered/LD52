using SystemBase.Core;
using UniRx;

namespace Systems.Levels
{
    public class CurrentLevelComponent : GameComponent
    {
        public ReactiveProperty<int> topArrow = new();
        public ReactiveProperty<int> leftArrow = new();
        public ReactiveProperty<int> rightArrow = new();
        public ReactiveProperty<int> bottomArrow = new();

        public BoolReactiveProperty harvesterRunning = new();
    }
}