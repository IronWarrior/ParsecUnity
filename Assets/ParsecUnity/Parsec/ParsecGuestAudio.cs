using ParsecGaming;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using UnityEngine;

public class ParsecGuestAudio : MonoBehaviour
{
    private Parsec parsec;

    private Queue<short> pcmStream;
    private bool pollAudio;

    public void Initialize(Parsec parsec)
    {
        AudioClip clip = AudioClip.Create("ParsecGuestAudioStream", 960, 2, 48000, true);

        AudioSource source = gameObject.AddComponent<AudioSource>();
        source.clip = clip;
        source.loop = true;
        source.Play();

        pcmStream = new Queue<short>();

        this.parsec = parsec;

        new Thread(() => PollAudio()).Start();
    }

    private void OnDestroy()
    {
        pollAudio = false;
    }

    private void OnAudioFilterRead(float[] data, int channels)
    {
        // TODO: Is the lock necessary here?
        lock (pcmStream)
        {
            // Unity's DSP buffer size is 1024, but Parsec sends frames in blocks of 960.
            // (Both are two channel, with their buffers 2x the size).
            // So we wait until there are enough frames from Parsec to send them through
            // Unity's DSP.
            if (pcmStream.Count >= 1024 * 2)
            {
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] = (float)pcmStream.Dequeue() / short.MaxValue;
                }
            }
        }
    }

    private void PollAudio()
    {
        pollAudio = true;

        while (pollAudio)
        {
            parsec.ClientPollAudio(ClientReceiveAudio, 0u);
            Thread.Sleep(5);
        }
    }

    private void ClientReceiveAudio(IntPtr pcm, uint frames, IntPtr opaque)
    {
        // TODO: Probably don't need to allocate a new array each time audio comes in.
        short[] marshalledPCM = new short[frames * 2];
        Marshal.Copy(pcm, marshalledPCM, 0, (int)frames * 2);

        for (int i = 0; i < marshalledPCM.Length; i++)
        {
            pcmStream.Enqueue(marshalledPCM[i]);
        }
    }
}
