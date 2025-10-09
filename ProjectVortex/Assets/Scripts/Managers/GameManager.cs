using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuMain;
    [SerializeField] Slider slider;

    public GameObject player;
    public playerController playerScript;

    public bool isPaused;

    float timeScaleOrig;
    public float OriginalTimeScale => timeScaleOrig;

    int gameGoalCount;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        timeScaleOrig = Time.timeScale;

        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();


    }

    private void Start()
    {
        Time.timeScale = timeScaleOrig;
    }

    // Update is called once per frame
    void Update()
    {
        checkHP();
    }

    public void statePause()
    {
        if (menuActive == null)
        {
            menuActive = menuPause;
            menuActive.SetActive(true);
        }

        isPaused = !isPaused;
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Time.timeScale = timeScaleOrig;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        menuActive.SetActive(false);
        menuActive = null;
    }

    public void UpdateGameGoal(int amount)
    {
        gameGoalCount += amount;

        if (gameGoalCount <= 0)
        {
            menuActive = menuWin;
            menuActive.SetActive(true);
            statePause();
        }
    }

    public void stateLose()
    {
        menuActive = menuLose;
        menuActive.SetActive(true);
        statePause();
    }

    public void checkHP()
    {
        if (playerScript.GetHPOrig() > 0)
        {
            slider.maxValue = playerScript.GetHPOrig();
            slider.minValue = 0;
        }
        slider.value = playerScript.GetHP();
    }

    public void stateMain()
    {
        menuActive = menuMain;
        menuActive.SetActive(true);
        statePause();
    }
}