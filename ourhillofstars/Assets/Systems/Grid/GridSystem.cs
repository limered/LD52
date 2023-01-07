using SystemBase.Core;
using Systems.GridRendering;

namespace Systems.Grid
{
    [GameSystem(typeof(GridRenderingSystem))]
    public class GridSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.backgroundGrid = new GameGrid(component.dimensions.x, component.dimensions.y);
            component.foregroundGrid = new GameGrid(component.dimensions.x, component.dimensions.y);
            
            component.gridsInitialized.Execute(component);
        }
    }
}