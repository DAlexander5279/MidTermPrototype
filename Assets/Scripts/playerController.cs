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
    [Range(1, 5)][SerializeField] int gunDMG;
    [Range(1, 3)] [SerializeField] int shootRate;
    [Range(1, 5)] [SerializeField] int shootDis;

    bool firing;
    private Vector3 playerVelocity;
    Vector3 move;
    int Timesjumped;
    // Start is called before the first frame update
    void Start()
    {
        
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
            Timesjumped = 0;
            playerVelocity.y = 0f;
        }

        move = transform.right * Input.GetAxis("Horizontal") + transform.forward * Input.GetAxis("Vertical");

        player.Move(move * Time.deltaTime * playerSpeed);


        // Changes the height position of the player..
        if (Input.GetButtonDown("Jump") && Timesjumped < jumpMax)
        {
            Timesjumped++;
            playerVelocity.y = jumpHeight;
        }

        playerVelocity.y -= gravity * Time.deltaTime;
        player.Move(playerVelocity * Time.deltaTime);

    }
    IEnumerator Shoot()
    {
        if (!firing && Input.GetButtonDown("Shoot"))
        {
            firing = true;

            RaycastHit Hit;

            Debug.Log("I Shoot");

            if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector2(0.5f, 0.5f)), out Hit, shootDis))
            {
                if (Hit.collider.GetComponent<IDamage>() != null)
                {
                    Hit.collider.GetComponent<IDamage>().takeDamage(gunDMG);
                }
            }
            yield return new WaitForSeconds(shootRate);
            firing = false;
        }
    }

}
