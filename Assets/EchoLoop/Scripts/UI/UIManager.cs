using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace EchoLoop.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Panels")]
        public GameObject winPanel;
        public GameObject hudPanel;

        [Header("Win Panel")]
        public Text winLoopCountText;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            // Wire win panel buttons before deactivating
            if (winPanel != null)
            {
                Button[] buttons = winPanel.GetComponentsInChildren<Button>(true);
                foreach (var btn in buttons)
                {
                    if (btn.gameObject.name == "RestartButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(RestartGame);
                    }
                    else if (btn.gameObject.name == "MenuButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(GoToMainMenu);
                    }
                }

                winPanel.SetActive(false);
            }

            if (hudPanel != null) hudPanel.SetActive(true);
        }

        public void ShowWinScreen(int loopCount)
        {
            if (winPanel != null)
            {
                winPanel.SetActive(true);
                winPanel.transform.SetAsLastSibling(); // Bring to front
                if (winLoopCountText != null)
                    winLoopCountText.text = $"You escaped in {loopCount} loops!\nYour past self saved you.";
            }
            else
            {
                Debug.LogError("UIManager: winPanel is null! Cannot show win screen.");
            }
            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }
    }
}
