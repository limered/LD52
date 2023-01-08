using SystemBase.Core;
using UniRx;

namespace Systems.Levels
{
    public class CurrentLevelComponent : GameComponent
    {
        public Level Level { get; set; }
        public ReactiveProperty<int> topArrows = new();
        public ReactiveProperty<int> leftArrows = new();
        public ReactiveProperty<int> rightArrows = new();
        public ReactiveProperty<int> bottomArrows = new();
        
        public ReactiveProperty<int> maxTopArrows = new();
        public ReactiveProperty<int> maxLeftArrows = new();
        public ReactiveProperty<int> maxRightArrows = new();
        public ReactiveProperty<int> maxBottomArrows = new();

        public BoolReactiveProperty harvesterRunning = new();
    }
}