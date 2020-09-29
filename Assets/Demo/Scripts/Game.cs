using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class Game : MonoBehaviour
{
    [SerializeField]
    GameObject playerPrefab;

    private Dictionary<int, GameObject> playersById;

    private void Awake()
    {
        playersById = new Dictionary<int, GameObject>();
    }

    public void Initialize(ParsecUnityController parsecUnityController)
    {
        gameObject.SetActive(true);

        // Normally you'd likely want a module between Parsec and your Game management code to inject
        // players without knowing where they came from, but we'll be lazy here.
        parsecUnityController.OnGuestConnected += ParsecUnityController_OnGuestConnected;
        parsecUnityController.OnGuestDisconnected += ParsecUnityController_OnGuestDisconnected;
        parsecUnityController.OnReceiveUserData += ParsecUnityController_OnReceiveUserData;
    }

    public void AddPlayer(int id, InputDevice device)
    {
        GameObject player = Instantiate(playerPrefab, new Vector3(Random.Range(-3f, 3f), 0, Random.Range(-3f, 3f)), Quaternion.identity);
        playersById[id] = player;

        if (device != null)
            player.GetComponent<PlayerInput>().SwitchCurrentControlScheme(device);
    }

    public void SetPlayerColor(int id, Color color)
    {
        playersById[id].GetComponent<Player>().SetColor(color);
    }

    private void ParsecUnityController_OnGuestConnected(ParsecGaming.Parsec.ParsecGuest guest, InputDevice device)
    {
        AddPlayer((int)guest.id, device);
    }

    private void ParsecUnityController_OnGuestDisconnected(ParsecGaming.Parsec.ParsecGuest guest)
    {
        Destroy(playersById[(int)guest.id]);
    }

    private void ParsecUnityController_OnReceiveUserData(ParsecGaming.Parsec.ParsecGuest guest, string jsonColor)
    {
        Color color = JsonUtility.FromJson<Color>(jsonColor);
        SetPlayerColor((int)guest.id, color);
    }
}
