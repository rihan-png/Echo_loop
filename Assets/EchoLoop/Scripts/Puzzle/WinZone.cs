using UnityEngine;
using EchoLoop.Loop;

namespace EchoLoop.Puzzle
{
    public class WinZone : MonoBehaviour
    {
        private bool triggered = false;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (triggered) return;

            // Only trigger for the real player, not ghost clones
            if (!collision.CompareTag("Player")) return;
            if (collision.gameObject.GetComponent<EchoLoop.Echo.EchoPlayer>() != null) return;

            triggered = true;

            LoopManager lm = FindFirstObjectByType<LoopManager>();
            int loops = lm != null ? lm.CurrentLoop : 1;

            UI.UIManager uiManager = FindFirstObjectByType<UI.UIManager>();
            if (uiManager != null)
            {
                uiManager.ShowWinScreen(loops);
            }
            else
            {
                // Fallback: just pause and log
                Debug.Log($"YOU WIN! Escaped in {loops} loops! (UIManager not found)");
                Time.timeScale = 0f;
            }

            // Play win sound
            if (EchoLoop.Core.SoundManager.Instance != null)
                EchoLoop.Core.SoundManager.Instance.PlayWin();
        }
    }
}
