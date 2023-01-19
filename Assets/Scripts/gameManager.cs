using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.SceneManagement;
using Unity.VisualScripting;

public class gameManager : MonoBehaviour
{
    public static gameManager instance;

    [Header("------Player Things------")]
    public GameObject player;
    public playerController playerScript;
    public cameraMovement Sensitivity;
    [Range(1.0f, 3.0f)][SerializeField] float damageModifier;
    [Range(0.0f, 3.0f)][SerializeField] float scalingModifer;

    public int maxRoomsCleared;
    public int enemiesKilled;

    [Header("------UI------")]
    public GameObject pauseMenu;
    public GameObject activeMenu;
    public GameObject shopMenu;
    public GameObject winMenu;
    public GameObject loseMenu;
    public GameObject titleScreen;
    public GameObject closeConfirmMenu;
    public GameObject playerFlashDamage;
    public Image playerHealthBar;
    public Image playerHealthAnim;
    public GameObject ammoHUD;
    public GameObject ammoHUDText;
    //public GameObject Reload;
    [SerializeField] TextMeshProUGUI Zoins; // its coins!
    [SerializeField] TextMeshProUGUI roomsCleared; // its rooms!

    [SerializeField] TextMeshProUGUI ammoCount;

    [SerializeField] TextMeshProUGUI playerDamage;

    [SerializeField] AudioMixer mixer;
    [SerializeField] Slider musicSliderVol;
    [SerializeField] Slider sfxSliderVol;
    [SerializeField] Slider SensX;
    [SerializeField] Slider SensY;
    public GameObject settingsMenu;


    [Header("------Extras------")]
    public int waveCount;
    public int itemCount;
    public int curRoomIndex;
    [SerializeField] int killWinCondition;
    public bool roomsNeedPushed;
    public int enemyCount;
    public bool paused;
    public bool settings;
    public bool startGame;
    public bool closeConMenu;
    public bool Confirm;
    public bool Cancel;
    public int zoins;
    public int AmmoCount;
    float HPTimer = 0;
    public bool shop;


    float origTime;

