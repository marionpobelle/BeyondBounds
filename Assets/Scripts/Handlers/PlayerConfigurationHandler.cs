using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerConfigurationHandler : MonoBehaviour
{
    [SerializeField, Tooltip("Maximum amount of players.")] private int _maxPlayers = 4;
    [SerializeField, Tooltip("Minimum amount of players.")] private int _minPlayers = 1;
    [SerializeField, Tooltip("Player available spawn points")] private List<Vector3> _spawnPoints;
    [Tooltip("List of all players.")] public List<PlayerController> PlayerConfigs;

    [SerializeField] private Camera GameCamera;
    public InputAction JoinAction;
    [SerializeField] private PlayerController _playerControllerPrefab;

    public static event Action OnAllPlayersReadyEvent;

    //Singleton
    public static PlayerConfigurationHandler Instance { get; private set; }

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Trying to create another instance of Player Configuration Manager!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            PlayerConfigs = new List<PlayerController>();
            JoinAction.Enable();
            JoinAction.performed += PlayerJoinHandler;
        }
    }

    /// <summary>
    /// Set the player with index as ready. If all players are ready, starts the game.
    /// </summary>
    /// <param name="index">Index of the player.</param>
    public void ReadyPlayer(int index)
    {
        PlayerConfigs[index].IsReady = true;
        if (PlayerConfigs.Count >= _minPlayers && PlayerConfigs.All(player => player.IsReady == true))
        {
            SceneManager.LoadScene("Game");
            JoinAction.Disable();
            OnAllPlayersReadyEvent?.Invoke();
        }
    }

    /// <summary>
    /// Spawns all players in the scene.
    /// </summary>
    public void SpawnPlayers()
    {
        foreach (PlayerController player in PlayerConfigs)
        {
            player.transform.position = player.OriginPosition;
            player.enabled = true;
        }
    }

    /// <summary>
    /// Resets the game.
    /// </summary>
    public void ResetGame()
    {
        JoinAction.Enable();
    }

    /// <summary>
    /// Empty the list of players.
    /// </summary>
    public void ResetPlayers()
    {
        foreach (PlayerController player in PlayerConfigs)
        {
            Debug.Log("Disconnecting player " + player.PlayerIndex);
            _spawnPoints.Add(player.OriginPosition);
            player.UnbindWithDevice();
            Destroy(player.gameObject);
        }
        PlayerConfigs.Clear();
    }

    /// <summary>
    /// Assigns a camera to the player with index.
    /// </summary>
    /// <param name="pIndex">Index of the player.</param>
    /// <param name="camera">Camera to assign to the player.</param>
    public void SetPlayerCamera(int pIndex, Camera camera)
    {
        PlayerConfigs[pIndex].CurrentCamera = camera;
    }

    /// <summary>
    /// Tries to spawn in a player when the join input is pressed on a controller.
    /// </summary>
    /// <param name="context">Context containing the inputs.</param>
    private void PlayerJoinHandler(InputAction.CallbackContext context)
    {
        //If we reach the maximum amount of players
        if (PlayerConfigs.Count == _maxPlayers)
        {
            Debug.Log("Player Count is already at 4, can't Spawn any more player", this);
            return;
        }
        //If a device tries to join again when they are already controlling a player
        foreach (PlayerController player in PlayerConfigs)
        {
            if (context.control.device.path == player.GetDevicePath())
            {
                Debug.Log("Player already joined the game!");
                return;
            }
        }
        //Select a random spawn point for the player
        int randomSpawnIndex = UnityEngine.Random.Range(0, _spawnPoints.Count());
        PlayerController playerController = Instantiate(_playerControllerPrefab, _spawnPoints[randomSpawnIndex] + new Vector3(0, 0, 900), Quaternion.identity);
        playerController.transform.SetParent(transform);
        playerController.OriginPosition = _spawnPoints[randomSpawnIndex];
        _spawnPoints.RemoveAt(randomSpawnIndex);

        //Matching the player and their inputs to their specific controller
        playerController.BindWithDevice(context.control.device);
        playerController.PlayerIndex = PlayerConfigs.Count;
        playerController.name = "Player" + playerController.PlayerIndex;
        playerController.enabled = false;
        //Adding the player to the player list
        Debug.Log("Adding player " + PlayerConfigs.Count + "to list");
        PlayerConfigs.Add(playerController);
        SetPlayerCamera(playerController.PlayerIndex, GameCamera);

    }

    //OnEnable is called when the object becomes enabled and active.
    private void OnEnable()
    {
        if (JoinAction != null) JoinAction.Enable();
    }

    //OnDisable is called when the object becomes disabled and inactive.
    private void OnDisable()
    {
        JoinAction.Disable();
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        JoinAction.performed -= PlayerJoinHandler;
    }
}
