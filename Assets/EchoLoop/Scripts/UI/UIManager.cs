using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

namespace EchoLoop.UI
{
    public class UIManager : MonoBehaviour
    {
        public const string GITHUB_URL = "https://github.com/rihan-png/Echo_loop";
        public static UIManager Instance;

        [Header("Panels")]
        public GameObject winPanel;
        public GameObject pausePanel;
        public GameObject hudPanel;

        [Header("Win Panel")]
        public Text winLoopCountText;

        private bool isPaused = false;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            Time.timeScale = 1f;

            // Wire Win Panel buttons
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

            // Wire Pause Panel buttons
            if (pausePanel != null)
            {
                Button[] pauseButtons = pausePanel.GetComponentsInChildren<Button>(true);
                foreach (var btn in pauseButtons)
                {
                    if (btn.gameObject.name == "ResumeButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(ResumeGame);
                    }
                    else if (btn.gameObject.name == "RestartButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(RestartGame);
                    }
                    else if (btn.gameObject.name == "MenuButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(GoToMainMenu);
                    }
                    else if (btn.gameObject.name == "GithubButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(OpenGithubRepo);
                    }
                    else if (btn.gameObject.name == "ExitButton")
                    {
                        btn.onClick.RemoveAllListeners();
                        btn.onClick.AddListener(QuitGame);
                    }
                }
                pausePanel.SetActive(false);
            }

            // Wire HUD Pause button
            Button hudPauseBtn = GameObject.Find("PauseButton")?.GetComponent<Button>();
            if (hudPauseBtn != null)
            {
                hudPauseBtn.onClick.RemoveAllListeners();
                hudPauseBtn.onClick.AddListener(TogglePause);
            }

            if (hudPanel != null) hudPanel.SetActive(true);
        }

        private void Update()
        {
            // Escape key or Android Back button toggles Pause
            if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                TogglePause();
            }
        }

        public void TogglePause()
        {
            if (winPanel != null && winPanel.activeSelf) return; // Don't pause if won

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            isPaused = true;
            Time.timeScale = 0f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(true);
                pausePanel.transform.SetAsLastSibling();
            }
        }

        public void ResumeGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            if (pausePanel != null)
            {
                pausePanel.SetActive(false);
            }
        }

        public void ShowWinScreen(int loopCount)
        {
            isPaused = false;
            if (pausePanel != null) pausePanel.SetActive(false);

            if (winPanel != null)
            {
                winPanel.SetActive(true);
                winPanel.transform.SetAsLastSibling();
                if (winLoopCountText != null)
                    winLoopCountText.text = $"You escaped in {loopCount} loops!\nYour past self saved you.";
            }
            else
            {
                Debug.LogError("UIManager: winPanel is null!");
            }
            Time.timeScale = 0f;
        }

        public void RestartGame()
        {
            isPaused = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void GoToMainMenu()
        {
            isPaused = false;
            Time.timeScale = 1f;
            SceneManager.LoadScene("MainMenu");
        }

        public void OpenGithubRepo()
        {
            Debug.Log("Opening GitHub Repo: " + GITHUB_URL);
            Application.OpenURL(GITHUB_URL);
        }

        public void QuitGame()
        {
            Debug.Log("Quitting application...");
            Application.Quit();
        }
    }
}
