using UnityEngine;
using UnityEngine.UI;

public class JoinLobby : MonoBehaviour
{
    [SerializeField] InputField joinCode;

    public void InputGameCode()
    {
        string lobbyCode = joinCode.text;  // Get the text from the Input Field
        Debug.Log("Join Code: " + lobbyCode);        // You can now use this name (e.g., send it to the server)
    }

}
