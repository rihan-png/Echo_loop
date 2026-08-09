using UnityEngine;

namespace EchoLoop.Echo
{
    public class EchoPlayer : MonoBehaviour
    {
        private EchoData dataToPlay;
        private int currentFrame = 0;
        private bool isPlaying = false;

        public void Setup(EchoData data)
        {
            dataToPlay = data;
        }

        public void StartPlayback()
        {
            currentFrame = 0;
            isPlaying = true;
            if (dataToPlay != null && dataToPlay.recordedPositions.Count > 0)
            {
                transform.position = dataToPlay.recordedPositions[0];
            }
        }

        private void FixedUpdate()
        {
            if (isPlaying && dataToPlay != null)
            {
                if (currentFrame < dataToPlay.recordedPositions.Count)
                {
                    transform.position = dataToPlay.recordedPositions[currentFrame];
                    currentFrame++;
                }
                else
                {
                    // Loop ended or out of bounds, stop playing
                    isPlaying = false;
                }
            }
        }
    }
}
