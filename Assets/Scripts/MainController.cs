using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

public class MainController : MonoBehaviour
{
    public PlayerControls playerControls;
    private InputUser inputUser;

    //OnEnable is called when the object becomes enabled and active.
    private void OnEnable()
    {
        if (playerControls != null) playerControls.Enable();
    }

    //OnDisable is called when the object becomes disabled and inactive.
    private void OnDisable()
    {
        playerControls.Disable();
    }

    /// <summary>
    /// Binds the main controller with the given device.
    /// </summary>
    /// <param name="device">A given controller.</param>
    public void BindWithDevice(InputDevice device)
    {
        inputUser = InputUser.PerformPairingWithDevice(device);
        playerControls = new PlayerControls();
        inputUser.AssociateActionsWithUser(playerControls);
        playerControls.Enable();
        Debug.Log($"Bind device : {device.path}!");
    }

    /// <summary>
    /// Unbinds the main controller with its device.
    /// </summary>
    public void UnbindWithDevice()
    {
        inputUser.UnpairDevicesAndRemoveUser();
        playerControls.Disable();
        playerControls.Dispose();
    }

    /// <summary>
    /// Gets the main controller device.
    /// </summary>
    public string GetDevicePath()
    {
        return inputUser.pairedDevices[0].path;
    }

    /// <summary>
    /// Gives the main controller control over the main UI.
    /// </summary>
    public void SetControlOverMenu()
    {
        var rootMenu = GameObject.Find("MainLayout");
        if (rootMenu != null)
        {
            var uiInput = rootMenu.GetComponentInChildren<InputSystemUIInputModule>();
            if (uiInput == null)
            {
                Debug.LogError("The input module is null", this);
                return;
            }
            uiInput.actionsAsset = playerControls.asset;
        }
    }
}
