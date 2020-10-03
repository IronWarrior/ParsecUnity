using UnityEngine.InputSystem;
using System.Collections.Generic;
using ParsecGaming;
using UnityEngine.InputSystem.Controls;

public static class ParsecInputSystemMapping
{
    public static readonly Dictionary<Parsec.ParsecKeycode, Key> Keys = new Dictionary<Parsec.ParsecKeycode, Key>()
    {
        { Parsec.ParsecKeycode.KEY_A, Key.A },
        { Parsec.ParsecKeycode.KEY_B, Key.B },
        { Parsec.ParsecKeycode.KEY_C, Key.C },
        { Parsec.ParsecKeycode.KEY_D, Key.D },
        { Parsec.ParsecKeycode.KEY_E, Key.E },
        { Parsec.ParsecKeycode.KEY_F, Key.F },
        { Parsec.ParsecKeycode.KEY_G, Key.G },
        { Parsec.ParsecKeycode.KEY_H, Key.H },
        { Parsec.ParsecKeycode.KEY_I, Key.I },
        { Parsec.ParsecKeycode.KEY_J, Key.J },
        { Parsec.ParsecKeycode.KEY_K, Key.K },
        { Parsec.ParsecKeycode.KEY_L, Key.L },
        { Parsec.ParsecKeycode.KEY_M, Key.M },
        { Parsec.ParsecKeycode.KEY_N, Key.N },
        { Parsec.ParsecKeycode.KEY_O, Key.O },
        { Parsec.ParsecKeycode.KEY_P, Key.P },
        { Parsec.ParsecKeycode.KEY_Q, Key.Q },
        { Parsec.ParsecKeycode.KEY_R, Key.R },
        { Parsec.ParsecKeycode.KEY_S, Key.S },
        { Parsec.ParsecKeycode.KEY_T, Key.T },
        { Parsec.ParsecKeycode.KEY_U, Key.U },
        { Parsec.ParsecKeycode.KEY_V, Key.V },
        { Parsec.ParsecKeycode.KEY_W, Key.W },
        { Parsec.ParsecKeycode.KEY_X, Key.X },
        { Parsec.ParsecKeycode.KEY_Y, Key.Y },
        { Parsec.ParsecKeycode.KEY_Z, Key.Z },

        { Parsec.ParsecKeycode.KEY_1, Key.Digit1 },
        { Parsec.ParsecKeycode.KEY_2, Key.Digit2 },
        { Parsec.ParsecKeycode.KEY_3, Key.Digit3 },
        { Parsec.ParsecKeycode.KEY_4, Key.Digit4 },
        { Parsec.ParsecKeycode.KEY_5, Key.Digit5 },
        { Parsec.ParsecKeycode.KEY_6, Key.Digit6 },
        { Parsec.ParsecKeycode.KEY_7, Key.Digit7 },
        { Parsec.ParsecKeycode.KEY_8, Key.Digit8 },
        { Parsec.ParsecKeycode.KEY_9, Key.Digit9 },
        { Parsec.ParsecKeycode.KEY_0, Key.Digit0 },

        { Parsec.ParsecKeycode.KEY_SPACE, Key.Space },
        { Parsec.ParsecKeycode.KEY_ENTER, Key.Enter },
        { Parsec.ParsecKeycode.KEY_TAB, Key.Tab },
        { Parsec.ParsecKeycode.KEY_LSHIFT, Key.LeftShift },
        { Parsec.ParsecKeycode.KEY_RSHIFT, Key.RightShift },
        { Parsec.ParsecKeycode.KEY_LALT, Key.LeftAlt },
        { Parsec.ParsecKeycode.KEY_RALT, Key.RightAlt },
        { Parsec.ParsecKeycode.KEY_LCTRL, Key.LeftCtrl },
        { Parsec.ParsecKeycode.KEY_RCTRL, Key.RightCtrl },

        { Parsec.ParsecKeycode.KEY_UP, Key.UpArrow },
        { Parsec.ParsecKeycode.KEY_DOWN, Key.DownArrow },
        { Parsec.ParsecKeycode.KEY_RIGHT, Key.RightArrow },
        { Parsec.ParsecKeycode.KEY_LEFT, Key.LeftArrow }
    };

    // TODO: Probably worth wrapping all this in a class that has a Gamepad injected.
    public static Dictionary<Parsec.ParsecGamepadButton, ButtonControl> GamepadButtons(Gamepad gamepad)
    {
        return new Dictionary<Parsec.ParsecGamepadButton, ButtonControl>()
        {
            // Ahh yes, the "A" button: https://images.pushsquare.com/a7c69ec3b5207/playstation-x-button-cross-xbox.original.jpg
            // (Parsec seems to match an Xbox controller layout.)
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_A, gamepad.buttonSouth },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_B, gamepad.buttonEast },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_X, gamepad.buttonWest },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_Y, gamepad.buttonNorth },

            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_LSHOULDER, gamepad.leftShoulder },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_RSHOULDER, gamepad.rightShoulder },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_LSTICK, gamepad.leftStickButton },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_RSTICK, gamepad.rightStickButton },

            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_UP, gamepad.dpad.up },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_DOWN, gamepad.dpad.down },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_LEFT, gamepad.dpad.left },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_RIGHT, gamepad.dpad.right },

            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_GUIDE, gamepad.startButton },
            { Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_BACK, gamepad.selectButton }
        };
    }

    public static Dictionary<Parsec.ParsecGamepadAxis, AxisControl> GamepadAxes(Gamepad gamepad)
    {
        return new Dictionary<Parsec.ParsecGamepadAxis, AxisControl>()
        {
            { Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_LX, gamepad.leftStick.x },
            { Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_LY, gamepad.leftStick.y },

            { Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_RX, gamepad.rightStick.x },
            { Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_RY, gamepad.rightStick.y },
        };
    }

    public static ButtonControl ParsecToGamepadButton(Gamepad gamepad, Parsec.ParsecGamepadButton button)
    {
        switch (button)
        {
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_A:
                return gamepad.buttonSouth;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_B:
                return gamepad.buttonEast;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_X:
                return gamepad.buttonWest;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_Y:
                return gamepad.buttonNorth;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_LSHOULDER:
                return gamepad.leftShoulder;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_RSHOULDER:
                return gamepad.rightShoulder;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_LSTICK:
                return gamepad.leftStickButton;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_RSTICK:
                return gamepad.rightStickButton;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_UP:
                return gamepad.dpad.up;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_DOWN:
                return gamepad.dpad.down;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_LEFT:
                return gamepad.dpad.left;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_DPAD_RIGHT:
                return gamepad.dpad.right;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_GUIDE:
                return gamepad.startButton;
            case Parsec.ParsecGamepadButton.GAMEPAD_BUTTON_BACK:
                return gamepad.selectButton;
            default:
                return null;
        }
    }

    public static AxisControl ParsecToGamepadAxis(Gamepad gamepad, Parsec.ParsecGamepadAxis axis)
    {
        switch (axis)
        {
            case Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_LX:
                return gamepad.leftStick.x;
            case Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_LY:
                return gamepad.leftStick.y;
            case Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_RX:
                return gamepad.rightStick.x;
            case Parsec.ParsecGamepadAxis.GAMEPAD_AXIS_RY:
                return gamepad.rightStick.y;
            default:
                return null;
        }
    }
}
