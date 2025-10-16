using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void startGame()
    {
        SceneManager.LoadScene("Nigel");
        GameManager.instance.stateUnpause();
        
    }

    public void getMainMenu()
    {
        GameManager.instance.statePause();
        SceneManager.LoadScene("startMenu");
    }

    public void resume()
    {
        InputManager.Instance.PauseEvent();
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        InputManager.Instance.PauseEvent();
    }


    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif

        GameManager.instance.stateUnpause();
    }

}