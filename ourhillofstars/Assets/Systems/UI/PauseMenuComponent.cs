using SystemBase.CommonSystems.Audio;
using SystemBase.CommonSystems.Audio.Actions;
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
        public Slider volumeSlider;

        protected override void OverwriteStart()
        {
            base.OverwriteStart();
            volumeSlider.value = PlayerPrefs.GetFloat("volume", volumeSlider.value);
            MessageBroker.Default.Publish(new AudioActSFXSetVolume(volumeSlider.value));
        }

        public void SetVolume()
        {
            PlayerPrefs.SetFloat("volume", volumeSlider.value);
            MessageBroker.Default.Publish(new AudioActSFXSetVolume(volumeSlider.value));
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