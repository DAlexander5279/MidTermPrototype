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
    [Range(10,20)] [SerializeField] int gravity;
    [SerializeField] bool isDisabled;
    

    [Header("------Gun Stats------")]
    [Range(1, 5)] [SerializeField] int gunDMG;
    [Range(1, 3)] [SerializeField] int shootRate;   // player's gun fire rate
    [Range(5, 200)] [SerializeField] int shootDis;    // effective range of the shot

    bool isShooting;
    private Vector3 playerVelocity;
    Vector3 move;

    int timesJumped;
    int HPOriginal;

    

    // Start is called before the first frame update
    private void Start()
    {
        HPOriginal = HP;

    }

    // Update is called once per frame
    void Update()
    {
        if(!isDisabled)
        {
            Movement();
            StartCoroutine(Shoot());
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

            //Debug.Log("I Shoot");

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out hit, shootDis))
            {
                if (hit.collider.GetComponent<IDamage>() != null)
                {
                    hit.collider.GetComponent<IDamage>().takeDamage(gunDMG);
                }
            }
            yield return new WaitForSeconds(shootRate);
            isShooting = false;
        }
    }

    public void takeDamage(int dmgIn)
    {
        HP -= dmgIn;
        StartCoroutine(playerFlashDamage());

        if(HP <= 0)
        {
            // will implement later
        }
    }

    IEnumerator playerFlashDamage()
    {
        gameManager.instance.playerFlashDamage.SetActive(true);
        yield return new WaitForSeconds(0.1f);
        gameManager.instance.playerFlashDamage.SetActive(false);

    }
   

}
