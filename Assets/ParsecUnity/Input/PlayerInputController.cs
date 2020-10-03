using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerInputController : MonoBehaviour
{
    private InputDeviceCollection deviceCollection;

    public void Initialize(InputDeviceCollection deviceCollection)
    {
        deviceCollection.OnUseUnpairedDevice += DeviceCollection_OnUseUnpairedDevice;

        this.deviceCollection = deviceCollection;
    }

    private void DeviceCollection_OnUseUnpairedDevice()
    {
        var devices = deviceCollection.Devices;        

        var playerInput = GetComponent<PlayerInput>();

        if (InputControlScheme.FindControlSchemeForDevices(devices, playerInput.actions.controlSchemes,
                    out _, out var matchResult, mustIncludeDevice: devices[0]))
        {
            try
            {
                var array = matchResult.devices.ToArray();

                Debug.Log($"Switching to devices {string.Join<object>(",", array)}");
                playerInput.SwitchCurrentControlScheme(array);
            }
            finally
            {
                matchResult.Dispose();
            }
        }
    }

    private void OnDestroy()
    {
        deviceCollection.OnUseUnpairedDevice -= DeviceCollection_OnUseUnpairedDevice;
    }
}
