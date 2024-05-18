using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class TaskHandler : MonoBehaviour
{

    [SerializeField, Tooltip("How long the task is in seconds.")] private int _taskDuration;
    [SerializeField, Tooltip("How long the task is in seconds.")] private int _taskCooldown;
    [SerializeField, Tooltip("The variation in the resources this task engender. 0:WATER, 1:FOOD, 2:ENERGY, 3:HEALTH")] private Vector4 _resourceChanges;

    [SerializeField, Tooltip("Is the task being performed.")] private bool _canPerformTask;
    [Tooltip("Is the task being performed.")] public bool IsTaskRunning;
    [SerializeField, Tooltip("The player performing the task.")] private PlayerController _playerDoingTask;

    //Events
    public static event Action<Vector4> OnResourceChangeEvent;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        _playerDoingTask = null; 
        IsTaskRunning = false;
        _canPerformTask = true;
    }

    /// <summary>
    /// Couroutine running the task.
    /// </summary>
    public IEnumerator RunTask()
    {
        Debug.Log("Task started !");
        IsTaskRunning = true;
        _canPerformTask = false;
        yield return new WaitForSeconds(_taskDuration);
        if (IsTaskRunning)
        {
            OnResourceChangeEvent?.Invoke(_resourceChanges);
            Debug.Log("Did change the resources");
        }
        IsTaskRunning = false;
        if(_playerDoingTask != null) _playerDoingTask.SetPlayerOccupied(false);
        _playerDoingTask = null;
        yield return new WaitForSeconds(_taskCooldown);
        _canPerformTask = true;
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
    /// Checks if the task is on cooldown.
    /// </summary>
    public bool GetTaskAvailability()
    {
        return _canPerformTask;
    }
}
