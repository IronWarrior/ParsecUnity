using UnityEngine;
using UnityEngine.InputSystem;
using ParsecGaming;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

// TODO: It's overall probably not recommended to directly inject inputs
// into the Unity Input System, as the app may require focus for the injected
// inputs to be run. It's nice that this is a fully seamless way to architect it,
// but this could be a critical problem for some apps.
public class ParsecHostInput : MonoBehaviour
{
    private Parsec parsec;

    public Dictionary<uint, Keyboard> guests;

    private void Awake()
    {
        guests = new Dictionary<uint, Keyboard>();
    }

    private void OnDestroy()
    {
        foreach (var kvp in guests)
        {
            InputSystem.RemoveDevice(kvp.Value);
        }
    }

    public void Initialize(Parsec parsec)
    {
        this.parsec = parsec;
    }

    public InputDevice AddGuest(Parsec.ParsecGuest guest)
    {
        Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
        guests[guest.id] = keyboard;

        return keyboard;
    }

    // TODO: This should be applied before all updates, or whenever
    // input normally is applied (based on the InputSystem configuration).
    // TODO: There's a bug where although a press and release are recorded,
    // sometimes the InputSystem does not correctly reset state.
    private void Update()
    {
        while (parsec.HostPollInput(0u, out Parsec.ParsecGuest guest, out Parsec.ParsecMessage msg))
        {
            switch (msg.type)
            {
                case Parsec.ParsecMessageType.MESSAGE_KEYBOARD:
                    if (ParsecInputSystemMapping.Keys.TryGetValue(msg.keyboard.code, out Key key))
                    {
                        Keyboard keyboard = guests[guest.id];

                        using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
                        {
                            ParsecUnityController.Log($"Guest {guest.id} {(msg.keyboard.pressed ? "pressed" : "released")} key {key} at {Time.time}");

                            keyboard[key].WriteValueIntoEvent<float>(msg.keyboard.pressed ? 1 : 0, eventPtr);
                            InputSystem.QueueEvent(eventPtr);
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}
