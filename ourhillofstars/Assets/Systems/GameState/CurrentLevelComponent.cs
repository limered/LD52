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

        public Grade CurrentGrade
        {
            get
            {
                var arrows = arrowsUsed.Value;

                return arrows < Level.aGradeCount ? Grade.S :
                    arrows >= Level.aGradeCount && arrows < Level.bGradeCount ? Grade.A :
                    arrows >= Level.bGradeCount && arrows < Level.cGradeCount ? Grade.B :
                    Grade.C;
            }
        }
    }
}