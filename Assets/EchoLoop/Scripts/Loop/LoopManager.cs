using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using EchoLoop.Echo;

namespace EchoLoop.Loop
{
    public class LoopManager : MonoBehaviour
    {
        public float loopDuration = 10f;
        public Text timerText;
        public Text loopNumberText;
        public Transform playerSpawnPoint;
        public GameObject player;
        public GameObject echoPrefab;

        private float timeRemaining;
        private int currentLoop = 1;
        public int CurrentLoop => currentLoop;
        
        private EchoRecorder playerRecorder;
        private List<EchoData> pastLoopsData = new List<EchoData>();
        private List<EchoPlayer> activeEchoes = new List<EchoPlayer>();

        private void Start()
        {
            if (player == null)
            {
                player = GameObject.Find("Player");
                Debug.LogWarning("LoopManager: player was null. Found player automatically.");
            }
            
            if (player != null)
            {
                playerRecorder = player.GetComponent<EchoRecorder>();
                if (playerRecorder == null) Debug.LogError("LoopManager: Player is missing EchoRecorder!");
            }
            else
            {
                Debug.LogError("LoopManager: Could not find Player in scene!");
            }

            if (echoPrefab == null)
            {
                Debug.LogError("LoopManager: EchoPrefab is missing! Ghosts cannot spawn.");
            }

            ResetLoop(false);
        }

        private void Update()
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                ResetLoop(true);
            }
            else if (Keyboard.current != null && Keyboard.current.rKey.wasPressedThisFrame)
            {
                ResetLoop(true);
            }

            UpdateUI();
        }

        private void ResetLoop(bool incrementLoop)
        {
            if (EchoLoop.Core.SoundManager.Instance != null)
                EchoLoop.Core.SoundManager.Instance.PlayLoop();
            if (incrementLoop)
            {
                currentLoop++;
                if (playerRecorder != null)
                {
                    EchoData data = playerRecorder.GetRecording();
                    pastLoopsData.Add(data);
                    Debug.Log($"LoopManager: Loop {currentLoop} starting. Saved previous loop with {data.recordedPositions.Count} frames.");
                }
            }
            
            timeRemaining = loopDuration;

            // Reset Player
            if (player != null && playerSpawnPoint != null)
            {
                player.transform.position = playerSpawnPoint.position;
                Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector2.zero;
                }
                
                if (playerRecorder != null)
                {
                    playerRecorder.StartRecording();
                }
            }

            // Spawn and start Echoes
            SpawnAndStartEchoes();

            UpdateUI();
        }

        private void SpawnAndStartEchoes()
        {
            // Clean up old echoes
            foreach (var echo in activeEchoes)
            {
                if (echo != null)
                {
                    Destroy(echo.gameObject);
                }
            }
            activeEchoes.Clear();

            Debug.Log($"LoopManager: Spawning {pastLoopsData.Count} ghosts.");

            if (echoPrefab != null)
            {
                foreach (var data in pastLoopsData)
                {
                    GameObject echoObj = Instantiate(echoPrefab, playerSpawnPoint.position, Quaternion.identity);
                    EchoPlayer echoPlayer = echoObj.GetComponent<EchoPlayer>();
                    if (echoPlayer != null)
                    {
                        echoPlayer.Setup(data);
                        echoPlayer.StartPlayback();
                        activeEchoes.Add(echoPlayer);
                    }
                    else
                    {
                        Debug.LogError("LoopManager: The spawned EchoClone prefab is missing the EchoPlayer script!");
                    }
                }
            }
        }

        private void UpdateUI()
        {
            if (timerText != null)
            {
                timerText.text = $"Time: {Mathf.CeilToInt(timeRemaining)}s";
            }
            if (loopNumberText != null)
            {
                loopNumberText.text = $"Loop: {currentLoop}";
            }
        }
    }
}
