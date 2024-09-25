using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuOption : MonoBehaviour
{
    [SerializeField] GameObject mainMenuOption;
    [SerializeField] GameObject joinLobby;
    public void CreateLobby() 
    {
        SceneManager.LoadScene("Lobby"); //Need to creat this scene but for now it will not work
    }

    public void JoinLobby() 
    {
        mainMenuOption.SetActive(false);            // Hide the Main Menu
        joinLobby.SetActive(true);
    }

    public void BackToMainMenu()
    {
        joinLobby.SetActive(false);           // Hide the Join Lobby
        mainMenuOption.SetActive(true);             // Show the Main Menu
    }

    public void Help() 
    {
        SceneManager.LoadScene("Help");
    }
    public void Setting() 
    {
        SceneManager.LoadScene("Settings");
    }
}
