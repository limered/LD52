using SystemBase;
using SystemBase.Core;
using SystemBase.GameState.States;
using SystemBase.Utils;
using Systems.ColorChange;
using UniRx;
using UnityEngine;

namespace Systems.MyInput
{
    [GameSystem]
    public class InputSystem : GameSystem<ColorChangeComponent>
    {
        public override void Register(ColorChangeComponent component)
        {
            SystemUpdate(component)
                .Where(_ => ColorChangeKeyPressed())
                .Where(_ => IoC.Game.gameStateContext.CurrentState.Value is Running)
                .Subscribe(CheckInput)
                .AddTo(component);
        }

        private static void CheckInput(ColorChangeComponent colorChangeComponent)
        {
            colorChangeComponent.ColorChangeCommand.ForceExecute(false);
        }
        
        private static bool ColorChangeKeyPressed()
        {
            return Input.GetKey("a");
        }
    }
}