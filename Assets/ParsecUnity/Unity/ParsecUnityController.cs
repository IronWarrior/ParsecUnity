﻿using ParsecGaming;
using System;
using System.Collections;
using UnityEngine;

public class ParsecUnityController : MonoBehaviour
{
    public Parsec Parsec { get; private set; }

    // Auto-enabling debug mode until this is all stable.
    // Static variables are so sinful :(
    public static bool EnableDebug { private get; set; } = true;

    // TODO: Ideally the host and client code should be split into two separate modules.
    #region Host
    // TODO: This should contain something more abstract.
    public event Action<Parsec.ParsecGuest, UnityEngine.InputSystem.InputDevice> OnGuestConnected;
    public event Action<Parsec.ParsecGuest> OnGuestDisconnected;

    private ParsecHostInput hostInput;
    private Texture2D screenshot;

    /// <summary>
    /// Starts hosting a Parsec session with the associated session ID.
    /// Streaming will not being until <see cref="StartHostStreamer"/> is called.
    /// </summary>
    /// <param name="sessionId"></param>
    public void Host(string sessionId)
    {
        InitializeParsec();

        var status = Parsec.HostStart(Parsec.ParsecHostMode.HOST_GAME, sessionId);
        Log($"Starting host: {status}");

        hostInput = gameObject.AddComponent<ParsecHostInput>();
        hostInput.Initialize(Parsec);

        StartCoroutine(HostPollEvents());
    }

    /// <summary>
    /// Starts capturing the screen at the end of every frame, submitting
    /// it to Parsec.
    /// </summary>
    public void StartHostStreamer()
    {
        StartCoroutine(HostSubmitFrame());
    }

    private IEnumerator HostPollEvents()
    {
        while (true)
        {
            yield return null;

            while (Parsec.HostPollEvents(0u, out Parsec.ParsecHostEvent hostEvent))
            {
                if (hostEvent.type == Parsec.ParsecHostEventType.HOST_EVENT_GUEST_STATE_CHANGE)
                {
                    Parsec.ParsecGuestState state = hostEvent.guestStateChange.guest.state;

                    switch (state)
                    {
                        case Parsec.ParsecGuestState.GUEST_CONNECTED:
                            Log($"Guest ID {hostEvent.guestStateChange.guest.id} connected.");

                            var device = hostInput.AddGuest(hostEvent.guestStateChange.guest);
                            OnGuestConnected?.Invoke(hostEvent.guestStateChange.guest, device);
                            break;
                        case Parsec.ParsecGuestState.GUEST_DISCONNECTED:
                            Log($"Guest ID {hostEvent.guestStateChange.guest.id} disconnected.");

                            OnGuestDisconnected?.Invoke(hostEvent.guestStateChange.guest);
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private IEnumerator HostSubmitFrame()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();

            if (screenshot != null)
                Destroy(screenshot);

            screenshot = ScreenCapture.CaptureScreenshotAsTexture(1);
            UnityNative.UnitySubmitFrame(Parsec.GetPointer(), screenshot.GetNativeTexturePtr());
            GL.IssuePluginEvent(UnityNative.UnityGetRenderEventFunction(), 1);
        }
    }
    #endregion

    #region Client
    [SerializeField]
    ParsecGuestView parsecGuestViewPrefab;

    private ParsecFrameDecoder parsecFrameDecoder;
    private ParsecGuestView parsecGuestView;

    public void Connect(string sessionId, string userId)
    {
        InitializeParsec();

        Parsec.ParsecClientConfig defaultConfig = new Parsec.ParsecClientConfig()
        {
            mediaContainer = 0,
            protocol = 1,
            secret = "",
            pngCursor = false
        };

        var status = Parsec.ClientConnect(defaultConfig, sessionId, userId);
        Log($"Connecting client: {status}");

        gameObject.AddComponent<ParsecGuestInput>().Initialize(Parsec);
    }

    public void StartClientPollFrames()
    {
        parsecFrameDecoder = new ParsecFrameDecoder();
        parsecGuestView = Instantiate(parsecGuestViewPrefab);

        StartCoroutine(ClientPollFrame());
    }

    private IEnumerator ClientPollFrame()
    {
        while (true)
        {
            yield return null;
            Parsec.ClientPollFrame(0, OnClientReceiveFrame, 0u);
        }
    }

    private void OnClientReceiveFrame(Parsec.ParsecFrame frame, IntPtr image, IntPtr opaque)
    {
        parsecFrameDecoder.Decode(frame, image);
        // TODO: Could just initialize the guest view with the decoder?
        parsecGuestView.Populate(parsecFrameDecoder.Y, parsecFrameDecoder.U, parsecFrameDecoder.V, parsecFrameDecoder.Padding);
    }
    #endregion

    public static void Log(string text)
    {
        if (!EnableDebug)
            return;

        Debug.Log($"<b>Unity Parsec</b>: {text}");
    }

    private void InitializeParsec()
    {
        Parsec = new Parsec();

        var status = Parsec.Init();
        Log($"Parsec Init: {status}");
    }

    private void OnDestroy()
    {
        if (Parsec != null)
            Parsec.ParsecDestroy();
    }
}
