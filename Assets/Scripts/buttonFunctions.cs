using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        Application.Quit();
    }
    public void settings()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.activeMenu = gameManager.instance.settingsMenu;
        gameManager.instance.activeMenu.SetActive(true);

    }
    public void startGame()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.startGame = true;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gameManager.instance.closeMainMenu();
    }
    public void cancel()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.Confirm = false;
        gameManager.instance.cancel();
    }
    public void exitSettings()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.settingsMenu.SetActive(false);
        if (gameManager.instance.savedMenu == gameManager.instance.titleScreen)
        {
            gameManager.instance.backToTitle();

        }
        else if (gameManager.instance.savedMenu == gameManager.instance.pauseMenu)
        {
            gameManager.instance.backToPause();

        }
    }
    public void Credits()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.activeMenu = gameManager.instance.CreditScreenPage1;
        gameManager.instance.activeMenu.SetActive(true);
    }
    public void exitCredits()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.CreditScreenPage1.SetActive(false);
        gameManager.instance.CreditScreenPage2.SetActive(false);
        gameManager.instance.CreditScreenPage3.SetActive(false);
        gameManager.instance.activeMenu = gameManager.instance.titleScreen;
        gameManager.instance.activeMenu.SetActive(true);


    }
    public void nextPage2()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.activeMenu = gameManager.instance.CreditScreenPage2;
        gameManager.instance.activeMenu.SetActive(true);
    }
    public void nextPage3()
    {
        gameManager.instance.playerScript.getPlayerAud().PlayOneShot(clickSound);
        gameManager.instance.activeMenu = gameManager.instance.CreditScreenPage3;
        gameManager.instance.activeMenu.SetActive(true);
    }
}