    void Awake()
    {


        instance = this;
        
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        origTime = Time.timeScale;
        PlayerPrefs.SetInt("enemyStat", 0);
        addZoins(0);
        waveCount = 0;
        if (PlayerPrefs.HasKey("MusicVol"))
        {
            musicSliderVol.value = PlayerPrefs.GetFloat("MusicVol");
            SetVolumeMixer("MusicVol", musicSliderVol.value);
        }
        if (PlayerPrefs.HasKey("SFXVol"))
        {
            sfxSliderVol.value = PlayerPrefs.GetFloat("SFXVol");
            SetVolumeMixer("SFXVol", sfxSliderVol.value);
        }
        if (PlayerPrefs.HasKey("SenXAxis"))
        {
            SensX.value = PlayerPrefs.GetFloat("SenXAxis");
            Sensitivity.SetSensX(SensX.value);
        }
        if (PlayerPrefs.HasKey("SenYAxis"))
        {
            SensY.value = PlayerPrefs.GetFloat("SenYAxis");
            Sensitivity.SetSensY(SensY.value);
        }
    }
    private void Start()
    {
       
        MainMenu();
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Cancel") && (activeMenu == null || activeMenu == pauseMenu))
        {
            paused = true;
            activeMenu = pauseMenu;
            activeMenu.SetActive(true);
            if (paused)
            {
                gamePause();
            }
            else
                gameUnpause();
        }

        if (Input.GetButtonDown("Cancel") && activeMenu == settingsMenu && startGame == true)
        {
            settings = false;
            activeMenu.SetActive(false);
            activeMenu = pauseMenu;
            paused = true;
            activeMenu.SetActive(true);
            

        }

        if (Input.GetButtonDown("Cancel") && activeMenu == titleScreen)
        {
            closeConMenu = true;
            activeMenu = closeConfirmMenu;
            activeMenu.SetActive(true);
            if (Confirm == false)
            {
                cancel();

            }
        }
       
        if (activeMenu == settingsMenu && startGame == false)
        {
            settings = true;
            if (Input.GetButtonDown("Cancel"))
            {
                activeMenu.SetActive(false);
                activeMenu = titleScreen;
                activeMenu.SetActive(true);
            }
        }
        if (Input.GetButtonDown("Cancel") && (activeMenu == null || activeMenu == shopMenu))
        {
            shop = !shop;
            shopMenu.SetActive(false);
            pauseMenu.SetActive(false);

            if (Time.timeScale == 0)
            {
                gameUnpause();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }

        }

        if (playerHealthAnim.fillAmount != playerHealthBar.fillAmount)
        {
            playerHealthAnim.fillAmount = Mathf.Lerp(playerHealthAnim.fillAmount, playerHealthBar.fillAmount, HPTimer);
            HPTimer += 0.25f * Time.deltaTime;
        }
        else { HPTimer = 0f; }

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
        pauseMenu.SetActive(false);
        activeMenu = null;

    }
    public void updateEnemyCount(int amount)
    {
        enemyCount += amount;


        //if ((enemyCount <= 0) && (enemiesKilled >= killWinCondition - 1))
        //// we subtract 1 from killWinCondition to get around enemyCount updating when killing the exact amount needed AND it is the last enemy alive
        //{
        //    winMenu.SetActive(true);
        //    gamePause();
        //    activeMenu = winMenu;
        //}
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

    public void addWaveCount(int amount)
    {
        waveCount += amount;
        if (waveCount < 0)
            roomsCleared.text = 0.ToString("F0");   //will rename to wavesCleared
        else
            roomsCleared.text = waveCount.ToString("F0");   //will rename to wavesCleared
    }
    public void ammoUpdate(int currentMag, int magSize, bool isMelee)
    {
        if (!isMelee)
        {
            //AmmoCount = amount;
            ammoHUD.SetActive(true);
            ammoHUDText.SetActive(true);
            ammoCount.text = currentMag.ToString("F0") + " / " + magSize.ToString("F0");
        }
        else if (isMelee)
        {
            ammoHUD.SetActive(false);
            ammoHUDText.SetActive(false);
        }
    }

    public float getDamageModifier()
    {
        return damageModifier;
    }

    public void updateItemCount(int amount)
    {
        itemCount += amount;
    }

    public void updatePlayerDamage(int damageStat)
    {
        playerDamage.text = damageStat.ToString("F0");
    }

    public int scalingFunction(int var)
    {
        return var + Mathf.FloorToInt(var * (scalingModifer * Mathf.Floor(waveCount * 0.2f)));
    }
    public void musicVolChange()
    {
        SetVolumeMixer("MusicVol", musicSliderVol.value);
        PlayerPrefs.SetFloat("MusicVol", musicSliderVol.value);
    }
    public void sfxVolChange()
    {
        SetVolumeMixer("SFXVol", sfxSliderVol.value);
        PlayerPrefs.SetFloat("SFXVol", sfxSliderVol.value);
    }
    void SetVolumeMixer(string key, float value)
    {
        mixer.SetFloat(key, Mathf.Log10(value) * 20);
    }
    public void setSensXChange()
    {
        Sensitivity.SetSensX(SensX.value);
        PlayerPrefs.SetFloat("SenXAxis", SensX.value);
    }
    public void setSensYChange()
    {
        Sensitivity.SetSensY(SensY.value);
        PlayerPrefs.SetFloat("SenYAxis", SensY.value);

    }
    public void confirm()
    {
        activeMenu = null;
        activeMenu.SetActive(false);
        Application.Quit();
    }
    public void cancel()
    {
        activeMenu.SetActive(false);
        activeMenu = titleScreen;
        activeMenu.SetActive(true);
    }
    public void MainMenu()
    {
        activeMenu = titleScreen;
        Time.timeScale = 0;
        activeMenu.SetActive(true);
    }
    public void closeMainMenu()
    {
        Time.timeScale = origTime;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        titleScreen.SetActive(false);
        activeMenu = null;
    }
}
