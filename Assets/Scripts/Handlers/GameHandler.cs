using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameHandler : MonoBehaviour
{
    [SerializeField, Tooltip("How long a run is in seconds. Example: 5 mins is 300 seconds.")] public float MaxTotalRunTime;
    [SerializeField, Tooltip("Current game run time.")] public float TotalRunTime = 0.0f;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    void Awake()
    {
        PlayerConfigurationHandler.Instance.SpawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        TotalRunTime = Time.timeSinceLevelLoad;
        if(TotalRunTime >= MaxTotalRunTime)
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
}
