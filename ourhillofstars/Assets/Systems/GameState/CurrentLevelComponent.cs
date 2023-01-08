using SystemBase.Core;
using Systems.Levels;
using UniRx;
using UnityEngine.Serialization;

namespace Systems.GameState
{
    public class CurrentLevelComponent : GameComponent
    {
        public LevelSo Level { get; set; }
        public bool IsPaused { get; set; }
        public ReactiveProperty<int> arrowsUsed = new();

        public BoolReactiveProperty harvesterRunning = new();
    }
}