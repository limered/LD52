using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Levels;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Systems.UI
{
    public class InventoryComponent : GameComponent
    {
        public Sprite[] arrowSprites;
        public GameObject arrowElementPrefab;
        public GameObject arrows;
        public Sprite primarySprite;
        public Sprite secondarySprite;
        public Image image;

        public void StartDrescher()
        {
            var newValue = !IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value;
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = newValue;
        }
    }
}