#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using EchoLoop.Player;
using EchoLoop.Loop;
using EchoLoop.Core;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;

namespace EchoLoop.Editor
{
    public class Milestone1Setup
    {
        [MenuItem("Echo Loop/Setup Milestone 1 Scene")]
        public static void RunSetup()
        {
            // 1. Setup Scene
            string prototypeScenePath = "Assets/EchoLoop/Scenes/Prototype.unity";
            Scene prototypeScene;
            if (System.IO.File.Exists(prototypeScenePath))
            {
                prototypeScene = EditorSceneManager.OpenScene(prototypeScenePath);
            }
            else
            {
                if (System.IO.File.Exists("Assets/Scenes/SampleScene.unity"))
                {
                    AssetDatabase.MoveAsset("Assets/Scenes/SampleScene.unity", prototypeScenePath);
                    prototypeScene = EditorSceneManager.OpenScene(prototypeScenePath);
                }
                else
                {
                    prototypeScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Single);
                    EditorSceneManager.SaveScene(prototypeScene, prototypeScenePath);
                }
            }

            // Clean scene (remove all except essential starting objects to build fresh)
            foreach (GameObject obj in prototypeScene.GetRootGameObjects())
            {
                if (obj.name != "Main Camera" && obj.name != "Directional Light")
                {
                    GameObject.DestroyImmediate(obj);
                }
            }

            // 2. Create Player
            GameObject player = new GameObject("Player");
            player.tag = "Player";
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            sr.color = Color.cyan;
            
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            rb.freezeRotation = true;
            
            CapsuleCollider2D col = player.AddComponent<CapsuleCollider2D>();
            col.size = new Vector2(1, 1);
            
            PlayerController pc = player.AddComponent<PlayerController>();
            
            GameObject groundCheck = new GameObject("GroundCheck");
            groundCheck.transform.SetParent(player.transform);
            groundCheck.transform.localPosition = new Vector3(0, -0.55f, 0);
            pc.groundCheck = groundCheck.transform;
            pc.groundLayer = LayerMask.GetMask("Default");

            PlayerInput pi = player.AddComponent<PlayerInput>();
            InputActionAsset actions = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
            pi.actions = actions;
            pi.defaultControlScheme = "Keyboard&Mouse";
            pi.defaultActionMap = "Player";
            pi.notificationBehavior = PlayerNotifications.SendMessages; 

            // 3. Test Level (Ground & Platforms)
            GameObject environment = new GameObject("Environment");
            
            GameObject ground = new GameObject("Ground");
            ground.transform.SetParent(environment.transform);
            ground.transform.position = new Vector3(0, -3, 0);
            ground.transform.localScale = new Vector3(20, 1, 1);
            SpriteRenderer gsr = ground.AddComponent<SpriteRenderer>();
            gsr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            gsr.color = Color.gray;
            ground.AddComponent<BoxCollider2D>();

            GameObject platform1 = new GameObject("Platform1");
            platform1.transform.SetParent(environment.transform);
            platform1.transform.position = new Vector3(3, -1, 0);
            platform1.transform.localScale = new Vector3(3, 0.5f, 1);
            SpriteRenderer psr1 = platform1.AddComponent<SpriteRenderer>();
            psr1.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
            psr1.color = Color.gray;
            platform1.AddComponent<BoxCollider2D>();

            GameObject spawnPoint = new GameObject("SpawnPoint");
            spawnPoint.transform.position = new Vector3(-5, -1, 0);
            player.transform.position = spawnPoint.transform.position;

            // 4. Loop Manager
            GameObject gameManager = new GameObject("GameManager");
            LoopManager lm = gameManager.AddComponent<LoopManager>();
            lm.player = player;
            lm.playerSpawnPoint = spawnPoint.transform;

            // 5. UI
            GameObject canvasObj = new GameObject("Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();

            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<InputSystemUIInputModule>();

            GameObject timerTextObj = new GameObject("TimerText");
            timerTextObj.transform.SetParent(canvasObj.transform, false);
            Text timerText = timerTextObj.AddComponent<Text>();
            timerText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            timerText.fontSize = 32;
            timerText.color = Color.white;
            timerText.alignment = TextAnchor.UpperRight;
            RectTransform timerRT = timerText.GetComponent<RectTransform>();
            timerRT.anchorMin = new Vector2(1, 1);
            timerRT.anchorMax = new Vector2(1, 1);
            timerRT.pivot = new Vector2(1, 1);
            timerRT.anchoredPosition = new Vector2(-20, -20);
            timerRT.sizeDelta = new Vector2(200, 50);
            lm.timerText = timerText;

            GameObject loopTextObj = new GameObject("LoopText");
            loopTextObj.transform.SetParent(canvasObj.transform, false);
            Text loopText = loopTextObj.AddComponent<Text>();
            loopText.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            loopText.fontSize = 32;
            loopText.color = Color.white;
            loopText.alignment = TextAnchor.UpperLeft;
            RectTransform loopRT = loopText.GetComponent<RectTransform>();
            loopRT.anchorMin = new Vector2(0, 1);
            loopRT.anchorMax = new Vector2(0, 1);
            loopRT.pivot = new Vector2(0, 1);
            loopRT.anchoredPosition = new Vector2(20, -20);
            loopRT.sizeDelta = new Vector2(200, 50);
            lm.loopNumberText = loopText;

            // 6. Main Camera Setup
            GameObject mainCamera = GameObject.Find("Main Camera");
            if (mainCamera != null)
            {
                mainCamera.transform.position = new Vector3(player.transform.position.x, player.transform.position.y, -10);
                CameraFollow cf = mainCamera.AddComponent<CameraFollow>();
                cf.target = player.transform;
            }

            // Save Scene
            EditorSceneManager.SaveScene(prototypeScene);
            Debug.Log("Milestone 1 Setup Complete! The Prototype scene is now configured.");
        }
    }
}
#endif
