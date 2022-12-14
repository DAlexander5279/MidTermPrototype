//using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class playerController : MonoBehaviour
{

    [Header("------Components------")]
    [SerializeField] CharacterController playerControl;

    // Player Stats
    #region
    [Header("------Player Stats------")]
    [Range(1, 10)][SerializeField] float playerSpeed;

    [SerializeField] int jumpMax;
    [Range(5, 15)][SerializeField] int jumpHeight;
    [Range(10, 20)][SerializeField] int gravity;
    [SerializeField] int pushTime;
    public int HP;
    public bool isDisabled;
    #endregion


    //keyBinds 
    #region  

    [Header("------KeyBinds------")]
    [SerializeField] KeyCode Reload;
    [SerializeField] KeyCode crouchCBind;

    [SerializeField] KeyCode crouch;
    #endregion

    // look to move to gameManager in the future + Reload.ToString()
    // do the same for the GrenadeThrow UI
    public GameObject reloadUI;

    //player Sounds
    #region
    [Header("------Player Sounds------")]
    [SerializeField] AudioSource aud;

    //gun sounds
    [SerializeField] AudioClip gunshotSound;
    [SerializeField] List<AudioClip> dryfireSound;
    [SerializeField] AudioClip gunReloadSound;
    [Range(0, 3)][SerializeField] float gunshotSoundVol;

    [SerializeField] AudioClip[] playerJumpAudio;
    [Range(0, 3)][SerializeField] float playerJumpAudioVol;

    [SerializeField] AudioClip[] playerHurtAudio;
    [Range(0, 3)][SerializeField] float playerHurtAudioVol;

    [SerializeField] AudioClip[] playerStepAudio;
    [Range(0, 3)][SerializeField] float playerStepAudioVol;
    #endregion

    // gun stats
    #region
    [Header("------Gun Stats------")]
    [SerializeField] List<gunStats> gunList = new List<gunStats>();

    [Range(0, 5)][SerializeField] int gunDMG;
    [SerializeField] float shootRate;   // player's gun fire rate
    [Range(0, 200)][SerializeField] int shootDist; // effective range of the shot
    [SerializeField] GameObject gunModel;   //also gun position/viewmodel position
    [SerializeField] GameObject hitEffect;
    [SerializeField] int fireSelect;
    [SerializeField] int pellets;
    [SerializeField] float spreadAccuracy;
    [SerializeField] bool hasScope;
    #endregion

    // extra variables
    #region
    bool isShooting;
    private Vector3 playerVelocity;
    Vector3 move;
    private Vector3 playerCrouch;
    Vector3 moveCrouch;
    int timesJumped;
    public int HPOriginal;
    int walkSpeedOrg;

    int selectedGun;

    int magSizeOrg;
    bool isReloading;
    bool audioIsPlaying;
    bool isRunning;
    Vector3 pushBack;
    #endregion

    //  crouch/prone stuff
    #region
    [Header("------Stances------")]
    public float orgPlayerSpeed;
    [Range(0, 1)][SerializeField] float crouchHeight;
    [SerializeField] float crouchSpeed;
    [Range(0, 1)][SerializeField] float proneHeight;
    [SerializeField] float proneSpeed;
    private float timePressed = 0;
    public float waitTimeReq = 1f;
    private float timeReset = 0;
    private bool isPressed = false;
    private bool isProned = false;
    private bool isCrouched = false;
    private bool isStanding = true;
    private float startStance;
    #endregion

    // Start is called before the first frame update

    private void Start()
    {
        HPOriginal = HP;
        startStance = transform.localScale.y;
        orgPlayerSpeed = playerSpeed;
        updatePlayerHP();
        gameManager.instance.updatePlayerDamage(gunDMG);

        isStanding = true;
        isCrouched = false;
        isProned = false;
        isReloading = false;

    }
    // Update is called once per frame
    void Update()
    {
        if (!isDisabled)    // makes sure to not allow player to move when teleporting
        {

            if (!gameManager.instance.paused)
            {
                pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushTime);

                Movement();

                if (!audioIsPlaying && move.magnitude > 0.3f && playerControl.isGrounded)
                {
                    StartCoroutine(stepsPlaying());
                }
                if (gunList.Count > 0)
                {
                    StartCoroutine(Shoot());
                    gunSelect();
                }
            }

        }
        if (Input.GetKey(crouch) && isPressed == false)
        {
            timeReset += Time.deltaTime;
        }
        if (Input.GetKey(crouch) || isPressed == true)
        {
            timeReset = 0;
        }
        if (Input.GetKey(crouch))
        {
            timePressed += Time.deltaTime;
        }
        if (timeReset >= waitTimeReq && isPressed == false)
        {
            isPressed = true;
        }
        if (Input.GetKeyUp(crouch))
        {
            isPressed = false;
        }

    }
    void Movement()
    {

        if (playerControl.isGrounded && playerVelocity.y < 0)
        {
            timesJumped = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        playerControl.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpMax)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;

            aud.PlayOneShot(playerJumpAudio[UnityEngine.Random.Range(0, playerJumpAudio.Length)], playerJumpAudioVol);
        }

        if (timePressed > waitTimeReq)
        {
            if (Input.GetKeyUp(crouch))
            {
                isProned = !isProned;
                isCrouched = false;
                transform.localScale = new Vector3(transform.localScale.x, proneHeight, transform.localScale.z);
                playerSpeed = proneSpeed;

            }

        }
        if (Input.GetKeyUp(crouch) && timePressed < waitTimeReq)
        {
            isCrouched = !isCrouched;
            isProned = false;
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
            playerSpeed = proneSpeed;

        }
        if (Input.GetKeyUp(crouchCBind) && timePressed < waitTimeReq)
        {
            isCrouched = !isCrouched;
            isProned = false;
            transform.localScale = new Vector3(transform.localScale.x, crouchHeight, transform.localScale.z);
            playerSpeed = proneSpeed;

        }
        if (isCrouched == false && isProned == false)
        {
            isStanding = true;
            isCrouched = false;
            isProned = false;
            transform.localScale = new Vector3(transform.localScale.x, startStance, transform.localScale.z);
            playerSpeed = orgPlayerSpeed;

        }
        else
        {
            isStanding = false;
        }
        if (Input.GetKeyUp(crouch))
        {
            timePressed = 0;
        }




        playerVelocity.y -= gravity * Time.deltaTime;
        playerControl.Move(playerVelocity * Time.deltaTime);
        playerControl.Move((playerVelocity + pushBack) * Time.deltaTime);


    }

    IEnumerator Shoot()
    {
        if (!isShooting && gunList[selectedGun].magCount > 0 &&
            ((fireSelect == 0 && Input.GetButton("Shoot")) || (fireSelect == 1 && Input.GetButtonDown("Shoot"))))//GetButton = full-auto | ...Down = semi-auto | ..Up = think single-action revolver) - J
        {
            RaycastHit hit;
            isShooting = true;
            gunList[selectedGun].magCount--;
            gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize);

            switch (pellets)
            {
                case 1:
                    //if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                    if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f * Random.Range(spreadAccuracy, 1.0f), 0.5f * Random.Range(spreadAccuracy, 1.0f))), out hit, shootDist))
                    {
                        if (hit.collider.GetComponent<IDamage>() != null)
                        {
                            hit.collider.GetComponent<IDamage>().takeDamage(gunDMG);
                        }

                        Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
                    }
                    break;
                default:
                    //int numberOfHits = 0;
                    //Collider enemyHit = null;
                    for (int i = 0; i < pellets; i++)
                    {
                        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f * Random.Range(spreadAccuracy, 1.0f), 0.5f * Random.Range(spreadAccuracy, 1.0f))), out hit, shootDist))
                        {
                            if (hit.collider.GetComponent<IDamage>() != null)
                            {
                                //enemyHit = hit.collider;
                                //numberOfHits++;
                                hit.collider.GetComponent<IDamage>().takeDamage(gunDMG);
                            }

                            Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
                        }
                    }
                    //enemyHit.GetComponent<IDamage>().takeDamage(gunDMG * numberOfHits);
                    break;
            }

            aud.PlayOneShot(gunshotSound, gunshotSoundVol);

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }

        if (gunList[selectedGun].magCount < gunList[selectedGun].magSize)
        {
            if (gunList[selectedGun].magCount == 0 || gunList[selectedGun].magCount <= 3)
            {
                //gameManager.instance.activeMenu = reloadUI;
                reloadUI.SetActive(true);
            }
            if (Input.GetKeyDown(Reload) && !isReloading)
            {
                isReloading = true;
                aud.PlayOneShot(gunReloadSound, gunshotSoundVol);
                yield return new WaitForSeconds(0.5f);
                gunList[selectedGun].magCount = gunList[selectedGun].magSize;
                gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize);
                reloadUI.SetActive(false);
                isReloading = false;
            }
            if (Input.GetButtonDown("Shoot") && gunList[selectedGun].magCount <= 0)
            {
                aud.PlayOneShot(dryfireSound[UnityEngine.Random.Range(0, dryfireSound.Count - 1)], gunshotSoundVol);

                //yield return new WaitForSeconds(shootRate * 2.0f);
            }
        }
    }
    public void takeDamage(int dmgIn)
    {
        HP -= gameManager.instance.scalingFunction(dmgIn);
        aud.PlayOneShot(playerHurtAudio[UnityEngine.Random.Range(0, playerHurtAudio.Length)], playerHurtAudioVol);
        updatePlayerHP();
        StartCoroutine(playerFlashDamage());

        if (HP <= 0)
        {
            aud.enabled = false;
            gameManager.instance.gamePause();
            gameManager.instance.loseMenu.SetActive(true);
            gameManager.instance.activeMenu = gameManager.instance.loseMenu;
            gameManager.instance.activeMenu = gameManager.instance.loseMenu;
        }
    }
    IEnumerator playerFlashDamage()
    {
        gameManager.instance.playerFlashDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerFlashDamage.SetActive(false);

    }
    IEnumerator stepsPlaying()
    {
        audioIsPlaying = true;

        aud.PlayOneShot(playerStepAudio[UnityEngine.Random.Range(0, playerStepAudio.Length)], playerStepAudioVol);
        if (isRunning)
        {
            yield return new WaitForSeconds(0.3f);


        }
        else
        {
            yield return new WaitForSeconds(0.5f);
        }

        audioIsPlaying = false;
    }
    public void gunPickup(gunStats gunStat)
    {

        bool foundGun = false;
        for (int i = 0; i < gunList.Count; i++)
        {
            if (gunStat == gunList[i])
            {
                foundGun = true;
                gunList[i].modifedGunDMG = Mathf.FloorToInt(gunList[i].modifedGunDMG * gameManager.instance.getDamageModifier());
                gunList[i].magCount = gunList[i].magSize;           // reloads the upgraded weapon automatically for faster gameplay
                if (gunList[i] == gunList[selectedGun])
                {
                    gunDMG = gunList[i].modifedGunDMG;
                    aud.PlayOneShot(gunReloadSound, gunshotSoundVol);
                    gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize);
                    reloadUI.SetActive(false);
                    gameManager.instance.updatePlayerDamage(gunDMG);
                }
            }
        }
        if (!foundGun)
        {
            gunStat.magCount = gunStat.magSize;
            gunStat.modifedGunDMG = gameManager.instance.scalingFunction(gunStat.gunDMG);
            gunList.Add(gunStat);
            selectedGun = gunList.Count - 1;
            changeCurrentGun();

            gameManager.instance.ammoUpdate(gunStat.magCount, gunList[selectedGun].magSize);
        }
        if (gameManager.instance.itemCount != 0)
            gameManager.instance.updateItemCount(-1);
    }

    void gunSelect()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun < gunList.Count - 1)  // scrolling up + staying within List's range
        {
            selectedGun++;
            changeCurrentGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun > 0)  // scrolling down + staying within List's range
        {
            selectedGun--;
            changeCurrentGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedGun == gunList.Count - 1)  // scrolling up + final gun in list...
        {
            selectedGun = 0;    // loop back to first gun in list
            changeCurrentGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedGun == 0)  // scrolling down + first gun in list...
        {
            selectedGun = gunList.Count - 1;    // loop forward to final gun in list
            changeCurrentGun();

        }
    }

    public void changeCurrentGun()
    {
        shootRate = gunList[selectedGun].shootRate;

        // gunDMG = gunList[selectedGun].gunDMG;
        gunDMG = gunList[selectedGun].modifedGunDMG;
        gameManager.instance.updatePlayerDamage(gunDMG);

        shootDist = gunList[selectedGun].shootDist;

        fireSelect = gunList[selectedGun].fireSelect;

        gunshotSound = gunList[selectedGun].gunshotSound;

        gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize);

        pellets = gunList[selectedGun].pellets;

        spreadAccuracy = gunList[selectedGun].spreadAccuracy;

        hasScope = gunList[selectedGun].hasScope;

        // transfer the gun's model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;

        //transfer the gun's textures/materials
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }

    public void changeCurrentGun(gunStats gunStat) // in case you want to manually give the player a gun
    {
        shootRate = gunStat.shootRate;
        gunDMG = gunStat.modifedGunDMG;
        gameManager.instance.updatePlayerDamage(gunDMG);
        shootDist = gunStat.shootDist;
        gunshotSound = gunStat.gunshotSound;
        fireSelect = gunStat.fireSelect;

        pellets = gunStat.pellets;
        spreadAccuracy = gunStat.spreadAccuracy;
        hasScope = gunStat.hasScope;

        // transfer the gun's model
        gunModel.GetComponent<MeshFilter>().sharedMesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;

        //transfer the gun's textures/materials
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunStat.gunModel.GetComponent<MeshRenderer>().sharedMaterial;
    }
    public void updatePlayerHP()
    {
        gameManager.instance.playerHealthBar.fillAmount = (float)HP / (float)HPOriginal;
    }
    public void inputPushBack(Vector3 dir)
    {
        pushBack = dir;
    }

    public bool getScopeStatus()
    {
        return hasScope;
    }

}

