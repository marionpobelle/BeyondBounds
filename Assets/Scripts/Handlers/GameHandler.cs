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
    [SerializeField, Tooltip("By how much the global resources deplete when they do. 0:WATER, 1:FOOD, 2:ENERGY, 3:HEALTH")] private Vector4 _globalResourcesDepletingAmounts;
    [SerializeField, Tooltip("Time in seconds in between each resource depleting. 0:WATER, 1:FOOD, 2:ENERGY, 3:HEALTH")] private Vector4 _globalResourcesDepletingCooldowns;

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
        UpdateHUDResources();
        DepleteGlobalResources();
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
        UpdateHUDResources();
    }

    /// <summary>
    /// Updates the HUD resource values.
    /// /// <param name="resourceChanges">Changs to apply.</param>
    /// </summary>
    private void UpdateHUDResources()
    {
        _waterText.text = _globalResources.x.ToString();
        _foodText.text = _globalResources.y.ToString();
        _energyText.text = _globalResources.z.ToString();
        _healthText.text = _globalResources.w.ToString();
    }

    /// <summary>
    /// Coroutine that depletes the water resource over time
    /// </summary>
    private IEnumerator DepleteResourceWater()
    {
        while (true)
        {
            yield return new WaitForSeconds(_globalResourcesDepletingCooldowns[0]);
            _globalResources[0] -= _globalResourcesDepletingAmounts[0];
            UpdateHUDResources();
        }
    }

    /// <summary>
    /// Coroutine that depletes the food resource over time
    /// </summary>
    private IEnumerator DepleteResourceFood()
    {
        while (true)
        {
            yield return new WaitForSeconds(_globalResourcesDepletingCooldowns[1]);
            _globalResources[1] -= _globalResourcesDepletingAmounts[1];
            UpdateHUDResources();
        }
    }

    /// <summary>
    /// Coroutine that depletes the energy resource over time
    /// </summary>
    private IEnumerator DepleteResourceEnergy()
    {
        while (true)
        {
            yield return new WaitForSeconds(_globalResourcesDepletingCooldowns[2]);
            _globalResources[2] -= _globalResourcesDepletingAmounts[2];
            UpdateHUDResources();
        }
    }

    /// <summary>
    /// Coroutine that depletes the health resource over time
    /// </summary>
    private IEnumerator DepleteResourceHealth()
    {
        while (true)
        {
            yield return new WaitForSeconds(_globalResourcesDepletingCooldowns[3]);
            _globalResources[3] -= _globalResourcesDepletingAmounts[3];
            UpdateHUDResources();
        }
    }

    /// <summary>
    /// Depletes all resources over time.
    /// </summary>
    private void DepleteGlobalResources()
    {
        StartCoroutine(DepleteResourceWater());
        StartCoroutine(DepleteResourceFood());
        StartCoroutine(DepleteResourceEnergy());
        StartCoroutine(DepleteResourceHealth());
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        TaskHandler.OnResourceChangeEvent -= OnResourceChange;
    }
}
