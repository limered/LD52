using SystemBase.Core;
using Systems.Levels;
using UniRx;
using UnityEngine.SceneManagement;

namespace Systems.UI.End
{
    public class EndScreenComponent : GameComponent
    {
        public void RestartAllLevels()
        {
            SceneManager.LoadScene("Main");
        }

        public void GoToLevelOverview()
        {
            gameObject.SetActive(false);
            MessageBroker.Default.Publish(new ShowLevelOverviewMsg());
        }
    }
}