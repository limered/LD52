using SystemBase.Core;
using UniRx;

namespace Systems.Levels
{
    public class LevelCellComponent : GameComponent
    {
        public int? level = null;

        public void LoadLevel()
        {
            if(level.HasValue) MessageBroker.Default.Publish(new LoadLevelMsg{LevelIndex = level.Value});
        }
    }
}