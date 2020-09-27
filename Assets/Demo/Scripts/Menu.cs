using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    [SerializeField]
    Button hostButton;

    [SerializeField]
    Button connectButton;

    [SerializeField]
    InputField sessionIdInput;

    [SerializeField]
    InputField peerIdInput;

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
    }

    private void Host()
    {
        parsecController.Host(sessionIdInput.text);
        parsecController.StartHostStreamer();

        game.Initialize(parsecController);
        game.AddPlayer(-1, null);
        game.SetPlayerColor(-1, rgbPicker.Color);

        Destroy(gameObject);
    }

    private void Connect()
    {
        parsecController.Connect(sessionIdInput.text, peerIdInput.text);
        parsecController.StartClientPollFrames();

        Destroy(gameObject);
    }
}
