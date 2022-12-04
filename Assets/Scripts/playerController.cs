using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerController : MonoBehaviour
{

    [Header("------Components------")]
    [SerializeField] CharacterController player;

    [Header("------Player Stats------")]

    [SerializeField] int HP;
    [Range(1, 10)][SerializeField] int playerSpeed;
    [SerializeField] int jumpMax;
    [Range(5, 15)][SerializeField] int jumpHeight;
    [Range(10,20)] [SerializeField] int gravity;

    [Header("------Gun Stats------")]
    [Range(1, 5)] [SerializeField] int gunDMG;
    [Range(1, 3)] [SerializeField] int shootRate;   // player's gun fire rate
    [Range(1, 5)] [SerializeField] int shootDis;    // effective range of the shot

    bool isShooting;
    private Vector3 playerVelocity;
    Vector3 move;
    int timesJumped;
    int HPOriginal;
    // Start is called before the first frame update
    void Start()
    {
        HPOriginal = HP;
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        StartCoroutine(Shoot());
    }
    void Movement()
    {
        if (player.isGrounded && playerVelocity.y < 0)
        {
            timesJumped = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        player.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && timesJumped < jumpMax)
        {
            timesJumped++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        player.Move(playerVelocity * Time.deltaTime);

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

}
