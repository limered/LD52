using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Levels.Events;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace Systems.UI
{
    public class InventoryComponent : GameComponent
    {
        public Sprite[] arrowSprites;
        public GameObject arrowElementPrefab;
        public GameObject arrows;
        public Sprite startButtonSprite;
        public Sprite resetButtonSprite;
        public Image startStopButtonImage;
        public GameObject nextLvl;

        public void StartDrescher()
        {
            var newValue = !IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value;
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = newValue;
        }

        public void GoToNextLvl()
        {
            nextLvl.gameObject.SetActive(false);
            MessageBroker.Default.Publish(new AskToGoToNextLevelMsg());
        }
    }
}