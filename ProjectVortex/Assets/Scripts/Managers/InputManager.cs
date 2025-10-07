using UnityEngine;

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
        pInput.Enable();
        pInput.UI.Pause.performed += Pause_performed;
        pInput.Combat.Shoot.performed += Shoot_performed;

        player = GameManager.instance.player.GetComponent<playerController>();
    }

    private void Shoot_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
    {
        player.ShootEvent();
    }

    private void Pause_performed(UnityEngine.InputSystem.InputAction.CallbackContext obj)
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
