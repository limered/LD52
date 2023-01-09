using SystemBase.Core;
using UniRx;
using UnityEngine.UI;

namespace Systems.Levels
{
    public class LevelButtonComponent : GameComponent
    {
        public int? level = null;
        public Image grade;

        public void LoadLevel()
        {
            if(level.HasValue) MessageBroker.Default.Publish(new LoadLevelMsg{LevelIndex = level.Value});
        }
    }
}