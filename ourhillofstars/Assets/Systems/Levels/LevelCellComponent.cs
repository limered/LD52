using SystemBase.Core;
using UniRx;

namespace Systems.Levels
{
    public class LevelCellComponent : GameComponent
    {
        public int level = 0;

        public void LoadLevel()
        {
            MessageBroker.Default.Publish(new LoadLevelMsg{LevelIndex = level});
        }
    }
}