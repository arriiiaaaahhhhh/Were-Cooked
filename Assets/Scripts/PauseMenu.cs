using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenu;
    public void Pause() 
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }

    public void Home() 
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }

    public void CloseButton() 
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }


}
