using UnityEngine;
using UnityEngine.SceneManagement;

namespace SystemBase.Scenes
{
    public class SceneLoader : MonoBehaviour
    {
        public void StartInitialGame()
        {
            SceneManager.LoadScene("Main");
        }
    }
}