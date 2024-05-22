using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField, Tooltip("How long a run is in seconds. Example: 5 mins is 300 seconds.")] public float MaxTotalRunTime;
    [SerializeField, Tooltip("Current game run time.")] public float TotalRunTime = 0.0f;

    [SerializeField, Tooltip("Global resources. 0:WATER, 1:FOOD, 2:ENERGY, 3:HEALTH")] private Vector4 _globalResources;

    [SerializeField] private TextMeshProUGUI _waterText;
    [SerializeField] private TextMeshProUGUI _foodText;
    [SerializeField] private TextMeshProUGUI _energyText;
    [SerializeField] private TextMeshProUGUI _healthText;

    [SerializeField] private TextMeshProUGUI _clockText;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    void Awake()
    {
        PlayerConfigurationHandler.Instance.SpawnPlayers();
        TaskHandler.OnResourceChangeEvent += OnResourceChange;
    }

    //Start is called on the frame when a script is enabled
    private void Start()
    {
        UpdateHUDResources(_globalResources);
    }

    // Update is called once per frame
    void Update()
    {
        TotalRunTime = Time.timeSinceLevelLoad;
        int minutes = Mathf.FloorToInt((MaxTotalRunTime - TotalRunTime) / 60F);
        int seconds = Mathf.FloorToInt((MaxTotalRunTime - TotalRunTime) - minutes * 60);
        _clockText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
        if (TotalRunTime >= MaxTotalRunTime)
        {
            LoadRunOver();
        }
    }

    /// <summary>
    /// Loads the end screen in case the players win.
    /// </summary>
    private void LoadRunOver()
    {
        PlayerConfigurationHandler.Instance.ResetPlayers();
        SceneManager.LoadScene("EndScreenWon");
    }

    /// <summary>
    /// Applies changes to the global resources.
    /// /// <param name="resourceChanges">Changs to apply.</param>
    /// </summary>
    private void OnResourceChange(Vector4 resourceChanges)
    {
        _globalResources += resourceChanges;
        UpdateHUDResources(resourceChanges);
    }

    /// <summary>
    /// Updates the HUD resource values.
    /// /// <param name="resourceChanges">Changs to apply.</param>
    /// </summary>
    private void UpdateHUDResources(Vector4 resourceChanges)
    {
        _waterText.text = resourceChanges.x.ToString();
        _foodText.text = resourceChanges.y.ToString();
        _energyText.text = resourceChanges.z.ToString();
        _healthText.text = resourceChanges.w.ToString();
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        TaskHandler.OnResourceChangeEvent -= OnResourceChange;
    }
}
