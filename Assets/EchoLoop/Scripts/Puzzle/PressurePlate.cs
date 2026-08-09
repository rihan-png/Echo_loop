using UnityEngine;

namespace EchoLoop.Puzzle
{
    public class PressurePlate : MonoBehaviour
    {
        public MonoBehaviour targetActivator; // Needs to implement IPuzzleActivator
        private IPuzzleActivator activatorInterface;

        private int objectsOnPlate = 0;
        private SpriteRenderer sr;
        public Color unpressedColor = Color.red;
        public Color pressedColor = Color.green;

        private void Start()
        {
            sr = GetComponent<SpriteRenderer>();
            if (sr != null) sr.color = unpressedColor;
            
            if (targetActivator != null)
            {
                activatorInterface = targetActivator as IPuzzleActivator;
                if (activatorInterface == null)
                {
                    Debug.LogError("PressurePlate target does not implement IPuzzleActivator!");
                }
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                objectsOnPlate++;
                UpdateState();
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                objectsOnPlate--;
                if (objectsOnPlate < 0) objectsOnPlate = 0;
                UpdateState();
            }
        }

        private void UpdateState()
        {
            if (objectsOnPlate > 0)
            {
                if (sr != null) sr.color = pressedColor;
                if (EchoLoop.Core.SoundManager.Instance != null)
                    EchoLoop.Core.SoundManager.Instance.PlayPlatePress();
                activatorInterface?.Activate();
            }
            else
            {
                if (sr != null) sr.color = unpressedColor;
                activatorInterface?.Deactivate();
            }
        }
    }
}
