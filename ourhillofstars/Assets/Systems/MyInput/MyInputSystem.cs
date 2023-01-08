using SystemBase.Core;
using Systems.UI;
using UniRx;
using UnityEngine;
using MessageBroker = UniRx.MessageBroker;

namespace Systems.MyInput
{
    [GameSystem]
    public class MyInputSystem : GameSystem<MyInputComponent>
    {
        public override void Register(MyInputComponent component)
        {
            Debug.Log("register my input");
            SystemUpdate(component)
                .Subscribe(_ => CheckButtonPressed() )
                .AddTo(component);
        }

        private static void CheckButtonPressed()
        {
            if (Input.GetKeyDown(("p")))
            {
                Debug.Log("Pressed p");
                MessageBroker.Default.Publish(new ShowPauseMenuMsg());
            }
        }
    }
}