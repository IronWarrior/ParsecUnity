using UnityEngine;
using UnityEngine.InputSystem;
using ParsecGaming;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

// TODO: It's overall probably not recommended to directly inject inputs
// into the Unity Input System, as the app may require focus for the injected
// inputs to be run. It's nice that this is a fully seamless way to architect it,
// but this could be a critical problem for some apps.
// TODO: The Input System-specific code should be moved to a different module. This
// class would then only be responsible for polymorphically converting Parsec inputs
// to the module's domain, and surfacing the converted inputs to the app.
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
        var keyboardMessages = new Dictionary<uint, List<Parsec.ParsecMessage>>();

        while (parsec.HostPollInput(0u, out Parsec.ParsecGuest guest, out Parsec.ParsecMessage msg))
        {
            switch (msg.type)
            {
                case Parsec.ParsecMessageType.MESSAGE_KEYBOARD:
                    if (!keyboardMessages.ContainsKey(guest.id))
                        keyboardMessages[guest.id] = new List<Parsec.ParsecMessage>();

                    keyboardMessages[guest.id].Add(msg);
                    break;
                case Parsec.ParsecMessageType.MESSAGE_MOUSE_BUTTON:
                    ParsecUnityController.Log($"Guest {guest.id} {(msg.mouseButton.pressed ? "pressed" : "released")} mouse button {msg.mouseButton} at {Time.time}");
                    break;
                case Parsec.ParsecMessageType.MESSAGE_GAMEPAD_BUTTON:
                    ParsecUnityController.Log($"Guest {guest.id} {(msg.gamepadButton.pressed ? "pressed" : "released")} gamepad button {msg.gamepadButton.button} on gamepad {msg.gamepadButton.id} at {Time.time}");
                    break;
                default:
                    break;
            }
        }

        // Unity's low level input event queuing system apparently does not like
        // multiple events to be queued for a single device each input round,
        // making it necessary to store all events per device ahead of time.
        foreach (var kvp in keyboardMessages)
        {
            Keyboard keyboard = guests[kvp.Key];

            using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            {
                foreach (var msg in kvp.Value)
                {
                    if (ParsecInputSystemMapping.Keys.TryGetValue(msg.keyboard.code, out Key key))
                    {
                        ParsecUnityController.Log($"Guest {kvp.Key} {(msg.keyboard.pressed ? "pressed" : "released")} key {key} at {Time.time}");

                        keyboard[key].WriteValueIntoEvent<float>(msg.keyboard.pressed ? 1 : 0, eventPtr);                        
                    }

                    InputSystem.QueueEvent(eventPtr);
                }
            }
        }
    }
}
