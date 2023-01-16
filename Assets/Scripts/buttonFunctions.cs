using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    public void resume()
    {
        gameManager.instance.gameUnpause();
        gameManager.instance.paused = !gameManager.instance.paused;
        gameManager.instance.pauseMenu.SetActive(false);
    }
    public void restart()
    {
        gameManager.instance.gameUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void quit()
    {
        Application.Quit();
    }
    public void settings()
    {
        gameManager.instance.gamePause();
        gameManager.instance.activeMenu = gameManager.instance.settingsMenu;
        gameManager.instance.activeMenu.SetActive(gameManager.instance.settingsMenu);
        
    }
    

}
