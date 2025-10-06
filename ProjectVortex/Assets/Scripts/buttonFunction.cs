using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonFunction : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void resume()
    {
<<<<<<< Updated upstream
        GameManager.instance.stateUnpause();
=======
        GameManager.instance.stateUnpause(); 
>>>>>>> Stashed changes
    }

    public void restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
<<<<<<< Updated upstream
        GameManager.instance.stateUnpasue();
    }
    public void quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit(); 
#endif
=======
        GameManager.instance.stateUnpause();
    }
    public void quit()
    {
    #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
    #else
                Application.Quit(); 
    #endif
>>>>>>> Stashed changes
    }
}