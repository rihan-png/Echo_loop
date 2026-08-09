#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using EchoLoop.Player;
using EchoLoop.Loop;
using EchoLoop.Core;
using EchoLoop.Puzzle;
using EchoLoop.Echo;
using EchoLoop.UI;

namespace EchoLoop.Editor
{
    public class FinalBuildSetup
    {
        [MenuItem("Echo Loop/FINAL BUILD - Setup Everything")]
        public static void RunFinalSetup()
        {
            BuildMainMenuScene();
            BuildGameScene();
            SetupBuildSettings();
            Debug.Log("=== FINAL BUILD COMPLETE! Press Play on MainMenu scene to test the full game! ===");
        }

        // ─────────────────────────────────────────────
        // MAIN MENU SCENE
        // ─────────────────────────────────────────────
        static void BuildMainMenuScene()
        {
            var menuScene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

            // Camera
            GameObject camObj = new GameObject("MainCamera");
            Camera cam = camObj.AddComponent<Camera>();
            cam.backgroundColor = new Color(0.05f, 0.05f, 0.15f);
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.orthographic = true;
            cam.orthographicSize = 5;
            camObj.tag = "MainCamera";

            // Canvas
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();

            // Title
            GameObject titleObj = new GameObject("Title");
            titleObj.transform.SetParent(canvasObj.transform, false);
            Text titleText = titleObj.AddComponent<Text>();
            titleText.text = "ECHO LOOP";
            titleText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleText.fontSize = 72;
            titleText.fontStyle = FontStyle.Bold;
            titleText.alignment = TextAnchor.MiddleCenter;
            titleText.color = new Color(0.4f, 0.9f, 1f);
            RectTransform titleRect = titleObj.GetComponent<RectTransform>();
            titleRect.anchorMin = new Vector2(0.5f, 0.7f);
            titleRect.anchorMax = new Vector2(0.5f, 0.9f);
            titleRect.sizeDelta = new Vector2(600, 100);
            titleRect.anchoredPosition = Vector2.zero;

            // Subtitle
            GameObject subObj = new GameObject("Subtitle");
            subObj.transform.SetParent(canvasObj.transform, false);
            Text subText = subObj.AddComponent<Text>();
            subText.text = "Use your past self to escape the loop.";
            subText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            subText.fontSize = 24;
            subText.alignment = TextAnchor.MiddleCenter;
            subText.color = new Color(0.8f, 0.8f, 0.8f);
            RectTransform subRect = subObj.GetComponent<RectTransform>();
            subRect.anchorMin = new Vector2(0.5f, 0.55f);
            subRect.anchorMax = new Vector2(0.5f, 0.7f);
            subRect.sizeDelta = new Vector2(600, 60);
            subRect.anchoredPosition = Vector2.zero;

            // Play Button
            GameObject playBtnObj = CreateButton(canvasObj, "PlayButton", "PLAY", new Vector2(0, -50), new Vector2(220, 60));
            UnityEngine.UI.Button playBtn = playBtnObj.GetComponent<UnityEngine.UI.Button>();
            playBtnObj.GetComponentInChildren<Text>().color = Color.white;
            
            // Quit Button
            GameObject quitBtnObj = CreateButton(canvasObj, "QuitButton", "QUIT", new Vector2(0, -130), new Vector2(220, 60));
            quitBtnObj.GetComponentInChildren<Text>().color = Color.white;

            // MainMenuController
            GameObject menuController = new GameObject("MenuController");
            MainMenuController mmc = menuController.AddComponent<MainMenuController>();

            // Wire buttons
            playBtn.onClick.AddListener(mmc.PlayGame);
            UnityEngine.UI.Button quitBtn = quitBtnObj.GetComponent<UnityEngine.UI.Button>();
            quitBtn.onClick.AddListener(mmc.QuitGame);

            // Add EventSystem with NEW Input System module
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<UnityEngine.EventSystems.EventSystem>();
            eventSystem.AddComponent<UnityEngine.InputSystem.UI.InputSystemUIInputModule>();

            // Save MainMenu scene
            string scenePath = "Assets/EchoLoop/Scenes/MainMenu.unity";
            EditorSceneManager.SaveScene(menuScene, scenePath);
            Debug.Log("Main Menu scene built and saved!");
        }

