using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField, Tooltip("How long a run is in seconds. Example: 5 mins is 300 seconds.")] public float MaxTotalRunTime;
    [SerializeField, Tooltip("Current game run time.")] public float TotalRunTime = 0.0f;

    [SerializeField, Tooltip("Global resources. 0:WATER, 1:FOOD, 2:ENERGY, 3:HEALTH")] private Vector4 _globalResources;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    void Awake()
    {
        PlayerConfigurationHandler.Instance.SpawnPlayers();
        TaskHandler.OnResourceChangeEvent += OnResourceChange;
    }

    // Update is called once per frame
    void Update()
    {
        TotalRunTime = Time.timeSinceLevelLoad;
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
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        TaskHandler.OnResourceChangeEvent -= OnResourceChange;
    }
}
