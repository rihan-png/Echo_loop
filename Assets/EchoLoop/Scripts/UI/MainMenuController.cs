using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;

namespace EchoLoop.UI
{
    public class MainMenuController : MonoBehaviour
    {
        private Text instructionText;
        private float tapTimer = 0.3f;

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
            // Wire standard UI buttons if they exist
            Button playBtn = GameObject.Find("PlayButton")?.GetComponent<Button>();
            Button quitBtn = GameObject.Find("QuitButton")?.GetComponent<Button>();

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
                    t.text = "TAP SCREEN OR PRESS PLAY";
                    t.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    t.fontSize = 26;
                    t.alignment = TextAnchor.MiddleCenter;
                    t.color = new Color(1f, 1f, 1f, 0.9f);
                    RectTransform r = tapObj.GetComponent<RectTransform>();
                    r.anchorMin = new Vector2(0.5f, 0.15f);
                    r.anchorMax = new Vector2(0.5f, 0.25f);
                    r.sizeDelta = new Vector2(600, 50);
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
                float alpha = 0.5f + Mathf.Abs(Mathf.Sin(Time.time * 3f)) * 0.5f;
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

            // Enhanced touch input (Reliable on all Android devices)
            if (UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches.Count > 0)
            {
                foreach (var touch in UnityEngine.InputSystem.EnhancedTouch.Touch.activeTouches)
                {
                    if (touch.phase == UnityEngine.InputSystem.TouchPhase.Began)
                    {
                        PlayGame();
                        return;
                    }
                }
            }

            // Standard Touchscreen input
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.wasPressedThisFrame)
            {
                PlayGame();
                return;
            }

            // Mouse click
            if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
            {
                PlayGame();
                return;
            }
        }

        public void PlayGame()
        {
            Debug.Log("MainMenuController: Loading Prototype...");
            Time.timeScale = 1f; // Ensure time scale is not paused
            SceneManager.LoadScene("Prototype");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }
}
