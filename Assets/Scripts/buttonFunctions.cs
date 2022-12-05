using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.gameUnpaused();
        gameManager.instance.paused = !gameManager.instance.paused;
    }
    public void restart()
    {
        gameManager.instance.gameUnpaused();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void quit()
    {
        Application.Quit();
    }
    

}
