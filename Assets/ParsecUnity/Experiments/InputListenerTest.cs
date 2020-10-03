using UnityEngine;
using UnityEngine.InputSystem.Users;

public class InputListenerTest : MonoBehaviour
{
    private void Start()
    {
        InputUser.onUnpairedDeviceUsed += InputUser_onUnpairedDeviceUsed;
        ++InputUser.listenForUnpairedDeviceActivity;
    }

    private void InputUser_onUnpairedDeviceUsed(UnityEngine.InputSystem.InputControl control, UnityEngine.InputSystem.LowLevel.InputEventPtr arg2)
    {
        Debug.Log($"using device {control.device.name}");
    }

    private void OnDestroy()
    {
        InputUser.onUnpairedDeviceUsed -= InputUser_onUnpairedDeviceUsed;
    }
}
