using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace EchoLoop.UI
{
    public class MainMenuController : MonoBehaviour
    {
        public const string GITHUB_URL = "https://github.com/rihan-png/Echo_loop";

        private Text instructionText;
        private float tapTimer = 0.4f;

        private void OnEnable()
        {
            EnhancedTouchSupport.Enable();
        }

        private void OnDisable()
        {
            EnhancedTouchSupport.Disable();
        }

        private void Start()
        {
            Time.timeScale = 1f;

            // Wire UI buttons
            Button playBtn = GameObject.Find("PlayButton")?.GetComponent<Button>();
            Button quitBtn = GameObject.Find("QuitButton")?.GetComponent<Button>();
            Button gitBtn = GameObject.Find("GithubButton")?.GetComponent<Button>();

            if (playBtn != null)
            {
                playBtn.onClick.RemoveAllListeners();
                playBtn.onClick.AddListener(PlayGame);
            }

            if (quitBtn != null)
            {
                quitBtn.onClick.RemoveAllListeners();
                quitBtn.onClick.AddListener(QuitGame);
            }

            if (gitBtn != null)
            {
                gitBtn.onClick.RemoveAllListeners();
                gitBtn.onClick.AddListener(OpenGithubDownload);
            }

            // Create or find TAP ANYWHERE text
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                GameObject tapObj = GameObject.Find("TapToPlay");
                if (tapObj == null)
                {
                    tapObj = new GameObject("TapToPlay");
                    tapObj.transform.SetParent(canvas.transform, false);
                    Text t = tapObj.AddComponent<Text>();
                    t.text = "TAP TO PLAY";
                    t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    t.fontSize = 24;
                    t.alignment = TextAnchor.MiddleCenter;
                    t.color = new Color(1f, 1f, 1f, 0.9f);
                    RectTransform r = tapObj.GetComponent<RectTransform>();
                    r.anchorMin = new Vector2(0.5f, 0.18f);
                    r.anchorMax = new Vector2(0.5f, 0.26f);
                    r.sizeDelta = new Vector2(500, 40);
                    r.anchoredPosition = Vector2.zero;
                    instructionText = t;
                }
                else
                {
                    instructionText = tapObj.GetComponent<Text>();
                }
            }
        }

        private void Update()
        {
            if (tapTimer > 0f)
            {
                tapTimer -= Time.deltaTime;
                return;
            }

            // Pulse text
            if (instructionText != null)
            {
                float alpha = 0.4f + Mathf.Abs(Mathf.Sin(Time.time * 3f)) * 0.6f;
                Color c = instructionText.color;
                c.a = alpha;
                instructionText.color = c;
            }

            // Keyboard input
            if (Keyboard.current != null &&
                (Keyboard.current.spaceKey.wasPressedThisFrame ||
                 Keyboard.current.enterKey.wasPressedThisFrame))
            {
                PlayGame();
                return;
            }
        }

        public void PlayGame()
        {
            Debug.Log("MainMenuController: Loading Prototype...");
            Time.timeScale = 1f;
            SceneManager.LoadScene("Prototype");
        }

        public void OpenGithubDownload()
        {
            Debug.Log("Opening GitHub download page: " + GITHUB_URL);
            Application.OpenURL(GITHUB_URL);
        }

        public void QuitGame()
        {
            Debug.Log("Exiting Game...");
            Application.Quit();
        }
    }
}
