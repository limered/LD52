using SystemBase.Core;
<<<<<<< Updated upstream
using Systems.Levels;
using UniRx;
using UnityEngine;
=======
using UnityEngine.Serialization;
>>>>>>> Stashed changes
using UnityEngine.UI;

namespace Systems.UI
{
    public class PauseMenuComponent : GameComponent
    {
        public float volume = 0.5f;

        public void SetVolume()
        {
            var slider = GetComponent<Slider>();
<<<<<<< Updated upstream
=======
            
>>>>>>> Stashed changes
            volume = slider.value;
        }

        public void ExitPause()
        {
<<<<<<< Updated upstream
            gameObject.SetActive(false);
=======
            
>>>>>>> Stashed changes
        }

        public void ExitGame()
        {
<<<<<<< Updated upstream
            Application.Quit();
        }

        public void ShowLevelOverview()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new ShowLevelOverviewMsg());

=======
            
>>>>>>> Stashed changes
        }
    }
}