#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using EchoLoop.Puzzle;
using EchoLoop.Echo;
using EchoLoop.Loop;

namespace EchoLoop.Editor
{
    public class Milestone3Setup
    {
        [MenuItem("Echo Loop/Setup Milestone 3 Scene")]
        public static void RunSetup()
        {
            string prototypeScenePath = "Assets/EchoLoop/Scenes/Prototype.unity";
            if (!System.IO.File.Exists(prototypeScenePath))
            {
                Debug.LogError("Prototype scene not found. Run previous setups first.");
                return;
            }

            Scene prototypeScene = EditorSceneManager.OpenScene(prototypeScenePath);

            // 1. Tag the EchoClone prefab so the pressure plate detects it
            string prefabPath = "Assets/EchoLoop/Prefabs/EchoClone.prefab";
            GameObject echoPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefabPath);
            if (echoPrefab != null)
            {
                using (var editingScope = new PrefabUtility.EditPrefabContentsScope(prefabPath))
                {
                    GameObject prefabRoot = editingScope.prefabContentsRoot;
                    prefabRoot.tag = "Player";
                    
                    // The EchoClone needs a collider to trigger the pressure plate!
                    if (prefabRoot.GetComponent<Collider2D>() == null)
                    {
                        CapsuleCollider2D col = prefabRoot.AddComponent<CapsuleCollider2D>();
                        col.size = new Vector2(1, 1);
                        col.isTrigger = true; // Triggers plate without blocking physics
                    }
                    
                    // Rigidbody2D needed for triggers to fire reliably if moving kinematically
                    if (prefabRoot.GetComponent<Rigidbody2D>() == null)
                    {
                        Rigidbody2D rb = prefabRoot.AddComponent<Rigidbody2D>();
                        rb.bodyType = RigidbodyType2D.Kinematic;
                    }
                }
            }
            else
            {
                Debug.LogError("EchoClone prefab not found! Did you run Milestone 2 setup?");
                return;
            }

            // 2. Build the Door
            GameObject environment = GameObject.Find("Environment");
            if (environment == null)
            {
                environment = new GameObject("Environment");
            }

            GameObject door = GameObject.Find("Door");
            if (door == null)
            {
                door = new GameObject("Door");
                door.transform.SetParent(environment.transform);
                door.transform.position = new Vector3(8, -1f, 0);
                door.transform.localScale = new Vector3(1, 4, 1);
                
                SpriteRenderer dsr = door.AddComponent<SpriteRenderer>();
                dsr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                dsr.color = new Color(0.8f, 0.4f, 0.1f); // Orange-ish brown
                
                door.AddComponent<BoxCollider2D>(); // Blocks player
                door.AddComponent<Door>();
            }

            // 3. Build the Pressure Plate
            GameObject plate = GameObject.Find("PressurePlate");
            if (plate == null)
            {
                plate = new GameObject("PressurePlate");
                plate.transform.SetParent(environment.transform);
                // Position slightly above ground
                plate.transform.position = new Vector3(-2, -2.4f, 0); 
                plate.transform.localScale = new Vector3(1.5f, 0.2f, 1);
                
                SpriteRenderer psr = plate.AddComponent<SpriteRenderer>();
                psr.sprite = AssetDatabase.GetBuiltinExtraResource<Sprite>("UI/Skin/Background.psd");
                
                BoxCollider2D pcol = plate.AddComponent<BoxCollider2D>();
                pcol.isTrigger = true;
                
                PressurePlate pp = plate.AddComponent<PressurePlate>();
                pp.targetActivator = door.GetComponent<Door>();
            }
            else
            {
                PressurePlate pp = plate.GetComponent<PressurePlate>();
                if (pp != null)
                {
                    pp.targetActivator = door.GetComponent<Door>();
                }
            }

            // 4. Fix Ground and Spawn Point
            GameObject ground = GameObject.Find("Ground");
            if (ground != null)
            {
                Undo.RecordObject(ground.transform, "Widen Ground");
                ground.transform.localScale = new Vector3(20, 1, 1); // Make it very wide so we don't fall!
            }

            GameObject spawnPoint = GameObject.Find("SpawnPoint");
            if (spawnPoint != null)
            {
                Undo.RecordObject(spawnPoint.transform, "Move Spawn Point");
                spawnPoint.transform.position = new Vector3(-6, -1, 0);
                
                GameObject player = GameObject.Find("Player");
                if (player != null)
                {
                    Undo.RecordObject(player.transform, "Move Player");
                    player.transform.position = spawnPoint.transform.position;
                }
            }

            // 5. Bulletproof check for Echo Prefab
            GameObject gameManager = GameObject.Find("GameManager");
            if (gameManager != null && echoPrefab != null)
            {
                LoopManager lm = gameManager.GetComponent<LoopManager>();
                if (lm != null && lm.echoPrefab == null)
                {
                    Undo.RecordObject(lm, "Fix missing Echo Prefab");
                    lm.echoPrefab = echoPrefab;
                }
            }

            // Save Scene
            EditorSceneManager.SaveScene(prototypeScene);
            Debug.Log("Milestone 3 Setup Complete! Fixed ground width and ensured ghosts are working.");
        }
    }
}
#endif
