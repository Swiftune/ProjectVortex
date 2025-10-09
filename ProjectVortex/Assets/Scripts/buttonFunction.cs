using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void startGame()
    {
        GameManager.instance.stateUnpause();
    }

    public void getMainMenu()
    {
        GameManager.instance.stateUnpause(); 
        GameManager.instance.stateMain(); 
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