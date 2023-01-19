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
        gameManager.instance.paused = false;
        gameManager.instance.pauseMenu.SetActive(false);
    }
    public void restart()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.gameUnpause();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void quit()
    {
        audioClick.PlayOneShot(clickSound, clickVol);
        Application.Quit();
    }
    public void settings()
    {
        audioClick.PlayOneShot(clickSound, clickVol);
        gameManager.instance.activeMenu = gameManager.instance.settingsMenu;
        gameManager.instance.activeMenu.SetActive(true);

    }
    public void startGame()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.startGame = true;
        gameManager.instance.closeMainMenu();
    }
    public void cancel()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.Confirm = false;
        gameManager.instance.cancel();
    }
}
