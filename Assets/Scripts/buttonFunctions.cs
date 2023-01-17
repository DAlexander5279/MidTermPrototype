using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class buttonFunctions : MonoBehaviour
{
    [SerializeField] AudioSource audioClick;
    [Range(0, 3)][SerializeField] float clickVol;
    [SerializeField] AudioClip clickSound;
    public void resume()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);

        gameManager.instance.gameUnpause();
        gameManager.instance.paused = !gameManager.instance.paused;
        gameManager.instance.pauseMenu.SetActive(false);
    }
    public void restart()
    {
        gameManager.instance.gameUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
    }
    public void quit()
    {
        audioClick.PlayOneShot(clickSound, clickVol);

        Application.Quit();
    }
    public void settings()
    {
        audioClick.PlayOneShot(clickSound, clickVol);

        gameManager.instance.gamePause();
        gameManager.instance.activeMenu = gameManager.instance.settingsMenu;
        gameManager.instance.activeMenu.SetActive(gameManager.instance.settingsMenu);
        
    }
    

}
