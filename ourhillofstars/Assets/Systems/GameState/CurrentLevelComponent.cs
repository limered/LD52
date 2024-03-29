﻿using SystemBase.Core;
using Systems.Levels;
using UniRx;

namespace Systems.GameState
{
    public enum GameState
    {
        Playing,
        Paused,
        LevelSelect,
    }
    public class CurrentLevelComponent : GameComponent
    {
        public Level Level { get; set; }
        public ReactiveProperty<bool> IsPaused = new();
        public GameState GameState { get; set; }
        public ReactiveProperty<int> arrowsUsed = new();

        public BoolReactiveProperty harvesterRunning = new();

        public Grade CurrentGrade
        {
            get
            {
                var arrows = arrowsUsed.Value;
                if (arrows == 0) return Grade.None;

                return arrows < Level.aGradeCount ? Grade.S :
                    arrows >= Level.aGradeCount && arrows < Level.bGradeCount ? Grade.A :
                    arrows >= Level.bGradeCount && arrows < Level.cGradeCount ? Grade.B :
                    Grade.C;
            }
        }
    }
}