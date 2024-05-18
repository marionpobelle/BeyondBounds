using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class PlayerSetupPanel : MonoBehaviour
{
    [SerializeField, Tooltip("Player index of the owner of the panel")] private int _playerIndex;
    [SerializeField, Tooltip("List of colors representing the players")] private List<Color> _playerColors;

    private PlayerControls _playerControls;
    private bool _inputEnabled;
    private MultiplayerEventSystem _multiplayerEventSystem;

    [SerializeField] private TextMeshProUGUI _readyText;
    [SerializeField] private TextMeshProUGUI _playerIndexText;
    [SerializeField] private Button _readyButton;


    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        _multiplayerEventSystem = GetComponent<MultiplayerEventSystem>();
        if (_multiplayerEventSystem == null)
        {
            Debug.LogError("Multiplayer Event System is null and can't get retrieved", this);
        }
        _readyButton.onClick.AddListener(OnReadyButtonClicked);
        _inputEnabled = true;
        _multiplayerEventSystem.SetSelectedGameObject(_readyButton.gameObject);
    }

    /// <summary>
    /// Performs a set of actions after the ready button has been clicked.
    /// </summary>
    private void OnReadyButtonClicked()
    {
        Debug.Log("ReadyButton Clicked !");
        ReadyPlayer();
    }

    /// <summary>
    /// Sets the player as ready.
    /// </summary>
    public void ReadyPlayer()
    {
        if (!_inputEnabled)
        {
            return;
        }
        PlayerConfigurationHandler.Instance.ReadyPlayer(_playerIndex);
        _readyButton.gameObject.SetActive(false);
        _readyText.gameObject.SetActive(true);
        _playerControls.UI.Disable();
    }

    /// <summary>
    /// Sets the player index.
    /// </summary>
    /// <param name="pIndex">Player index. </param>
    public void SetPlayerIndex(int pIndex)
    {
        _playerIndex = pIndex;
        _playerIndexText.text = ("P" + (pIndex + 1).ToString());
        _playerIndexText.color = _playerColors[pIndex];
    }

    /// <summary>
    /// Sets the player controls.
    /// </summary>
    /// <param name="pControls">Player controls. </param>
    public void SetPlayerControls(PlayerControls pControls)
    {
        _playerControls = pControls;
        if (_playerControls == null)
        {
            Debug.LogError("Could not retrieve the player controller", this);
        }
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        _readyButton.onClick.RemoveListener(OnReadyButtonClicked);
    }
}
