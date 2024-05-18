using UnityEngine;
using UnityEngine.SceneManagement;

public class LaunchHandler : MonoBehaviour
{
    //Awake is called when an enabled script instance is being loaded, before the application starts.
    private void Awake()
    {
        MainControllerHandler.OnMainControllerJoinedEvent += OnMainControllerJoined;
    }

    /// <summary>
    /// Loads the main menu.
    /// </summary>
    private void OnMainControllerJoined()
    {
        SceneManager.LoadScene("PlayerJoinSetup");
    }

    //OnDestroy is called when Destroying the attached Behaviour. It will only be called on game objects that have previously been active.
    private void OnDestroy()
    {
        MainControllerHandler.OnMainControllerJoinedEvent -= OnMainControllerJoined;
    }
}
