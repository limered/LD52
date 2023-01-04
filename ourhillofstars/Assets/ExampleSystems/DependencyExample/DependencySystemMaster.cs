using ExampleSystems.Example;
using SystemBase.Core;

namespace ExampleSystems.DependencyExample
{
    [GameSystem]
    public class DependencySystemMaster : GameSystem<FunnyMovementComponent>
    {
        public override void Register(FunnyMovementComponent component)
        {
        }
    }
}
