using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void Resume()
    {
        gameManager.instance.gameUnpaused();
        gameManager.instance.paused = !gameManager.instance.paused;
    }
    public void Restart()
    {
        gameManager.instance.gameUnpaused();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Quit()
    {
        Application.Quit();
    }
    

}
