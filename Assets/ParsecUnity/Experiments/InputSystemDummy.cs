using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

public class InputSystemDummy : MonoBehaviour
{
    [SerializeField]
    int executeFrame = 1;

    [SerializeField]
    public bool doA;

    [SerializeField]
    public bool doB;

    [Header("Debug")]
    public bool pressedA;
    public bool pressedB;

    private Keyboard keyboard;

    private void Start()
    {
        keyboard = InputSystem.AddDevice<Keyboard>();
    }

    private void Update()
    {
        if (Time.frameCount == executeFrame)
        {
            using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            {
                keyboard[Key.A].WriteValueIntoEvent<float>(1, eventPtr);
                keyboard[Key.B].WriteValueIntoEvent<float>(1, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }

            //if (doA)
            //{
            //    using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            //    {
            //        keyboard[Key.A].WriteValueIntoEvent<float>(1, eventPtr);
            //        InputSystem.QueueEvent(eventPtr);
            //    }
            //}

            //if (doB)
            //{
            //    using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            //    {
            //        keyboard[Key.B].WriteValueIntoEvent<float>(1, eventPtr);
            //        InputSystem.QueueEvent(eventPtr);
            //    }
            //}
        }
        else if (Time.frameCount == executeFrame * 2)
        {
            using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            {
                keyboard[Key.A].WriteValueIntoEvent<float>(0, eventPtr);
                keyboard[Key.B].WriteValueIntoEvent<float>(0, eventPtr);
                InputSystem.QueueEvent(eventPtr);
            }

            //if (doA)
            //{
            //    using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            //    {
            //        keyboard[Key.A].WriteValueIntoEvent<float>(0, eventPtr);
            //        InputSystem.QueueEvent(eventPtr);
            //    }
            //}

            //if (doB)
            //{
            //    using (StateEvent.From(keyboard, out InputEventPtr eventPtr))
            //    {
            //        keyboard[Key.B].WriteValueIntoEvent<float>(0, eventPtr);
            //        InputSystem.QueueEvent(eventPtr);
            //    }
            //}
        }

        pressedA = keyboard[Key.A].ReadValue() > 0.1f;
        pressedB = keyboard[Key.B].ReadValue() > 0.1f;
    }

    private void OnDestroy()
    {
        InputSystem.RemoveDevice(keyboard);
    }
}
