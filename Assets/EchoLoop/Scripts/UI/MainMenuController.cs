using UnityEngine;
using UnityEngine.SceneManagement;

namespace EchoLoop.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public void PlayGame()
        {
            Debug.Log("PLAY pressed - loading Prototype");
            SceneManager.LoadScene("Prototype", LoadSceneMode.Single);
        }

        public void QuitGame()
        {
            Debug.Log("QUIT pressed");
            Application.Quit();
        }
    }
}
