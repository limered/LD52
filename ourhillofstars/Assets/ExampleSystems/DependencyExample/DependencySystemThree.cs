using SystemBase;
using ExampleSystems.Example;
using SystemBase.Core;

namespace ExampleSystems.DependencyExample
{
    [GameSystem(typeof(DependencySystemOne), typeof(FunnyMovementSystem))]
    public class DependencySystemThree : GameSystem<FunnyMovementComponent>
    {
        public override void Register(FunnyMovementComponent component)
        {
        }
    }
}
