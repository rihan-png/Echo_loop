using UnityEngine;

namespace EchoLoop.Echo
{
    public class EchoRecorder : MonoBehaviour
    {
        private EchoData currentRecording = new EchoData();
        private bool isRecording = false;

        public void StartRecording()
        {
            currentRecording.Clear();
            isRecording = true;
        }

        private void FixedUpdate()
        {
            if (isRecording)
            {
                currentRecording.AddPosition(transform.position);
            }
        }

        public EchoData GetRecording()
        {
            isRecording = false;
            // Create a copy so we don't accidentally clear it in the next loop
            EchoData copy = new EchoData();
            copy.recordedPositions.AddRange(currentRecording.recordedPositions);
            return copy;
        }
    }
}
