using SystemBase.Core;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Systems.UI.End
{
    public class EndScreenComponent : GameComponent
    {
        public void RestartAllLevels()
        {
            Debug.Log("Restart all levels");
            SceneManager.LoadScene("Main");
        }
    }
}