using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EndScreenHandler : MonoBehaviour
{

    [SerializeField] private Button _restartButton;
    [SerializeField] private Button _quitButton;

    [SerializeField] private EventSystem _layoutEventSystem;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    void Awake()
    {
        _layoutEventSystem.SetSelectedGameObject(_restartButton.gameObject);
        _restartButton.onClick.AddListener(OnRestartButtonClicked);
        _quitButton.onClick.AddListener(OnQuitButtonClicked);
    }

    private void OnQuitButtonClicked()
    {
        Application.Quit();
    }

    private void OnRestartButtonClicked()
    {
        PlayerConfigurationHandler.Instance.ResetGame();
        SceneManager.LoadScene("PlayerJoinSetup");
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        _restartButton.onClick.RemoveListener(OnRestartButtonClicked);
        _quitButton.onClick.RemoveListener(OnQuitButtonClicked);
    }
}
