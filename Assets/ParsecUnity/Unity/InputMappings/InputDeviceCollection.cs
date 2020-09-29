using UnityEngine.InputSystem;
using System.Collections.Generic;

public class InputDeviceCollection
{
    public event System.Action<InputDevice> OnUseDifferentDevice;

    public InputDevice[] Devices => devices.ToArray();
    private readonly List<InputDevice> devices;

    private InputDevice mostRecentlyUsedDevice;

    public InputDeviceCollection()
    {
        devices = new List<InputDevice>();
    }

    public T Add<T>() where T : InputDevice
    {        
        T device = InputSystem.AddDevice<T>();
        devices.Add(device);
        Use(device);

        return device;
    }

    public bool Get<T>(out T device) where T : InputDevice
    {
        for (int i = 0; i < devices.Count; i++)
        {
            if (devices[i] is T t)
            {
                device = t;
                return true;
            }
        }

        device = null;
        return false;
    }

    /// <summary>
    /// Call this when a device is used. This allow this object to notify
    /// listeners of <see cref="OnUseDifferentDevice"/> when the "current"
    /// device has changed.
    /// </summary>
    /// <param name="device"></param>
    /// <returns>True if the current device has now changed.</returns>
    public bool Use(InputDevice device)
    {
        if (devices.Contains(device) && mostRecentlyUsedDevice != device)
        {
            mostRecentlyUsedDevice = device;
            OnUseDifferentDevice?.Invoke(device);

            return true;
        }

        return false;
    }

    public void Free()
    {
        foreach (var device in devices)
        {
            InputSystem.RemoveDevice(device);
        }
    }
}