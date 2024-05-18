using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.UI;

public class PlayerJoinSetupHandler : MonoBehaviour
{
    [SerializeField] private GameObject _playerSetupPanelPrefab;
    [SerializeField] private PlayerController _playerController;
    private GameObject _setupPanel;

    //Start is called on the frame when a script is enabled just before any of the Update methods are called the first time.
    private void Start()
    {
        var rootMenu = GameObject.Find("Main Layout");
        if (rootMenu != null)
        {
            _setupPanel = Instantiate(_playerSetupPanelPrefab, rootMenu.transform);
            var uiInput = _setupPanel.GetComponentInChildren<InputSystemUIInputModule>();
            if (uiInput == null)
            {
                Debug.LogError("The input module is null", this);
                return;
            }

            var playerControls = _playerController.GetPlayerControls();
            if (playerControls == null)
            {
                Debug.LogError("The player controls can't be retrieved from the PlayerController", this);
                return;
            }
            uiInput.actionsAsset = playerControls.asset;
            _setupPanel.GetComponent<PlayerSetupPanel>().SetPlayerIndex(_playerController.PlayerIndex);
            _setupPanel.GetComponent<PlayerSetupPanel>().SetPlayerControls(playerControls);
        }
    }
}
