using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("------Player Things------")]
    public GameObject player;
    public playerController scriptPlayer;

    [Header("------UI------")]

    public GameObject pause;
    public GameObject activeMenu;
    public GameObject win;
    public GameObject lose;

    [Header("------Extras------")]
    [SerializeField] int roomCount;

    public int enemyCount;
    public bool paused;


    float origTime;
    void Awake()
    {
        instance = this;
        origTime = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            paused = !paused;
            activeMenu = pause;
            activeMenu.SetActive(paused);
            if (paused)
            {
                gamePaused();
            }
            else
                gameUnpaused();
        }
    }
    public void gamePaused()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
    public void gameUnpaused()
    {
        Time.timeScale = origTime;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;

    }
    public void numbOfEnemies(int amount)
    {
        enemyCount += amount;
        
        if(enemyCount <= 0)
        {

        }
    }
}