        // ─────────────────────────────────────────────
        // GAME SCENE - FULL LEVEL
        // ─────────────────────────────────────────────
        static void BuildGameScene()
        {
            string protoPath = "Assets/EchoLoop/Scenes/Prototype.unity";
            var scene = EditorSceneManager.OpenScene(protoPath);

            // Clean up old puzzle objects so we rebuild fresh
            DestroyIfExists("Door");
            DestroyIfExists("PressurePlate");
            DestroyIfExists("Door2");
            DestroyIfExists("PressurePlate2");
            DestroyIfExists("WinZone");
            DestroyIfExists("SoundManager");

            // ── Ground ──
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                ground.transform.position = new Vector3(7, -3, 0);
                ground.transform.localScale = new Vector3(36, 1, 1);
                ground.GetComponent<SpriteRenderer>().color = new Color(0.25f, 0.25f, 0.35f);
            }

            // ── Spawn Point ──
            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null) spawnPoint.transform.position = new Vector3(-10, -1, 0);

            // ── Player ──
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                player.transform.position = new Vector3(-10, -1, 0);
                SpriteRenderer psr = player.GetComponent<SpriteRenderer>();
                if (psr != null) psr.color = new Color(0.2f, 0.8f, 1f);
            }

            // ── Camera ──
            GameObject camObj = GameObject.Find("Main Camera");
            if (camObj == null) camObj = GameObject.FindObjectOfType<Camera>()?.gameObject;
            if (camObj != null)
            {
                Camera cam = camObj.GetComponent<Camera>();
                if (cam != null) cam.backgroundColor = new Color(0.05f, 0.05f, 0.15f);
            }

            // ── Platforms ──
            CreatePlatform("Platform1", new Vector3(-5, -2, 0), new Vector3(4, 0.4f, 1), new Color(0.3f, 0.3f, 0.4f));
            CreatePlatform("Platform2", new Vector3(2, -1.5f, 0), new Vector3(3, 0.4f, 1), new Color(0.3f, 0.3f, 0.4f));
            CreatePlatform("Platform3", new Vector3(10, -1, 0), new Vector3(4, 0.4f, 1), new Color(0.3f, 0.3f, 0.4f));
            CreatePlatform("Platform4", new Vector3(18, -1.5f, 0), new Vector3(4, 0.4f, 1), new Color(0.3f, 0.3f, 0.4f));

            // ── PUZZLE 1: Plate at -7, Door at 0 ──
            GameObject door1 = CreateDoor("Door", new Vector3(0f, -1.5f, 0), new Color(0.9f, 0.4f, 0.1f));
            CreatePlate("PressurePlate", new Vector3(-7f, -2.65f, 0), door1);

            // ── PUZZLE 2: Plate at 7, Door at 14 ──
            GameObject door2 = CreateDoor("Door2", new Vector3(14f, -1.5f, 0), new Color(0.2f, 0.7f, 0.4f));
            CreatePlate("PressurePlate2", new Vector3(7f, -2.65f, 0), door2);

            // ── WIN ZONE ──
            GameObject winZoneObj = new GameObject("WinZone");
            winZoneObj.transform.position = new Vector3(23, -2, 0);
            winZoneObj.transform.localScale = new Vector3(2, 3, 1);
            SpriteRenderer wsr = winZoneObj.AddComponent<SpriteRenderer>();
            wsr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            wsr.color = new Color(1f, 0.9f, 0.1f, 0.8f);
            BoxCollider2D wCol = winZoneObj.AddComponent<BoxCollider2D>();
            wCol.isTrigger = true;
            winZoneObj.AddComponent<WinZone>();

            // ── SOUND MANAGER ──
            GameObject soundObj = new GameObject("SoundManager");
            soundObj.AddComponent<SoundManager>();

            // ── WIN SCREEN UI ──
            BuildWinScreenUI();

            // ── Fix EchoClone prefab tag & collider ──
            string prefabPath = "Assets/EchoLoop/Prefabs/EchoClone.prefab";
            GameObject echoPrefabAsset = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (echoPrefabAsset != null)
            {
                using (var scope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
                {
                    var root = scope.prefabContentsRoot;
                    root.tag = "Player";
                    if (root.GetComponent<Collider2D>() == null)
                    {
                        var col = root.AddComponent<CapsuleCollider2D>();
                        col.size = new Vector2(0.8f, 0.8f);
                        col.isTrigger = true;
                    }
                    if (root.GetComponent<Rigidbody2D>() == null)
                    {
                        var rb = root.AddComponent<Rigidbody2D>();
                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }
                }

                // Wire LoopManager
                GameObject gm = GameObject.Find("GameManager");
                if (gm != null)
                {
                    LoopManager lm = gm.GetComponent<LoopManager>();
                    if (lm != null)
                    {
                        Undo.RecordObject(lm, "Wire echo prefab");
                        lm.echoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
                        EditorUtility.SetDirty(lm);
                    }
                }
            }

            EditorSceneManager.SaveScene(scene);
            Debug.Log("Game scene fully rebuilt!");
        }

        static void BuildWinScreenUI()
        {
            // Destroy any old UIManager
            UIManager existingUI = GameObject.FindFirstObjectByType<UIManager>();
            if (existingUI != null) Object.DestroyImmediate(existingUI.gameObject);

            // Create a dedicated UIManager object (NOT on Canvas)
            GameObject uiManagerObj = new GameObject("UIManager");
            UIManager uiMgr = uiManagerObj.AddComponent<UIManager>();

            GameObject canvas = GameObject.Find("Canvas");
            if (canvas == null) return;

            // Win Panel
            GameObject winPanel = new GameObject("WinPanel");
            winPanel.transform.SetParent(canvas.transform, false);
            UnityEngine.UI.Image bg = winPanel.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0, 0, 0, 0.85f);
            RectTransform wpRect = winPanel.GetComponent<RectTransform>();
            wpRect.anchorMin = Vector2.zero;
            wpRect.anchorMax = Vector2.one;
            wpRect.sizeDelta = Vector2.zero;
            winPanel.SetActive(false);

            // Win title text
            GameObject winTitle = new GameObject("WinTitle");
            winTitle.transform.SetParent(winPanel.transform, false);
            Text wt = winTitle.AddComponent<Text>();
            wt.text = "YOU ESCAPED!";
            wt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            wt.fontSize = 64;
            wt.fontStyle = FontStyle.Bold;
            wt.alignment = TextAnchor.MiddleCenter;
            wt.color = new Color(1f, 0.9f, 0.1f);
            RectTransform wtRect = winTitle.GetComponent<RectTransform>();
            wtRect.anchorMin = new Vector2(0.5f, 0.65f);
            wtRect.anchorMax = new Vector2(0.5f, 0.85f);
            wtRect.sizeDelta = new Vector2(700, 100);
            wtRect.anchoredPosition = Vector2.zero;

            // Loop count text
            GameObject loopCountObj = new GameObject("WinLoopCount");
            loopCountObj.transform.SetParent(winPanel.transform, false);
            Text lct = loopCountObj.AddComponent<Text>();
            lct.text = "You escaped in X loops!";
            lct.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            lct.fontSize = 28;
            lct.alignment = TextAnchor.MiddleCenter;
            lct.color = Color.white;
            RectTransform lctRect = loopCountObj.GetComponent<RectTransform>();
            lctRect.anchorMin = new Vector2(0.5f, 0.5f);
            lctRect.anchorMax = new Vector2(0.5f, 0.65f);
            lctRect.sizeDelta = new Vector2(700, 80);
            lctRect.anchoredPosition = Vector2.zero;

            uiMgr.winPanel = winPanel;
            uiMgr.winLoopCountText = lct;

            // Restart Button
            GameObject restartBtn = CreateButton(winPanel, "RestartButton", "PLAY AGAIN", new Vector2(0, -60), new Vector2(200, 55));
            GameObject menuBtn = CreateButton(winPanel, "MenuButton", "MAIN MENU", new Vector2(0, -130), new Vector2(200, 55));

            UnityEngine.UI.Button rBtn = restartBtn.GetComponent<UnityEngine.UI.Button>();
            UnityEngine.UI.Button mBtn = menuBtn.GetComponent<UnityEngine.UI.Button>();
            rBtn.onClick.AddListener(uiMgr.RestartGame);
            mBtn.onClick.AddListener(uiMgr.GoToMainMenu);
        }

        // ─────────────────────────────────────────────
        // BUILD SETTINGS
        // ─────────────────────────────────────────────
        static void SetupBuildSettings()
        {
            var scenes = new[]
            {
                new EditorBuildSettingsScene("Assets/EchoLoop/Scenes/MainMenu.unity", true),
                new EditorBuildSettingsScene("Assets/EchoLoop/Scenes/Prototype.unity", true),
            };
            EditorBuildSettings.scenes = scenes;
            Debug.Log("Build settings updated: MainMenu → Prototype");
        }

        // ─────────────────────────────────────────────
        // HELPERS
        // ─────────────────────────────────────────────
        static GameObject CreateDoor(string name, Vector3 position, Color color)
        {
            GameObject door = new GameObject(name);
            door.transform.position = position;
            door.transform.localScale = new Vector3(1.2f, 5f, 1);
            SpriteRenderer sr = door.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = color;
            door.AddComponent<BoxCollider2D>();
            door.AddComponent<Door>();
            return door;
        }

        static void CreatePlate(string name, Vector3 position, GameObject door)
        {
            GameObject plate = new GameObject(name);
            plate.transform.position = position;
            plate.transform.localScale = new Vector3(2f, 0.3f, 1);
            SpriteRenderer sr = plate.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = Color.red;
            BoxCollider2D col = plate.AddComponent<BoxCollider2D>();
            col.isTrigger = true;
            PressurePlate pp = plate.AddComponent<PressurePlate>();
            pp.targetActivator = door.GetComponent<Door>();
        }

        static void CreatePlatform(string name, Vector3 position, Vector3 scale, Color color)
        {
            GameObject existing = GameObject.Find(name);
            if (existing != null) Object.DestroyImmediate(existing);

            GameObject platform = new GameObject(name);
            platform.transform.position = position;
            platform.transform.localScale = scale;
            SpriteRenderer sr = platform.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            sr.color = color;
            platform.AddComponent<BoxCollider2D>();
        }

        static GameObject CreateButton(GameObject parent, string name, string label, Vector2 anchoredPos, Vector2 size)
        {
            GameObject btnObj = new GameObject(name);
            btnObj.transform.SetParent(parent.transform, false);
            UnityEngine.UI.Image img = btnObj.AddComponent<UnityEngine.UI.Image>();
            img.color = new Color(0.15f, 0.5f, 0.9f);
            UnityEngine.UI.Button btn = btnObj.AddComponent<UnityEngine.UI.Button>();
            RectTransform rect = btnObj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.sizeDelta = size;
            rect.anchoredPosition = anchoredPos;

            GameObject textObj = new GameObject("Text");
            textObj.transform.SetParent(btnObj.transform, false);
            Text txt = textObj.AddComponent<Text>();
            txt.text = label;
            txt.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            txt.fontSize = 22;
            txt.fontStyle = FontStyle.Bold;
            txt.alignment = TextAnchor.MiddleCenter;
            txt.color = Color.white;
            RectTransform tRect = textObj.GetComponent<RectTransform>();
            tRect.anchorMin = Vector2.zero;
            tRect.anchorMax = Vector2.one;
            tRect.sizeDelta = Vector2.zero;

            return btnObj;
        }

        static void DestroyIfExists(string name)
        {
            GameObject obj = GameObject.Find(name);
            if (obj != null) Object.DestroyImmediate(obj);
        }
    }
}
#endif
