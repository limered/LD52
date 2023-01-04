using SystemBase.Core;
using UniRx;

namespace ExampleSystems.Example
{
    public class FunnyMovementConfigComponent : GameComponent
    {
        public FloatReactiveProperty Speed = new FloatReactiveProperty(10);
        public StateContext<FunnyMovementComponent> MovementState = new StateContext<FunnyMovementComponent>();
    }
}