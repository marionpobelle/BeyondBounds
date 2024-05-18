using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{

    [SerializeField, Tooltip("How long the task is in seconds.")] private int _taskDuration;
    [SerializeField, Tooltip("The variation in the resources this task engender. 0:WATER, 1:FOOD, 2:ENERGY, 3:HEALTH")] private Vector4 _resourceChanges;

    [SerializeField, Tooltip("Is the task being performed.")] private bool _isTaskRunning;
    [SerializeField, Tooltip("The player performing the task.")] private PlayerController _playerDoingTask;

    //Events
    public static event Action<Vector4> OnResourceChangeEvent;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        _playerDoingTask = null; 
        _isTaskRunning = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isTaskRunning && _playerDoingTask != null)
        {
            StartCoroutine(RunTask());
        }
        if(_isTaskRunning && _playerDoingTask == null)
        {
            StopCoroutine(RunTask());
            _isTaskRunning = false;
            Debug.Log("Cancelled task");
        }
    }

    /// <summary>
    /// Couroutine running the task.
    /// </summary>
    private IEnumerator RunTask()
    {
        _isTaskRunning = true;
        yield return new WaitForSeconds(_taskDuration);
        _isTaskRunning = false;
        _playerDoingTask.SetPlayerOccupied(false);
        _playerDoingTask = null;
        //OnResourceChangeEvent?.Invoke(_resourceChanges);
        Debug.Log("Change In resources");
    }

    /// <summary>
    /// Sets the player performing the task.
    /// <param name="player">Player controller to set up as the player performing the task.</param>
    /// </summary>
    public void SetPlayer(PlayerController player)
    {
        _playerDoingTask = player;
    }

    /// <summary>
    /// Checks if the task is being performed by a player already or not.
    /// </summary>
    public bool GetRunningState()
    {
        return _isTaskRunning;
    }
}
