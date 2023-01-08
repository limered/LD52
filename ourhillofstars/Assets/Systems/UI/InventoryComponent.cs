using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
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

        public void StartDrescher()
        {
            var newValue = !IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value;
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = newValue;
        }
    }
}