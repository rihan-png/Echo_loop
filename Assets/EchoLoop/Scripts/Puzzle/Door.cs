using UnityEngine;

namespace EchoLoop.Puzzle
{
    public class Door : MonoBehaviour, IPuzzleActivator
    {
        public float openHeight = 3.5f;
        public float speed = 5f;

        private Vector3 closedPosition;
        private Vector3 openPosition;
        private bool isOpen = false;

        private void Start()
        {
            closedPosition = transform.position;
            openPosition = closedPosition + Vector3.up * openHeight;
        }

        private void Update()
        {
            Vector3 target = isOpen ? openPosition : closedPosition;
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }

        public void Activate()
        {
            isOpen = true;
        }

        public void Deactivate()
        {
            isOpen = false;
        }
    }
}
