//using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class playerController : MonoBehaviour
{

    [Header("------Components------")]
    [SerializeField] CharacterController playerControl;


    // Player Stats
    #region
    [Header("------Player Stats------")]
    [Range(1, 20)] [SerializeField] float playerSpeed;
    [Range(1.0f, 5)] [SerializeField] float sprintMod;
    [SerializeField] int jumpMax;
    [Range(5, 15)] [SerializeField] int jumpHeight;
    [Range(10, 20)] [SerializeField] int gravity;
    [SerializeField] int pushTime;
    public int HP;
    public bool isDisabled;
    #endregion


    //keyBinds 
    #region  

    [Header("------KeyBinds------")]
    [SerializeField] KeyCode Reload;
    //stance keybinds
    [SerializeField] KeyCode crouch;
    [SerializeField] KeyCode crouchCBind;
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
    [Range(0, 3)] [SerializeField] float dryfireSoundVol;

    [SerializeField] AudioClip gunReloadSound;
    [Range(0, 3)] [SerializeField] float gunshotSoundVol;

    [SerializeField] AudioClip[] playerJumpAudio;
    [Range(0, 3)] [SerializeField] float playerJumpAudioVol;

    [SerializeField] AudioClip[] playerHurtAudio;
    [Range(0, 3)] [SerializeField] float playerHurtAudioVol;

    [SerializeField] AudioClip[] playerStepAudio;
    [Range(0, 3)] [SerializeField] float playerStepAudioVol;

    [SerializeField] AudioClip laserUpgradeSFX;
    [Range(0, 3)] [SerializeField] float laserUpgradeSFXVol;




    #endregion

    // gun stats
    #region
    [Header("------Gun Stats------")]
    //[SerializeField] List<gunStats> gunList = new List<gunStats>();
    public List<gunStats> gunList = new List<gunStats>();

    [Range(0, 5)] [SerializeField] int gunDMG;
    [SerializeField] float shootRate;   // player's gun fire rate
    [Range(0, 200)] [SerializeField] int shootDist; // effective range of the shot
    [SerializeField] GameObject gunModel;   //also gun position/viewmodel position
    [SerializeField] GameObject meleeModel;
    [SerializeField] GameObject hitEffect;
    [SerializeField] TrailRenderer playerBulletTracer;
    [SerializeField] GameObject muzzlePosition;
    [SerializeField] int fireSelect;
    [SerializeField] int pellets;
    [SerializeField] float spreadAccuracy;
    [SerializeField] bool hasScope;
    [SerializeField] float gunCriticalMult;
    [SerializeField] int weaponLvl;
    public bool isMeleeWeapon;
    public bool isGun;

    #endregion

    // extra variables
    #region
    bool isShooting;
    private Vector3 playerVelocity;
    Vector3 move;
    int timesJumped;
    public int HPOriginal;
    int walkSpeedOrg;
    public int CostOfWeapons;
    public int CostOfUpgrade;
    float playerSpeedOrig;
    bool isSprinting;

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
    //public float orgPlayerSpeed;
    //[SerializeField] float crouchHeight;
    //[SerializeField] float crouchSpeed;
    //[SerializeField] bool isCrouched;
    //[SerializeField] bool isStanding;
    //[SerializeField] float scaleX;
    //[SerializeField] float scaleY;
    //[SerializeField] float scaleZ;

    Vector3 playerCrouch;
    private float startStance;
    #endregion




    // Start is called before the first frame update
    private void Start()
    {
        gameManager.instance.player = GameObject.FindGameObjectWithTag("Player");
        gameManager.instance.playerScript = gameManager.instance.player.GetComponent<playerController>();
        HPOriginal = HP;
        playerSpeedOrig = playerSpeed;
        updatePlayerHP();
        gameManager.instance.updatePlayerDamage(gunDMG);
        isReloading = false;
        isSprinting = false;
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
                sprint();

                if (!audioIsPlaying && move.magnitude > 0.3f && playerControl.isGrounded)
                {
                    StartCoroutine(stepsPlaying());
                }
                if (gunList.Count > 0 && gameManager.instance.activeMenu != gameManager.instance.shopMenu)
                {
                    StartCoroutine(Shoot());
                    gunSelect();
                }
            }

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

        playerControl.Move(playerSpeed * Time.deltaTime * move);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpMax)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;

            aud.PlayOneShot(playerJumpAudio[UnityEngine.Random.Range(0, playerJumpAudio.Length)], playerJumpAudioVol);
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        playerControl.Move(playerVelocity * Time.deltaTime);
        playerControl.Move((playerVelocity + pushBack) * Time.deltaTime);




    }

    void sprint()
    {
        if (Input.GetButtonDown("Sprint"))
        {
            playerSpeed = playerSpeedOrig * sprintMod;
            isSprinting = true;
        }
        else if (Input.GetButtonUp("Sprint"))
        {
            playerSpeed = playerSpeedOrig;
            isSprinting = false;
        }
    }

    IEnumerator Shoot()
    {
        if (!isShooting && gunList[selectedGun].magCount > 0 && !isReloading &&
            ((fireSelect == 0 && Input.GetButton("Shoot")) || (fireSelect == 1 && Input.GetButtonDown("Shoot"))))//GetButton = full-auto | ...Down = semi-auto | ..Up = think single-action revolver) - J
        {
            RaycastHit hit;
            RaycastHit tracerHitInfo;
            isShooting = true;
            float accuracyFactor = (0.5f - (0.5f * spreadAccuracy)) * 0.5f; // divided in half so that total inaccuracy equals the spread
                                                                            // if not, then accuracy is actual twice as bad
            TrailRenderer tracer = null;

            if (!isMeleeWeapon)
            {
                gunList[selectedGun].magCount--;
                gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize, gunList[selectedGun].isMelee);
            }

            bool successfulHit;
            switch (pellets)
            {
                case 1:
                    if (muzzlePosition != null)
                    {
                        tracer = Instantiate(playerBulletTracer, muzzlePosition.transform.position, Quaternion.identity);
                        tracer.AddPosition(muzzlePosition.transform.position);
                    }
                    //if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDist))
                    successfulHit = Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor), Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor))), out tracerHitInfo, 2000);
                    if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor), Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor))), out hit, shootDist))
                    {

                        if (hit.collider.GetComponent<IDamage>() != null)
                        {
                            if (hit.collider is BoxCollider && !isMeleeWeapon)
                            {
                                hit.collider.GetComponent<IDamage>().takeDamage(gunDMG, true, gunCriticalMult);
                            }
                            else
                            {
                                hit.collider.GetComponent<IDamage>().takeDamage(gunDMG, false, 1.0f);
                                if (isMeleeWeapon)  // grant small healing for melee weapons (no overflow like health pickups)
                                {
                                    int healAmount = Mathf.FloorToInt(gunDMG * 0.25f);
                                    if (HP + healAmount >= HPOriginal)
                                    {
                                        HP = HPOriginal;
                                        updatePlayerHP();
                                    }
                                }
                            }
                        }

                        if (!hit.collider.CompareTag("Ceiling")) Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);


                    }
                    if (tracer != null) tracer.transform.position = tracerHitInfo.point;
                    break;
                default:
                    //int fullDamage = 0;
                    int criticalCounts = 0;
                    List<Collider> hitObjects = new List<Collider>();
                    List<int> hitObjectsDamage = new List<int>();
                    for (int i = 0; i < pellets; i++)
                    {
                        if (muzzlePosition != null)
                        {
                            tracer = Instantiate(playerBulletTracer, muzzlePosition.transform.position, Quaternion.identity);
                            tracer.AddPosition(muzzlePosition.transform.position);
                        }
                        successfulHit = Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor), Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor))), out tracerHitInfo, 2000);
                        if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor), Random.Range(0.5f - accuracyFactor, 0.5f + accuracyFactor))), out hit, shootDist))
                        {

                            if (hit.collider.GetComponent<IDamage>() != null)
                            {
                                if (!hitObjects.Contains(hit.collider))
                                {
                                    hitObjects.Add(hit.collider);
                                    hitObjectsDamage.Add(0);
                                }
                                if (hit.collider is BoxCollider && !isMeleeWeapon)    // melee should always have 1 pellet but extra check just in case
                                {
                                    //hit.collider.GetComponent<IDamage>().takeDamage(gunDMG, true, gunCriticalMult);
                                    hitObjectsDamage[hitObjects.IndexOf(hit.collider)] += Mathf.RoundToInt(gunDMG * gunCriticalMult);
                                    //fullDamage += Mathf.RoundToInt(gunDMG * gunCriticalMult);
                                    criticalCounts++;
                                }
                                else
                                {
                                    //hit.collider.GetComponent<IDamage>().takeDamage(gunDMG, false, 1.0f);
                                    hitObjectsDamage[hitObjects.IndexOf(hit.collider)] += gunDMG;
                                    //fullDamage += gunDMG;
                                }

                            }

                            if (!hit.collider.CompareTag("Ceiling")) Instantiate(hitEffect, hit.point, hitEffect.transform.rotation);
                        }
                        if (tracer != null) tracer.transform.position = tracerHitInfo.point;
                    }
                    if (criticalCounts == pellets)    // If all pellets hit a crit...
                    {
                        foreach (Collider enemy in hitObjects)
                        {
                            enemy.GetComponent<IDamage>().takeDamage(hitObjectsDamage[hitObjects.IndexOf(enemy)], true, 1.0f);    // CritMult is 1 since we did crit calculations in the loop
                                                                                                                                  // Will show a red number in damage text
                        }

                    }
                    else if (criticalCounts != pellets)// If some/no pellets hit a crit...
                    {
                        foreach (Collider enemy in hitObjects)
                        {
                            enemy.GetComponent<IDamage>().takeDamage(hitObjectsDamage[hitObjects.IndexOf(enemy)], false, 1.0f);    // CritMult is 1 since we did crit calculations in the loop
                                                                                                                                   // Will show a red number in damage text
                        }
                    }

                    hitObjectsDamage.Clear();
                    hitObjects.Clear();

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
                yield return new WaitForSeconds(0.8f);
                gunList[selectedGun].magCount = gunList[selectedGun].magSize;
                gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize, gunList[selectedGun].isMelee);
                reloadUI.SetActive(false);
                isReloading = false;
            }
            if (Input.GetButtonDown("Shoot") && gunList[selectedGun].magCount <= 0 && !isReloading)
            {
                aud.PlayOneShot(dryfireSound[UnityEngine.Random.Range(0, dryfireSound.Count - 1)], dryfireSoundVol);

                //yield return new WaitForSeconds(shootRate * 2.0f);
            }
        }
    }

    public void takeDamage(int dmgIn)
    {
        float damageReductionMult = 1.0f;
        if (isMeleeWeapon)  // melee weapons grant some damage reduction to be viable
        {
            damageReductionMult = 0.66f;
        }
        HP -= Mathf.CeilToInt(dmgIn * damageReductionMult);
        aud.PlayOneShot(playerHurtAudio[UnityEngine.Random.Range(0, playerHurtAudio.Length)], playerHurtAudioVol);
        updatePlayerHP();
        StartCoroutine(playerFlashDamage());

        if (HP <= 0)
        {
            aud.enabled = false;
            gameManager.instance.gamePause();
            gameManager.instance.shopMenu.SetActive(false);
            gameManager.instance.settingsMenu.SetActive(false);
            gameManager.instance.titleScreen.SetActive(false);
            gameManager.instance.CreditScreenPage1.SetActive(false);
            gameManager.instance.CreditScreenPage2.SetActive(false);
            gameManager.instance.CreditScreenPage3.SetActive(false);
            gameManager.instance.loseMenu.SetActive(true);
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
        if (gunList.Count > 0)
        {
            for (int i = 0; i < gunList.Count; i++)
            {
                if (gunStat == gunList[i])
                {
                    foundGun = true;
                    gunList[i].weaponLevel++;
                    gunList[i].modifedGunDMG = Mathf.FloorToInt(gunList[i].gunDMG * gunList[i].weaponLevel);
                    gunList[i].magCount = gunList[i].magSize;           // reloads the upgraded weapon automatically for faster gameplay
                    if (gunList[i] == gunList[selectedGun])
                    {
                        gunDMG = gunList[i].modifedGunDMG;
                        aud.PlayOneShot(gunReloadSound, gunshotSoundVol);
                        gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize, gunList[selectedGun].isMelee);
                        reloadUI.SetActive(false);
                        gameManager.instance.updatePlayerDamage(gunDMG);
                    }
                    aud.PlayOneShot(laserUpgradeSFX, laserUpgradeSFXVol);
                }
            }
        }
        if (!foundGun)
        {
            gunStat.magCount = gunStat.magSize;
            //if (gameManager.instance.waveCount >= 5)
            //{
            //    gunStat.modifedGunDMG = gameManager.instance.scalingFunction(gunStat.gunDMG);

            //}
            //else
            //{
            //    gunStat.modifedGunDMG = gunStat.gunDMG;
            //}
            gunStat.modifedGunDMG = gunStat.gunDMG;
            gunStat.weaponLevel = 1;
            gunList.Add(gunStat);
            selectedGun = gunList.Count - 1;
            changeCurrentGun();

            gameManager.instance.ammoUpdate(gunStat.magCount, gunStat.magSize, gunStat.isMelee);
        }
    }


    void gunSelect()
    {
        if (!gameManager.instance.paused)
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

        gameManager.instance.ammoUpdate(gunList[selectedGun].magCount, gunList[selectedGun].magSize, gunList[selectedGun].isMelee);

        pellets = gunList[selectedGun].pellets;

        spreadAccuracy = gunList[selectedGun].spreadAccuracy;
        hasScope = gunList[selectedGun].hasScope;
        gunCriticalMult = gunList[selectedGun].criticalMult;

        weaponLvl = gunList[selectedGun].weaponLevel;

        isReloading = false;

        isMeleeWeapon = gunList[selectedGun].isMelee;
        if (gunList[selectedGun].isGun == true && gunList[selectedGun].isMelee == false)
        {
            string muzzle = gunList[selectedGun].weaponName + " Muzzle";
            muzzlePosition = GameObject.Find(muzzle);           // most likely resource heavy method

            gunModel.GetComponent<MeshRenderer>().enabled = true;
            meleeModel.GetComponent<MeshRenderer>().enabled = false;
            // transfer the gun's model
            gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;

            //transfer the gun's textures/materials
            gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        }
        if (gunList[selectedGun].isGun == false && gunList[selectedGun].isMelee == true)
        {
            muzzlePosition = null;
            gunModel.GetComponent<MeshRenderer>().enabled = false;
            meleeModel.GetComponent<MeshRenderer>().enabled = true;
            // transfer the gun's model
            meleeModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;

            //transfer the gun's textures/materials
            meleeModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

        }

    }

    public void changeCurrentGun(gunStats gunStat) // in case you want to manually give the player a gun
    {
        shootRate = gunStat.shootRate;
        gunDMG = gunStat.modifedGunDMG;
        gameManager.instance.updatePlayerDamage(gunDMG);
        shootDist = gunStat.shootDist;
        gunshotSound = gunStat.gunshotSound;
        fireSelect = gunStat.fireSelect;

        gameManager.instance.ammoUpdate(gunStat.magCount, gunStat.magSize, gunStat.isMelee);

        pellets = gunStat.pellets;
        spreadAccuracy = gunStat.spreadAccuracy;
        hasScope = gunStat.hasScope;
        gunCriticalMult = gunStat.criticalMult;

        isMeleeWeapon = gunStat.isMelee;
        if (gunStat.isGun == true)
        {
            gunModel.GetComponent<MeshRenderer>().enabled = true;
            meleeModel.GetComponent<MeshRenderer>().enabled = false;
            // transfer the gun's model
            gunStat.GetComponent<MeshFilter>().sharedMesh = gunStat.gunModel.GetComponent<MeshFilter>().sharedMesh;

            if (gunList[selectedGun].isGun == true && gunList[selectedGun].isMelee == false)
            {
                gunModel.GetComponent<MeshRenderer>().enabled = true;
                meleeModel.GetComponent<MeshRenderer>().enabled = false;
                // transfer the gun's model
                gunModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;

                //transfer the gun's textures/materials
                gunModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

            }
            if (gunList[selectedGun].isGun == false && gunList[selectedGun].isMelee == true)
            {
                gunModel.GetComponent<MeshRenderer>().enabled = false;
                meleeModel.GetComponent<MeshRenderer>().enabled = true;
                // transfer the gun's model
                meleeModel.GetComponent<MeshFilter>().sharedMesh = gunList[selectedGun].gunModel.GetComponent<MeshFilter>().sharedMesh;

                //transfer the gun's textures/materials
                meleeModel.GetComponent<MeshRenderer>().sharedMaterial = gunList[selectedGun].gunModel.GetComponent<MeshRenderer>().sharedMaterial;

            }
        }
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
    public AudioSource getPlayerAud()
    {
        return aud;
    }

    public void BuyWeapon(GameObject weapon)
    {
        Instantiate(weapon, transform.position, transform.rotation);
    }

    public float getLaserSoundVol()
    {
        return laserUpgradeSFXVol;
    }

    public AudioClip getLaserSFX()
    {
        return laserUpgradeSFX;
    }
}

