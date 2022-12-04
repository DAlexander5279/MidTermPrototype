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
    [SerializeField] float fireRate;
    [SerializeField] GameObject bullet;
    [SerializeField] Transform shootPosition;

    int HPOriginal;
    bool isShooting;
    bool playerInRange;
    Vector3 playerDirection;
    float angleToPlayer;
    Color enemyMaterialOriginal;

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

    void canSeePlayer()
    {
        playerDirection = (gameManager.instance.player.transform.position - headPosition.position);
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        Debug.Log(angleToPlayer);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection,out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                agent.SetDestination(gameManager.instance.player.transform.position);

                if (!isShooting)
                    StartCoroutine(shoot());

                if (agent.remainingDistance <= agent.stoppingDistance)
                {
                    facePlayer();
                }
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

        Instantiate(bullet, shootPosition.position, transform.rotation);
        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }
}
