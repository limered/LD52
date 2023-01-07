using SystemBase.Core;
using Systems.GridRendering;

namespace Systems.Grid
{
    [GameSystem(typeof(GridRenderingSystem))]
    public class GridSystem : GameSystem<MainGridComponent>
    {
        public override void Register(MainGridComponent component)
        {
            component.grid = new GameGrid(component.dimensions.x, component.dimensions.y);
            component.gridLoaded.Execute(component);
        }
    }
}