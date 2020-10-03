using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    GameObject menuContainer;

    [SerializeField]
    Button hostButton;

    [SerializeField]
    Button connectButton;

    [SerializeField]
    InputField sessionIdInput;

    [SerializeField]
    InputField peerIdInput;

    [SerializeField]
    Text statusText;

    [SerializeField]
    RGBPicker rgbPicker;

    [SerializeField]
    ParsecUnityController parsecController;

    [SerializeField]
    Game game;

    private void Awake()
    {
        hostButton.onClick.AddListener(Host);
        connectButton.onClick.AddListener(Connect);

        statusText.gameObject.SetActive(false);
    }

    private void Host()
    {
        parsecController.Host(sessionIdInput.text);
        parsecController.StartHostStreamer();

        game.Initialize(parsecController);
        game.AddPlayer(-1, new InputDeviceCollection(InputSystem.devices.ToArray()));
        game.SetPlayerColor(-1, rgbPicker.Color);

        Destroy(gameObject);
    }

    private void Connect()
    {
        StartCoroutine(ConnectRoutine());
    }

    private IEnumerator ConnectRoutine()
    {
        menuContainer.gameObject.SetActive(false);
        statusText.gameObject.SetActive(true);

        parsecController.Connect(sessionIdInput.text, peerIdInput.text);

        ParsecGaming.Parsec.ParsecStatus status;

        do
        {
            status = parsecController.Parsec.ClientGetStatus(out _);
            statusText.text = $"<b>Status: </b>{status}";
            yield return null;
        } 
        while (status != ParsecGaming.Parsec.ParsecStatus.PARSEC_OK);

        parsecController.ClientSendUserData(rgbPicker.Color);
        parsecController.StartClientPollFrames();

        Destroy(gameObject);
    }
}
