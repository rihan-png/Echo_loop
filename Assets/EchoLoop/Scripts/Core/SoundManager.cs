using UnityEngine;

namespace EchoLoop.Core
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager Instance;

        private AudioSource audioSource;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                audioSource = gameObject.AddComponent<AudioSource>();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private AudioClip GenerateTone(float frequency, float duration, float volume = 0.3f)
        {
            int sampleRate = 44100;
            int samples = Mathf.CeilToInt(sampleRate * duration);
            AudioClip clip = AudioClip.Create("tone", samples, 1, sampleRate, false);
            float[] data = new float[samples];
            for (int i = 0; i < samples; i++)
            {
                float t = (float)i / sampleRate;
                float fade = 1f - (t / duration); // fade out
                data[i] = Mathf.Sin(2 * Mathf.PI * frequency * t) * volume * fade;
            }
            clip.SetData(data, 0);
            return clip;
        }

        public void PlayJump()
        {
            audioSource.PlayOneShot(GenerateTone(440f, 0.15f, 0.2f));
        }

        public void PlayLoop()
        {
            audioSource.PlayOneShot(GenerateTone(300f, 0.4f, 0.25f));
        }

        public void PlayPlatePress()
        {
            audioSource.PlayOneShot(GenerateTone(600f, 0.1f, 0.2f));
        }

        public void PlayWin()
        {
            // Play a simple victory arpeggio
            StartCoroutine(PlayArpeggio());
        }

        private System.Collections.IEnumerator PlayArpeggio()
        {
            float[] notes = { 523f, 659f, 784f, 1047f };
            foreach (float note in notes)
            {
                audioSource.PlayOneShot(GenerateTone(note, 0.2f, 0.3f));
                yield return new WaitForSecondsRealtime(0.18f);
            }
        }
    }
}
