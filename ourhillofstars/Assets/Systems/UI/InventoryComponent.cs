using SystemBase.Core;
using SystemBase.Utils;
using Systems.Levels;
using UnityEngine;

namespace Systems.UI
{
    public class InventoryComponent : GameComponent
    {
        public Sprite[] arrowSprites;
        public GameObject arrowElementPrefab;
        public GameObject arrows;

        public void StartDrescher()
        {
            var oldValue = IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value;
            IoC.Game.GetComponent<CurrentLevelComponent>().harvesterRunning.Value = !oldValue;
        }
    }
}