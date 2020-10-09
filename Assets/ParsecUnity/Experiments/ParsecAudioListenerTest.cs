using UnityEngine;

public class ParsecAudioListenerTest : MonoBehaviour
{
    private void OnAudioFilterRead(float[] data, int channels)
    {
        ParsecTest.SubmitAudio(data);
    }
}
