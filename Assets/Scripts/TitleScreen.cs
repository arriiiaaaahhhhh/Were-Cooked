using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    [SerializeField] GameController gameController;

    private void Update()
    {
        // switch scene to gameplay when any key is pressed
        if(Input.anyKey)
        {
            gameController.setState(1);
        }
    }
}
