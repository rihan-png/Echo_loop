#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using EchoLoop.Echo;
using EchoLoop.Loop;

namespace EchoLoop.Editor
{
    public class Milestone2Setup
    {
        [MenuItem("Echo Loop/Setup Milestone 2 Scene")]
        public static void RunSetup()
        {
            string prototypeScenePath = "Assets/EchoLoop/Scenes/Prototype.unity";
            if (!System.IO.File.Exists(prototypeScenePath))
            {
                Debug.LogError("Prototype scene not found. Please run Milestone 1 setup first.");
                return;
            }

            Scene prototypeScene = EditorSceneManager.OpenScene(prototypeScenePath);

            // 1. Setup Echo Prefab
            GameObject echoPrefabObj = new GameObject("EchoPlayer");
            SpriteRenderer sr = echoPrefabObj.AddComponent<SpriteRenderer>();
            sr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Knob.psd");
            // Transparent cyan color for the ghost
            sr.color = new Color(0, 1f, 1f, 0.4f);
            
            echoPrefabObj.AddComponent<EchoPlayer>();
            sr.sortingOrder = -1; // Draw behind player

            // Save Prefab
            string prefabPath = "Assets/EchoLoop/Prefabs/EchoClone.prefab";
            GameObject savedPrefab = PrefabUtility.SaveAsPrefabAsset(echoPrefabObj, prefabPath);
            GameObject.DestroyImmediate(echoPrefabObj); // Remove from scene

            // 2. Attach EchoRecorder to Player
            GameObject player = GameObject.Find("Player");
            if (player != null)
            {
                if (player.GetComponent<EchoRecorder>() == null)
                {
                    Undo.AddComponent<EchoRecorder>(player);
                }
            }
            else
            {
                Debug.LogError("Player not found in scene!");
            }

            // 3. Update LoopManager
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null)
            {
                LoopManager lm = gameManager.GetComponent<LoopManager>();
                if (lm != null)
                {
                    Undo.RecordObject(lm, "Assign Echo Prefab");
                    lm.echoPrefab = savedPrefab;
                    EditorUtility.SetDirty(lm);
                }
            }
            else
            {
                Debug.LogError("GameManager not found in scene!");
            }

            // Save Scene
            EditorSceneManager.SaveScene(prototypeScene);
            Debug.Log("Milestone 2 Setup Complete! The Echo System is configured. Press Play and press R to see clones!");
        }
    }
}
#endif
