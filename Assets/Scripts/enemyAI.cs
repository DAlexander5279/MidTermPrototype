using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("--- Components --")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;

    [Header("--- Enemy Stats ---")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightAngle;
    [SerializeField] Transform headPosition;

    [Header("--- Enemy Gun Stats ---")]
    [Range(1, 2)] [SerializeField] int gunType; // 1 = rifle, 2 = shotgun
    [SerializeField] float fireRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPosition;

    [Header("--- Shotgun Enemy Stats ---")]
    [SerializeField] int pelletSpread;
    [SerializeField] int pelletCount;

    int HPOriginal;
    bool isShooting;
    bool playerInRange;
    Vector3 playerDirection;
    float angleToPlayer;
    Color enemyMaterialOriginal;

    // If the enemy has seen the player and deemed them a threat...
    bool playerThreat;

    // Start is called before the first frame update
    void Start()
    {
        HPOriginal = HP;
        enemyMaterialOriginal = model.material.color;
        gameManager.instance.updateEnemyCount(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            canSeePlayer();
        }
    }

    // function that makes the enemy track and attack the player
    void targetPlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (!isShooting)
            StartCoroutine(shoot());

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            facePlayer();
        }
    }

    void canSeePlayer()
    {
        playerDirection = (gameManager.instance.player.transform.position - headPosition.position);
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                playerThreat = true;    // enemy views player as a viable threat
            }

            if (playerThreat && angleToPlayer >= sightAngle * 1.45)        // enemy will lose aggro if player leaves the "battle vision angle"
            {
                playerThreat = false;
            }

            if (playerThreat)           // as long as the player is a threat, the enemy will attack, follow, and track the player
            {
                targetPlayer();
            }
        }

    }

    void facePlayer()
    {
        playerDirection.y = 0; // set to 0 to avoid model doing Smooth Criminal lean
        Quaternion rot = Quaternion.LookRotation(playerDirection);
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * playerFaceSpeed);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            playerThreat = false;
        }
    }

    public void takeDamage(int dmgIn)
    {
        HP -= dmgIn;
        agent.SetDestination(gameManager.instance.player.transform.position);
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            // Update enemy count
            gameManager.instance.updateEnemyCount(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        model.material.color = enemyMaterialOriginal;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        if (gunType == 1)
            Instantiate(bullet, shootPosition.position, transform.rotation);

        if (gunType == 2)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                // Vector that will apply force to the bullet to imitate shotgun spread
                Vector3 spreadForce = transform.forward +
                    new Vector3(Random.Range(-pelletSpread, pelletSpread), Random.Range(-pelletSpread, pelletSpread), Random.Range(-pelletSpread, pelletSpread));

                // a pellet being shot straight like a bullet
                GameObject pellet = Instantiate(bullet, shootPosition.position, transform.rotation);

                // spread being applied to the pellet
                pellet.GetComponent<Rigidbody>().AddForce(spreadForce);
            }
        }
        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }

}
