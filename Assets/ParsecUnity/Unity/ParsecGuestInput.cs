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

        // TODO: Mouse button input is currently not being received on the host.
        // If no mouse is plugged in, this will be null.
        var mouse = Mouse.current;

        if (mouse != null)
        {
            if (mouse.leftButton.wasPressedThisFrame || mouse.leftButton.wasReleasedThisFrame)
            {
                var message = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_MOUSE_BUTTON };

                message.mouseButton.button = Parsec.ParsecMouseButton.MOUSE_L;
                message.mouseButton.pressed = mouse.leftButton.wasPressedThisFrame;

                parsec.ClientSendMessage(message);
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
        }

        var gamepad = Gamepad.current;

        if (gamepad != null)
        {
            foreach (var kvp in ParsecInputSystemMapping.GamepadButtonsMap(gamepad))
            {
                if (kvp.Value.wasPressedThisFrame || kvp.Value.wasReleasedThisFrame)
                {
                    var message = new Parsec.ParsecMessage { type = Parsec.ParsecMessageType.MESSAGE_GAMEPAD_BUTTON };

                    message.gamepadButton.button = kvp.Key;
                    message.gamepadButton.pressed = kvp.Value.wasPressedThisFrame;

                    parsec.ClientSendMessage(message);
                }
            }
        }
    }
}
