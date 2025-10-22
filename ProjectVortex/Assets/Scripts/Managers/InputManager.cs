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
        pInput.Combat.Shoot.canceled += Shoot_canceled;
        pInput.Combat.Grenade.performed += Grenade_performed;
        pInput.Combat.Grenade.canceled += Grenade_canceled;
        pInput.Movement.Jump.started += Jump_started;


        player = GameManager.instance.player.GetComponent<playerController>();
    }

    private void OnDestroy()
    {
        // Disable input to avoid memory leaks
        pInput.Disable();

        // UNSUBSCRIBE a method to the UI's Pause' Event Key being "started"
        pInput.UI.Pause.started -= Pause_Started; // this will be called when the event is hit
        pInput.Combat.Shoot.performed -= Shoot_performed;
        pInput.Combat.Shoot.canceled -= Shoot_canceled;
        pInput.Combat.Grenade.performed -= Grenade_performed;
        pInput.Combat.Grenade.canceled -= Grenade_canceled;
        pInput.Movement.Jump.performed -= Jump_started;
    }

    private void Grenade_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.grenadeEnabled = true;
    }
    private void Grenade_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.grenadeEnabled = false;
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.shootEnabled = true;
    }

    private void Shoot_canceled(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.shootEnabled = false;
    }

    private void Jump_started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.jump();
    }

    private void Pause_Started(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        PauseEvent();
    }

    public void PauseEvent()
    {
        if (GameManager.instance.isPaused)
        {
            GameManager.instance.stateUnpause();
            pInput.Combat.Enable();
        }
        else
        {
            GameManager.instance.statePause();
            pInput.Combat.Disable();
        }
    }


}
