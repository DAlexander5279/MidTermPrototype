using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;



public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("------Player Things------")]
    public GameObject player;
    public playerController playerScript;

    public int maxRoomsCleared;
    public int enemiesKilled;

    [Header("------UI------")]

    public GameObject pauseMenu;
    public GameObject activeMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject playerFlashDamage;
    public Image playerHealthBar;
    [SerializeField] TextMeshProUGUI Zoins; // its coins!

    [Header("------Extras------")]
    public int roomCount;
    public int curRoomIndex;
    [SerializeField] int killWinCondition;
    public bool roomsNeedPushed;

    public int enemyCount;
    public bool paused;

    public int zoins;

    float origTime;
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        origTime = Time.timeScale;
        PlayerPrefs.SetInt("enemyStat", 0);
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && (activeMenu == null || activeMenu == pauseMenu))
        {
            paused = !paused;
            activeMenu = pauseMenu;
            activeMenu.SetActive(paused);
            if (paused)
            {
                gamePause();
            }
            else
                gameUnpause();
        }

        //if(enemyCount <= 0)
        //{
        //    StartCoroutine(PushPlayer());
        //}
    }
    public void gamePause()
    {
        Time.timeScale = 0;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

    }
    public void gameUnpause()
    {
        Time.timeScale = origTime;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        activeMenu.SetActive(false);
        activeMenu = null;

    }
    public void updateEnemyCount(int amount)
    {
        enemyCount += amount;

        if ((enemyCount <= 0) && (enemiesKilled >= killWinCondition - 1))
        // we subtract 1 from killWinCondition to get around enemyCount updating when killing the exact amount needed AND it is the last enemy alive
        {
            winMenu.SetActive(true);
            gamePause();
            activeMenu = winMenu;
        }
    }

    public void pushRoomsBack(GameObject obj)
    {


        //Debug.Log(obj.transform.position);
        roomsNeedPushed = false;
        obj.transform.position = new Vector3(obj.transform.position.x - 25, 0f, 0f);
       
       
    }
    public void addZoins(int amount)
    {
        zoins += amount;
        Zoins.text = zoins.ToString("F0");
    }
}
