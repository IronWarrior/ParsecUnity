﻿using ParsecGaming;
using System;
using System.Collections;
using UnityEngine;

public class ParsecUnityController : MonoBehaviour
{
    /// <summary>
    /// NOTE: Ideally this should be accessed publicly as little as possible, with
    /// all relevant data and operations encapsulated to the consumer.
    /// </summary>
    public Parsec Parsec { get; private set; }

    // Manually toggling this on and off until better debugging tools are made.
    // Static variables are so sinful :(
    public static bool EnableDebug { private get; set; } = true;

    // TODO: Ideally the host and client code should be split into two separate modules.
    #region Host
    // TODO: This should contain something more abstract.
    public event Action<Parsec.ParsecGuest, InputDeviceCollection> OnGuestConnected;
    public event Action<Parsec.ParsecGuest> OnGuestDisconnected;
    // TODO: This is currently only used for the client to send their color on game start.
    // Autoconverts in the incoming UTF-8 string into JSON, but does not encode anything about
    // its type (its assumed to be a color right now).
    public event Action<Parsec.ParsecGuest, string> OnReceiveUserData;

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

    public void StartHostAudio(AudioListener audioListener)
    {
        audioListener.gameObject.AddComponent<ParsecAudioListener>().Initialize(Parsec);
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

                            var deviceCollection = hostInput.AddGuest(hostEvent.guestStateChange.guest);
                            OnGuestConnected?.Invoke(hostEvent.guestStateChange.guest, deviceCollection);
                            break;
                        case Parsec.ParsecGuestState.GUEST_DISCONNECTED:
                            Log($"Guest ID {hostEvent.guestStateChange.guest.id} disconnected.");

                            OnGuestDisconnected?.Invoke(hostEvent.guestStateChange.guest);
                            break;
                        default:
                            break;
                    }
                }
                else if (hostEvent.type == Parsec.ParsecHostEventType.HOST_EVENT_USER_DATA)
                {
                    Log($"Received user data from {hostEvent.userData.guest.id}.");

                    string json = Parsec.HostGetUserDataString(hostEvent.userData.key);
                    OnReceiveUserData?.Invoke(hostEvent.userData.guest, json);
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

        // TODO: This should probably start a coroutine to monitor the connection attempt,
        // firing appropriate events on status change/success/failure.
    }

    /// <summary>
    /// Instantiates a client view and begins polling for frames to populate it.
    /// </summary>
    public void StartClientPollFrames()
    {
        parsecFrameDecoder = new ParsecFrameDecoder();
        parsecGuestView = Instantiate(parsecGuestViewPrefab);

        StartCoroutine(ClientMonitorScreenSize());
        StartCoroutine(ClientPollFrame());
    }

    /// <summary>
    /// Starts polling for incoming audio, feeding it into an audio source.
    /// </summary>
    public void StartClientPollAudio()
    {
        gameObject.AddComponent<ParsecGuestAudio>().Initialize(Parsec);
    }

    public void ClientSendUserData(Color color)
    {
        string json = JsonUtility.ToJson(color);

        Parsec.ClientSendUserData(0, json);
    }

    private int currentScreenWidth, currentScreenHeight;

    private IEnumerator ClientMonitorScreenSize()
    {
        while (true)
        {
            yield return null;

            if (Screen.width != currentScreenWidth || Screen.height != currentScreenHeight)
            {
                currentScreenWidth = Screen.width;
                currentScreenHeight = Screen.height;

                Parsec.ClientSetDimensions(0, (uint)currentScreenWidth, (uint)currentScreenHeight, 1);
            }
        }
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
        parsecGuestView.Populate(parsecFrameDecoder.Y, parsecFrameDecoder.U, parsecFrameDecoder.V, parsecFrameDecoder.Padding, parsecFrameDecoder.AspectRatio);
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
