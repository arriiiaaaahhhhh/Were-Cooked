using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    private int gameState = 0;
    private static int gameScore = 0;
    private GUIStyle customStyle;
    private Texture2D grayTexture;
    
    private void Awake()
    {
        // persistence between scenes
        // check for gamecontrollers, if there is more than one then destroy self otherwise restarting will create duplicates
        GameObject[] gameControllers = GameObject.FindGameObjectsWithTag("gamecontroller");

        if(gameControllers.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(gameObject);

        customStyle = new GUIStyle();
        customStyle.normal.textColor = Color.black;
        customStyle.fontStyle = FontStyle.Bold;
        grayTexture = new Texture2D(1, 1);
        grayTexture.SetPixel(0, 0, new Color(0.5f, 0.5f, 0.5f, 1f)); // RGB(128, 128, 128) with 50% opacity
        grayTexture.Apply();
    }

    // return state
    public int getState()
    {
        return gameState;
    }

    // set game state
    // loads scene attached to state
    // for example title=0, gameplay=1
    public void setState(int state)
    {
        gameState = state;

        SceneManager.LoadScene(gameState);
    }

    private void Update()
    {
        // restart game if r is pressed
        if (Input.GetKey(KeyCode.R))
        {
            setState(0);
        }
    }

    public static void addScore() {
        gameScore++;
    }

    void OnGUI() {
        // only draw score during gameplay
        if(gameState == 1) {
            Rect labelRect = new Rect(800, 10, 200, 300);
            Rect boxRect = new Rect(790, 7, 100, 20);
            String scoreText = "Score: " + gameScore;
            GUI.DrawTexture(boxRect, grayTexture);
            GUI.Label(labelRect, scoreText, customStyle);
        }
    }
}
