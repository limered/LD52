using System;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using UnityEngine.UI;
using Systems.Levels;
using UniRx;
using UnityEngine;


namespace Systems.UI
{
    public class PauseMenuComponent : GameComponent
    {
        public float volume = 0.5f;

        public void SetVolume()
        {
            var slider = GetComponent<Slider>();
            volume = slider.value;
        }

        public void ExitPause()
        {
            var currentLevel = IoC.Game.GetComponent<CurrentLevelComponent>();
            gameObject.SetActive(false);
            if (currentLevel.GameState == GameState.GameState.LevelSelect) return;
            currentLevel.IsPaused.Value = false;
        }

        public void ExitGame()
        {
            Application.Quit();
        }

        public void ShowLevelOverview()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new ShowLevelOverviewMsg());
        }
    }
}