using ParsecGaming;
using UnityEngine;
using UnityEngine.InputSystem;

public class ParsecGuestInput : MonoBehaviour
{
    private Parsec parsec;

    public void Initialize(Parsec parsec)
    {
        this.parsec = parsec;
    }

    private void LateUpdate()
    {
        // If no keyboard is plugged in, this will be null.
        var keyboard = Keyboard.current;

        if (keyboard != null)
        {
            foreach (var kvp in ParsecInputSystemMapping.Keys)
            {
                if (keyboard[kvp.Value].wasPressedThisFrame || keyboard[kvp.Value].wasReleasedThisFrame)
                {
                    var message = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_KEYBOARD };

                    message.keyboard.code = kvp.Key;
                    message.keyboard.pressed = keyboard[kvp.Value].wasPressedThisFrame;

                    parsec.ClientSendMessage(message);
                }
            }
        }

        // If no mouse is plugged in, this will be null.
        var mouse = Mouse.current;

        if (mouse != null)
        {
            foreach (var kvp in ParsecInputSystemMapping.MouseButtons(mouse))
            {
                if (kvp.Value.wasPressedThisFrame || kvp.Value.wasReleasedThisFrame)
                {
                    var message = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_MOUSE_BUTTON };

                    message.mouseButton.button = kvp.Key;
                    message.mouseButton.pressed = kvp.Value.wasPressedThisFrame;

                    parsec.ClientSendMessage(message);
                }
            }

            // Unity stores the deltas as floats, but they are always
            // whole numbers.
            int deltaX = (int)mouse.delta.x.ReadValue();
            int deltaY = (int)mouse.delta.y.ReadValue();

            if (deltaX > 0 || deltaY > 0)
            {
                var message = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_MOUSE_MOTION };

                message.mouseMotion.relative = true;
                message.mouseMotion.x = deltaX;
                message.mouseMotion.y = deltaY;

                parsec.ClientSendMessage(message);
            }

            int posX = (int)mouse.position.x.ReadValue();
            int posY = (int)mouse.position.y.ReadValue();

            var positionMessage = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_MOUSE_MOTION };

            positionMessage.mouseMotion.relative = false;
            positionMessage.mouseMotion.x = posX;
            positionMessage.mouseMotion.y = posY;

            parsec.ClientSendMessage(positionMessage);
        }

        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            foreach (var kvp in ParsecInputSystemMapping.GamepadButtons(gamepad))
            {
                if (kvp.Value.wasPressedThisFrame || kvp.Value.wasReleasedThisFrame)
                {
                    var message = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_GAMEPAD_BUTTON };

                    message.gamepadButton.button = kvp.Key;
                    message.gamepadButton.pressed = kvp.Value.wasPressedThisFrame;

                    parsec.ClientSendMessage(message);
                }
            }

            foreach (var kvp in ParsecInputSystemMapping.GamepadAxes(gamepad))
            {
                short axis = (short)(kvp.Value.ReadValue() * short.MaxValue);

                var msg = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_GAMEPAD_AXIS };
                msg.gamepadAxis.axis = kvp.Key;
                msg.gamepadAxis.value = axis;

                parsec.ClientSendMessage(msg);
            }
        }
    }
}
