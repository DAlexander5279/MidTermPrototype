using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    [Header("------Components------")]
    [SerializeField] CharacterController playerControl;

    [Header("------Player Stats------")]

    [SerializeField] int HP;
    [Range(1, 10)][SerializeField] int playerSpeed;
    [SerializeField] int jumpMax;
    [Range(5, 15)][SerializeField] int jumpHeight;
    [Range(10, 20)][SerializeField] int gravity;
    [SerializeField] int pushTime;




    [Header("------Player Sounds------")]
    [SerializeField] AudioSource aud;

    //gun sounds
    [SerializeField] AudioClip audioGunShot;
    [Range(0, 3)][SerializeField] float audioShot;

    [SerializeField] AudioClip[] playerJumpAudio;
    [Range(0, 3)][SerializeField] float jumpVolAudio;

    [SerializeField] AudioClip[] playerHurtAudio;
    [Range(0, 3)][SerializeField] float hurtVolAudio;

    [SerializeField] AudioClip[] playerStepAudio;
    [Range(0, 3)][SerializeField] float audioPlayerStep;


    [Header("------Gun Stats------")]
    [SerializeField] List<gunStats> listGuns = new List<gunStats>();

    [Range(0, 5)][SerializeField] int gunDMG;
    [SerializeField] float shootRate;   // player's gun fire rate
    [Range(0, 200)][SerializeField] int shootDis; // effective range of the shot
    [SerializeField] GameObject gunModel;

    [SerializeField] GameObject effectedHit;




    bool isShooting;
    private Vector3 playerVelocity;
    Vector3 move;
    int timesJumped;
    int HPOriginal;
    int walkSpeedOrg;
    int selectedWeapon;
    bool audioIsPlaying;
    bool isRunning;
    Vector3 pushBack;
    public bool isDisabled;


    // Start is called before the first frame update
    private void Start()
    {
        HPOriginal = HP;

    }

    // Update is called once per frame
    void Update()
    {

        if (!gameManager.instance.paused)
        {
            pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime * pushTime);

            Movement();

            if (!audioIsPlaying && move.magnitude > 0.3f && playerControl.isGrounded)
            {
                StartCoroutine(stepsPlaying());
            }
            if (listGuns.Count > 0)
            {
                StartCoroutine(Shoot());
                selectedGun();
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

        playerControl.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpMax)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;

            aud.PlayOneShot(playerJumpAudio[UnityEngine.Random.Range(0, playerJumpAudio.Length)], jumpVolAudio);
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        playerControl.Move(playerVelocity * Time.deltaTime);

    }
    IEnumerator Shoot()
    {
        if (!isShooting && Input.GetButton("Shoot"))    //GetButton = full-auto | ...Down = semi-auto | ..Up = think single-action revolver) - J
        {
            isShooting = true;
            RaycastHit hit;


            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDis))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(gunDMG);
                }

                Instantiate(effectedHit, hit.point, effectedHit.transform.rotation);
            }

            aud.PlayOneShot(audioGunShot, audioShot);

            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void takeDamage(int dmgIn)
    {
        HP -= dmgIn;
        aud.PlayOneShot(playerHurtAudio[UnityEngine.Random.Range(0, playerHurtAudio.Length)], hurtVolAudio);
        updatePlayerHP();
        StartCoroutine(playerFlashDamage());

        if (HP <= 0)
        {
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

        aud.PlayOneShot(playerStepAudio[UnityEngine.Random.Range(0, playerStepAudio.Length)], audioPlayerStep);
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

    public void equippedGuns(gunStats statsOfGun)
    {
        gunDMG = statsOfGun.gunDMG;
        shootRate = statsOfGun.fireRate;
        shootDis = statsOfGun.shootDis;

        audioGunShot = statsOfGun.audioGun;

        gunModel.GetComponent<MeshFilter>().sharedMesh = statsOfGun.modelGun.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = statsOfGun.modelGun.GetComponent<MeshRenderer>().sharedMaterial;

        listGuns.Add(statsOfGun);

        
    }

    void EquippedGun()
    {
        shootRate = listGuns[selectedWeapon].fireRate;
        gunDMG = listGuns[selectedWeapon].gunDMG;
        shootDis = listGuns[selectedWeapon].shootDis;

        audioGunShot = listGuns[selectedWeapon].audioGun;

        // transfer the gun's model
        gunModel.GetComponent<MeshFilter>().sharedMesh = listGuns[selectedWeapon].modelGun.GetComponent<MeshFilter>().sharedMesh;

        //transfer the gun's textures/materials
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = listGuns[selectedWeapon].modelGun.GetComponent<MeshRenderer>().sharedMaterial;
    }
    void selectedGun()
    {
        gunDMG = listGuns[selectedWeapon].gunDMG;
        shootRate = listGuns[selectedWeapon].fireRate;
        shootDis = listGuns[selectedWeapon].shootDis;

        audioGunShot = listGuns[selectedWeapon].audioGun;


        gunModel.GetComponent<MeshFilter>().sharedMesh = listGuns[selectedWeapon].modelGun.GetComponent<MeshFilter>().sharedMesh;
        gunModel.GetComponent<MeshRenderer>().sharedMaterial = listGuns[selectedWeapon].modelGun.GetComponent<MeshRenderer>().sharedMaterial;

        if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon < listGuns.Count - 1)  // scrolling up + staying within List's range
        {
            selectedWeapon++;
            EquippedGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon > 0)  // scrolling down + staying within List's range
        {
            selectedWeapon--;
            EquippedGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0 && selectedWeapon == listGuns.Count - 1)  // scrolling up + final gun in list...
        {
            selectedWeapon = 0;    // loop back to first gun in list
            EquippedGun();

        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0 && selectedWeapon == 0)  // scrolling down + first gun in list...
        {
            selectedWeapon = listGuns.Count - 1;    // loop forward to final gun in list
            EquippedGun();

        }

    }
    private void updatePlayerHP()
    {
        gameManager.instance.playerHealthBar.fillAmount = (float)HP / (float)HPOriginal;
    }
    public void inputPushBack(Vector3 dir)
    {
        pushBack = dir;
    }


}
