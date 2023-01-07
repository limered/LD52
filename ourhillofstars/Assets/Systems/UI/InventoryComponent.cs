using System.Linq;
using SystemBase.Core;
using SystemBase.Utils;
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
        private Image _image;

        public void StartDrescher()
        {
            var oldValue = IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value;
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = !oldValue;
            _image = GetComponentsInChildren<Image>().First(image => image.sprite.name == primarySprite.name);
            _image.sprite = secondarySprite;
            (secondarySprite, primarySprite) = (primarySprite, secondarySprite);
        }
    }
}