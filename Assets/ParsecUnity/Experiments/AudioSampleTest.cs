using UnityEngine;

public class AudioSampleTest : MonoBehaviour
{
    public int sampleCount = 960;
    public int samplerate = 44100;
    public float frequency = 440;
    public int position = 0;

    private void Awake()
    {
        AudioClip clip = AudioClip.Create("MySinusoid", sampleCount, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);        
        AudioSource source = GetComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.Play();
    }

    void OnAudioRead(float[] data)
    {
        Debug.Log(data.Length);

        int count = 0;
        while (count < data.Length)
        {
            data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate);
            position++;
            count++;
        }
    }

    void OnAudioSetPosition(int newPosition)
    {
        position = newPosition;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {

    }
}
