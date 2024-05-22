using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class PlayerController : MonoBehaviour
{

    [SerializeField, Tooltip("Player movement speed.")] private float _playerSpeed;
    [SerializeField, Tooltip("Player rotation speed.")] private float _playerRotationSpeed;

    [SerializeField, Tooltip("Speed at which the player slows down after they stop moving.")] private float _slowDownSpeed = .1f;
    [SerializeField, Tooltip("Speed at which the player speeds up after they start moving.")] float _speedUpRate = .1f;

    [SerializeField, Tooltip("Player index.")] public int PlayerIndex;
    [SerializeField, Tooltip("Player ready state.")] public bool IsReady;
    [SerializeField, Tooltip("Player spawn position.")] public Vector3 OriginPosition;
    [SerializeField, Tooltip("Current camera taken into consideration.")] public Camera CurrentCamera;
    [SerializeField, Tooltip("Player current velocity.")] private Vector3 velocity;
    [SerializeField, Tooltip("Player movement direction.")] private Vector3 _movementDirection;
    [SerializeField, Tooltip("Player facing direction.")] private Vector3 _facingDirection;

    [SerializeField, Tooltip("Is the player doing a task.")] private bool _isOccupied;
    [SerializeField, Tooltip("Is the player grounded.")] private bool _isGrounded;

    [SerializeField, Tooltip("Is the player pressing the interact button.")] private bool _isPressingInteract;
    [SerializeField, Tooltip("Is the player pressing the interact button.")] private TaskHandler _currentTask;

    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private TrailRenderer _trailRenderer;

    [SerializeField] private Animator _animator;

    //Player Inputs & Controls
    private Vector2 movementInput;
    private PlayerControls playerControls;
    private InputUser inputUser;

    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        IsReady = false;
        OriginPosition = transform.position;
    }

    private void Update()
    {
        if (_isOccupied)
        {
            return;
        }
        if (_movementDirection != Vector3.zero)
        {

            //Animation Walk
            if (_animator != null)
            {
                _animator.SetBool("IsMoving", true);
            }

        }
        else
        {
            if (_animator != null)
            {
                //Animation Idle 
                _animator.SetBool("IsMoving", false);
            }

        }
    }

    // Update is called once per frame.
    private void FixedUpdate()
    {
        if (_isOccupied)
        {
            return;
        }
        _movementDirection = new Vector3(movementInput.x, 0, movementInput.y);
        _movementDirection = Quaternion.Euler(0, CurrentCamera.transform.eulerAngles.y, 0) * _movementDirection;
        _movementDirection.Normalize();
        _facingDirection = _movementDirection;

        if (_movementDirection != Vector3.zero)
        {
            if (_isGrounded)
            {
                velocity = Vector3.Lerp(velocity, _movementDirection * _playerSpeed, _speedUpRate);
            }
            else
            {
                _movementDirection = new Vector3(_movementDirection.x, -1, _movementDirection.z);
                velocity = Vector3.Lerp(velocity, _movementDirection * _playerSpeed, _speedUpRate);
            }
            Quaternion toRotation = Quaternion.LookRotation(_facingDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, _playerRotationSpeed);

            _playerRigidbody.AddForce(velocity, ForceMode.Impulse);
            if (_playerRigidbody.velocity.magnitude > _playerSpeed) _playerRigidbody.velocity = _playerRigidbody.velocity.normalized * _playerSpeed;


        }
        else
        {
            if (_isGrounded)
            {
                velocity = Vector3.Lerp(velocity, Vector3.zero, _slowDownSpeed);
            }
            else
            {
                velocity = Vector3.Lerp(velocity, new Vector3(0, -10, 0), _slowDownSpeed);
            }
            
        }

    }

    /// <summary>
    /// Executes actions when the player moves.
    /// </summary>
    /// <param name="context">Context containing the inputs.</param>
    private void OnMove(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Executes actions when the player presses interact.
    /// </summary>
    /// <param name="context">Context containing the inputs.</param>
    private void OnInteract(InputAction.CallbackContext context)
    {
        _isPressingInteract = true;
    }

    /// <summary>
    /// Executes actions when the player unpresses grab.
    /// </summary>
    /// <param name="context">Context containing the inputs.</param>
    private void OnInteractCanceled(InputAction.CallbackContext context)
    {
        _isPressingInteract = false;
    }

    /// <summary>
    /// Executes actions when the player unpresses grab.
    /// </summary>
    /// <param name="context">Context containing the inputs.</param>
    private void OnTaskCanceled(InputAction.CallbackContext context)
    {
        if (_isOccupied && _currentTask != null)
        {
            _isOccupied = false;
            _currentTask.SetPlayer(null);
            _currentTask.IsTaskRunning = false;
            Debug.Log("Canceled task");
            _currentTask = null;
        }
    }

    //OnEnable is called when the object becomes enabled and active.
    private void OnEnable()
    {
        if (playerControls != null) playerControls.Enable();
    }

    //OnDisable is called when the object becomes disabled and inactive.
    private void OnDisable()
    {
        playerControls.Disable();
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        playerControls.Gameplay.Move.performed -= OnMove;
        playerControls.Gameplay.Move.canceled -= OnMove;
        playerControls.Gameplay.Interact.performed -= OnInteract;
        playerControls.Gameplay.Interact.canceled -= OnInteractCanceled;
        playerControls.Gameplay.Cancel.performed -= OnTaskCanceled;
    }

    /// <summary>
    /// Binds the player with the given device.
    /// </summary>
    /// <param name="device">A given controller.</param>
    public void BindWithDevice(InputDevice device)
    {
        inputUser = InputUser.PerformPairingWithDevice(device);
        playerControls = new PlayerControls();
        inputUser.AssociateActionsWithUser(playerControls);
        playerControls.Gameplay.Move.performed += OnMove;
        playerControls.Gameplay.Move.canceled += OnMove;
        playerControls.Gameplay.Interact.performed += OnInteract;
        playerControls.Gameplay.Interact.canceled += OnInteractCanceled;
        playerControls.Gameplay.Cancel.performed += OnTaskCanceled;
        playerControls.Enable();
        Debug.Log($"Bind with device : {device.path}!");
    }

    /// <summary>
    /// Unbinds the player with the player's device.
    /// </summary>
    public void UnbindWithDevice()
    {
        inputUser.UnpairDevicesAndRemoveUser();
        playerControls.Gameplay.Move.performed -= OnMove;
        playerControls.Gameplay.Move.canceled -= OnMove;
        playerControls.Gameplay.Interact.performed -= OnInteract;
        playerControls.Gameplay.Interact.canceled -= OnInteractCanceled;
        playerControls.Gameplay.Cancel.performed -= OnTaskCanceled;
        playerControls.Disable();
        playerControls.Dispose();
    }

    /// <summary>
    /// Gets the player device.
    /// </summary>
    public string GetDevicePath()
    {
        return inputUser.pairedDevices[0].path;
    }

    /// <summary>
    /// Gets the player controls.
    /// </summary>
    public PlayerControls GetPlayerControls()
    {
        return playerControls;
    }

    /// <summary>
    /// Set's the player occupied status.
    /// </summary>
    public void SetPlayerOccupied(bool isOccupied)
    {
        _isOccupied = isOccupied;
    }

    //OnCollisionStay is called once per frame for every Collider or Rigidbody that touches another Collider or Rigidbody.
    private void OnCollisionStay(Collision collision)
    {
        //Player grounded
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = true;
            _playerRigidbody.drag = 20;
        }
        //Player next to an object they can interact with
        if (collision.gameObject.CompareTag("InteractableObject") && _isPressingInteract && !_isOccupied)
        {
            _currentTask = collision.gameObject.GetComponent<TaskHandler>();
            if (_currentTask.IsTaskRunning || !_currentTask.GetTaskAvailability())
            {
                _currentTask = null;
                return;
            }
            else
            {
                _isOccupied = true;
                if (_animator != null)
                {
                    //Animation Idle 
                    _animator.SetBool("IsMoving", false);
                }
                _currentTask = collision.gameObject.GetComponent<TaskHandler>();
                _currentTask.SetPlayer(this);
                StartCoroutine(_currentTask.RunTask());
            }
            
        }
    }

    //OnCollisionStay is called once per frame for every Collider or Rigidbody that touches another Collider or Rigidbody.
    private void OnCollisionExit(Collision collision)
    {
        //Player grounded
        if (collision.gameObject.layer == 3)
        {
            _isGrounded = false;
            _playerRigidbody.drag = 0.1f;
        }
    }
}
