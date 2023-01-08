using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
using Systems.GameState;
using Systems.Levels;
using UnityEngine;
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
        private Image _someImage;

        public void StartDrescher()
        {
            var oldValue = IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value;
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = !oldValue;
            _someImage = GetComponentsInChildren<Image>().First(image => image.sprite.name == primarySprite.name);
            _someImage.sprite = secondarySprite;
            (secondarySprite, primarySprite) = (primarySprite, secondarySprite);
        }
    }
}