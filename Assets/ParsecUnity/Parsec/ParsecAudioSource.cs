using UnityEngine;

public class ParsecAudioSource : MonoBehaviour
{
    private void OnAudioFilterRead(float[] data, int channels)
    {
        ParsecTest.SubmitAudio(data);
    }
}
