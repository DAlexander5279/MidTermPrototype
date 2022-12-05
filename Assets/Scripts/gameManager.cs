using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("------Player Things------")]
    public GameObject player;
    public playerController playerScript;

    [Header("------UI------")]

    public GameObject pauseMenu;
    public GameObject activeMenu;
    public GameObject winMenu;
    public GameObject loseMenu;

    [Header("------Extras------")]
    [SerializeField] int roomCount;
    public bool roomsNeedPushed;
    public int enemyCount;
    public bool paused;


    float origTime;
    void Awake()
    {
        instance = this;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        origTime = Time.timeScale;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            paused = !paused;
            activeMenu = pauseMenu;
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
    public void updateEnemyCount(int amount)
    {
        enemyCount += amount;
        
        // might not need code below if going off of room count
        if(enemyCount <= 0)
        {

        }
    }

    public void pushRoomsBack(GameObject obj)
    {
        Debug.Log(obj.transform.position);
        roomsNeedPushed = false;
        obj.transform.position = new Vector3(obj.transform.position.x - 25, 0f, 0f);
    }

    public void pushPlayerBack()
    {
        player.transform.position = new Vector3(player.transform.position.x - 25, 0f, 0f);
    }
}
