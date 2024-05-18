using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainControllerHandler : MonoBehaviour
{
    [SerializeField] private MainController _mainControllerPrefab;
    public InputAction JoinAction;
    [SerializeField] private MainController _mainController;

    //Events
    public static event Action OnMainControllerJoinedEvent;

    //Singleton
    public static MainControllerHandler Instance { get; private set; }

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.Log("Trying to create another instance of Main controller handler!");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
            JoinAction.Enable();
            JoinAction.performed += MainControllerJoinHandler;
        }
    }

    /// <summary>
    /// Tries to bind the main controller when the join input is pressed on a controller.
    /// </summary>
    /// <param name="context">Context containing the inputs.</param>
    private void MainControllerJoinHandler(InputAction.CallbackContext context)
    {
        if (_mainController != null)
        {
            Debug.Log("Main controller has already been binded!", this);
            return;
        }
        MainController newMainController = Instantiate(_mainControllerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
        newMainController.transform.SetParent(transform);
        newMainController.name = "Main Controller";
        newMainController.BindWithDevice(context.control.device);
        _mainController = newMainController;
        OnMainControllerJoinedEvent?.Invoke();
    }

    /// <summary>
    /// Sets if the main controller is active or inactive.
    /// </summary>
    /// <param name="isActive">Bollean active or inactive.</param>
    public void SetMainControllerActivity(bool isActive)
    {
        _mainController.gameObject.SetActive(isActive);
    }

    /// <summary>
    /// Returns the main controller.
    /// </summary>
    public MainController GetMainController()
    {
        return _mainController;
    }

    //OnEnable is called when the object becomes enabled and active.
    private void OnEnable()
    {
        if (JoinAction != null) JoinAction.Enable();
    }

    //OnEnable is called when the object becomes disabled and inactive.
    private void OnDisable()
    {
        JoinAction.Disable();
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        JoinAction.performed -= MainControllerJoinHandler;
    }
}
