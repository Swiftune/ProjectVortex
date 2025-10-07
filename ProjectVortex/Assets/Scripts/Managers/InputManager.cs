using UnityEngine;
using UnityEngine.Windows;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    PlayerInput pInput;
    playerController player;

    public Vector2 Movement => pInput.Movement.Move.ReadValue<Vector2>();

    private void Awake()
    {
        Instance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pInput = new PlayerInput();
        // Enable all input maps to start with
        pInput.Enable();

        // Subscribe a method to the UI's Pause' Event Key being "started"
        pInput.UI.Pause.started += Pause_Started;
        pInput.Combat.Shoot.performed += Shoot_performed;

        player = GameManager.instance.player.GetComponent<playerController>();
    }

    private void OnDestroy()
    {
        // Disable input to avoid memory leaks
        pInput.Disable();

        // UNSUBSCRIBE a method to the UI's Pause' Event Key being "started"
        pInput.UI.Pause.started -= Pause_Started; // this will be called when the event is hit
        pInput.Combat.Shoot.performed -= Shoot_performed;
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.ShootEvent();
    }

    private void Pause_Started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PauseEvent();
        Debug.Log("Pause event ran");
    }

    public void PauseEvent()
    {
        Debug.Log(pInput.UI.Pause.WasPressedThisFrame());

        if (GameManager.instance.isPaused)
        {
            GameManager.instance.stateUnpause();
            pInput.Combat.Enable();
        }
        else
        {
            Debug.Log("Paused?");
            GameManager.instance.statePause();
            pInput.Combat.Disable();
        }
    }


}
