using System;
using SystemBase.Core;
using UnityEngine.UI;
using Systems.Levels;
using UniRx;
using UnityEngine;


namespace Systems.UI
{
    public class PauseMenuComponent : GameComponent
    {
        public float volume = 0.5f;
        [NonSerialized] public ReactiveProperty<bool> isPaused = new();

        public void SetVolume()
        {
            var slider = GetComponent<Slider>();
            volume = slider.value;
        }

        public void ExitPause()
        {
            gameObject.SetActive(false);
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