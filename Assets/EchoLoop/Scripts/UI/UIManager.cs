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
            if (winPanel != null) winPanel.SetActive(false);
            if (hudPanel != null) hudPanel.SetActive(true);

            // Wire win panel buttons at runtime
            Button restartBtn = GameObject.Find("RestartButton")?.GetComponent<Button>();
            Button menuBtn = GameObject.Find("MenuButton")?.GetComponent<Button>();
            if (restartBtn != null) restartBtn.onClick.AddListener(RestartGame);
            if (menuBtn != null) menuBtn.onClick.AddListener(GoToMainMenu);
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
