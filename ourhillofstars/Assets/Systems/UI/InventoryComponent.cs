using SystemBase.Core;
using Systems.UI.Events;
using UniRx;
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
            MessageBroker.Default
            .Publish(
                new UpdateGameStateMessage {}
            );
        }
    }
}