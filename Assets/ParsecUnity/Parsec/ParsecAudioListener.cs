using ParsecGaming;
using UnityEngine;

public class ParsecAudioListener : MonoBehaviour
{
    private Parsec parsec;

    private uint sampleRate;

    public void Initialize(Parsec parsec)
    {
        this.parsec = parsec;

        // Necessary to cache this, as it is not available on the OnAudioFilterRead
        // thread.
        sampleRate = (uint)AudioSettings.outputSampleRate;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        uint frames = (uint)(data.Length / 2);
        parsec.HostSubmitAudio(sampleRate, data, frames);
    }
}
