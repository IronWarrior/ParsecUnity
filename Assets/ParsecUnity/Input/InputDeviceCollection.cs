using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.LowLevel;
using System.Collections.Generic;

public class InputDeviceCollection
{
    public event System.Action OnUseUnpairedDevice;
    public InputDevice[] Devices => devices.ToArray();

    private readonly List<InputDevice> devices;

    public InputDeviceCollection()
    {
        devices = new List<InputDevice>();

        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
        // TODO: Find out why this is an int and not a bool. Line taken from
        // PlayerInput.cs.
        ++InputUser.listenForUnpairedDeviceActivity;
    }

    ~InputDeviceCollection()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
    }

    public InputDeviceCollection(params InputDevice[] devices) : this()
    {
        for (int i = 0; i < devices.Length; i++)
        {
            this.devices.Add(devices[i]);
        }
    }

    private void OnUnpairedDeviceUsed(InputControl control, InputEventPtr eventPtr)
    {
        int indexOfDevice = devices.IndexOf(control.device);

        // Move the device to the front of the list so that it has priority
        // if the list is passed into a control scheme.
        if (indexOfDevice >= 0)
        {
            InputDevice temp = devices[0];
            devices[0] = control.device;
            devices[indexOfDevice] = temp;

            OnUseUnpairedDevice?.Invoke();
        }
    }

    /// <summary>
    /// Creates a virtual input device of the provided type and adds it this collection.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T CreateAndAdd<T>() where T : InputDevice
    {        
        T device = InputSystem.AddDevice<T>();
        devices.Add(device);

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

    public void Free()
    {
        foreach (var device in devices)
        {
            InputSystem.RemoveDevice(device);            
        }
    }
}