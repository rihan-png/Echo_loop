using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace EchoLoop.UI
{
    public class MainMenuController : MonoBehaviour
    {
        private RectTransform playButtonRect;
        private RectTransform quitButtonRect;
        private Canvas canvas;

        private void Start()
        {
            canvas = FindFirstObjectByType<Canvas>();

            GameObject playObj = GameObject.Find("PlayButton");
            GameObject quitObj = GameObject.Find("QuitButton");

            if (playObj != null) playButtonRect = playObj.GetComponent<RectTransform>();
            if (quitObj != null) quitButtonRect = quitObj.GetComponent<RectTransform>();
        }

        private void Update()
        {
            // Press SPACE or ENTER to play immediately
            if (Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame ||
                 Keyboard.current.enterKey.wasPressedThisFrame))
            {
                PlayGame();
                return;
            }

            // Mouse click detection
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector2 mousePos = Mouse.current.position.ReadValue();

                if (playButtonRect != null && RectTransformUtility.RectangleContainsScreenPoint(playButtonRect, mousePos))
                {
                    PlayGame();
                }
                else if (quitButtonRect != null && RectTransformUtility.RectangleContainsScreenPoint(quitButtonRect, mousePos))
                {
                    QuitGame();
                }
            }
        }

        public void PlayGame()
        {
            Debug.Log("Loading Prototype...");
            SceneManager.LoadScene("Prototype");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
