﻿using UnityEngine;
using UnityEngine.InputSystem;
using ParsecGaming;
using System.Collections.Generic;
using UnityEngine.InputSystem.LowLevel;

// TODO: The Input System-specific code should be moved to a different module. This
// class would then only be responsible for polymorphically converting Parsec inputs
// to the module's domain, and surfacing the converted inputs to the app.
public class ParsecHostInput : MonoBehaviour
{
    private Parsec parsec;

    public Dictionary<uint, InputDeviceCollection> guests;

    private void Awake()
    {
        guests = new Dictionary<uint, InputDeviceCollection>();
    }

    private void OnDestroy()
    {
        foreach (var kvp in guests)
        {
            kvp.Value.Free();
        }
    }

    public void Initialize(Parsec parsec)
    {
        this.parsec = parsec;
    }

    public InputDeviceCollection AddGuest(Parsec.ParsecGuest guest)
    {
        guests[guest.id] = new InputDeviceCollection();
        return guests[guest.id];
    }

    // TODO: This should be applied before all updates, or whenever
    // input normally is applied (based on the InputSystem configuration).
    private void Update()
    {
        // TODO: Probably don't need to allocate a new set of dictionaries every frame.
        var keyboardMessages = new Dictionary<uint, List<Parsec.ParsecMessage>>();
        var mouseMessages = new Dictionary<uint, List<Parsec.ParsecMessage>>();
        var gamepadMessages = new Dictionary<uint, List<Parsec.ParsecMessage>>();

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
                    if (!mouseMessages.ContainsKey(guest.id))
                        mouseMessages[guest.id] = new List<Parsec.ParsecMessage>();

                    mouseMessages[guest.id].Add(msg); 
                    break;
                case Parsec.ParsecMessageType.MESSAGE_MOUSE_MOTION:
                    if (!mouseMessages.ContainsKey(guest.id))
                        mouseMessages[guest.id] = new List<Parsec.ParsecMessage>();

                    mouseMessages[guest.id].Add(msg);
                    break;
                case Parsec.ParsecMessageType.MESSAGE_GAMEPAD_BUTTON:
                    if (!gamepadMessages.ContainsKey(guest.id))
                        gamepadMessages[guest.id] = new List<Parsec.ParsecMessage>();

                    gamepadMessages[guest.id].Add(msg);
                    break;
                case Parsec.ParsecMessageType.MESSAGE_GAMEPAD_AXIS:
                    if (!gamepadMessages.ContainsKey(guest.id))
                        gamepadMessages[guest.id] = new List<Parsec.ParsecMessage>();

                    gamepadMessages[guest.id].Add(msg);
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
            if (!guests[kvp.Key].Get(out Keyboard keyboard))
            {
                keyboard = guests[kvp.Key].CreateAndAdd<Keyboard>();
            }

            using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            {
                foreach (var msg in kvp.Value)
                {
                    if (ParsecInputSystemMapping.Keys.TryGetValue(msg.keyboard.code, out Key key))
                    {
                        ParsecUnityController.Log($"Guest {kvp.Key} {(msg.keyboard.pressed ? "pressed" : "released")} key {key} at {Time.time}");

                        keyboard[key].WriteValueIntoEvent<float>(msg.keyboard.pressed ? 1 : 0, eventPtr);                        
                    }                    
                }

                InputSystem.QueueEvent(eventPtr);
            }
        }

        foreach (var kvp in mouseMessages)
        {
            if (!guests[kvp.Key].Get(out Mouse mouse))
            {
                mouse = guests[kvp.Key].CreateAndAdd<Mouse>();
            }

            using (StateEvent.From(mouse, out InputEventPtr eventPtr))
            {
                foreach (var msg in kvp.Value)
                {
                    if (msg.type == Parsec.ParsecMessageType.MESSAGE_MOUSE_BUTTON)
                    {
                        var button = ParsecInputSystemMapping.ParsecToMouseButton(mouse, msg.mouseButton.button);

                        if (button != null)
                        {
                            ParsecUnityController.Log($"Guest {kvp.Key} {(msg.mouseButton.pressed ? "pressed" : "released")} {button} on mouse at {Time.time}");

                            button.WriteValueIntoEvent<float>(msg.mouseButton.pressed ? 1 : 0, eventPtr);
                        }
                    }
                    else if (msg.type == Parsec.ParsecMessageType.MESSAGE_MOUSE_MOTION && !msg.mouseMotion.relative)
                    {
                        // TODO: Logging levels for very low priority debugs, like mouse motion or position.
                        // ParsecUnityController.Log($"Guest {kvp.Key} mouse position is at {msg.mouseMotion.x}, {msg.mouseMotion.y} at {Time.time}");

                        mouse.position.x.WriteValueIntoEvent<float>(msg.mouseMotion.x, eventPtr);
                        mouse.position.y.WriteValueIntoEvent<float>(msg.mouseMotion.y, eventPtr);
                    }
                    else if (msg.type == Parsec.ParsecMessageType.MESSAGE_MOUSE_MOTION && msg.mouseMotion.relative)
                    {
                        // TODO: Logging levels for very low priority debugs, like mouse motion or position.
                        // ParsecUnityController.Log($"Guest {kvp.Key} mouse delta was {msg.mouseMotion.x}, {msg.mouseMotion.y} at {Time.time}");

                        mouse.delta.x.WriteValueIntoEvent<float>(msg.mouseMotion.x, eventPtr);
                        mouse.delta.y.WriteValueIntoEvent<float>(msg.mouseMotion.y, eventPtr);
                    }
                }

                InputSystem.QueueEvent(eventPtr);
            }
        }

        foreach (var kvp in gamepadMessages)
        {
            if (!guests[kvp.Key].Get(out Gamepad gamepad))
            {
                gamepad = guests[kvp.Key].CreateAndAdd<Gamepad>();
            }

            using (StateEvent.From(gamepad, out InputEventPtr eventPtr))
            {
                foreach (var msg in kvp.Value)
                {
                    if (msg.type == Parsec.ParsecMessageType.MESSAGE_GAMEPAD_BUTTON)
                    {
                        var button = ParsecInputSystemMapping.ParsecToGamepadButton(gamepad, msg.gamepadButton.button);

                        if (button != null)
                        {                            
                            ParsecUnityController.Log($"Guest {kvp.Key} {(msg.gamepadButton.pressed ? "pressed" : "released")} {button} on gamepad {msg.gamepadButton.id} at {Time.time}");

                            button.WriteValueIntoEvent<float>(msg.gamepadButton.pressed ? 1 : 0, eventPtr);
                        }                        
                    }
                    else if (msg.type == Parsec.ParsecMessageType.MESSAGE_GAMEPAD_AXIS)
                    {
                        var axis = ParsecInputSystemMapping.ParsecToGamepadAxis(gamepad, msg.gamepadAxis.axis);

                        if (axis != null)
                        {
                            float value = (float)msg.gamepadAxis.value / short.MaxValue;
                            
                            axis.WriteValueIntoEvent(value, eventPtr);
                        }
                    }
                }

                InputSystem.QueueEvent(eventPtr);
            }
        }
    }
}
