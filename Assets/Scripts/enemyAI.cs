using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class enemyAI : MonoBehaviour, IDamage
{
    [Header("--- Components --")]
    [SerializeField] Renderer model;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] GameObject enemyUI;
    [SerializeField] Image enemyHPBar;
    [SerializeField] Animator anim;

    [Header("--- Enemy Stats ---")]
    [SerializeField] int HP;
    [SerializeField] int playerFaceSpeed;
    [SerializeField] int sightAngle;
    [SerializeField] Transform headPosition;
    [Range(1.0f, 2.0f)] [SerializeField] float dangerSightModifier;

    [SerializeField] int droppedZoinsAmt;   // game currency

    [SerializeField] List<GameObject> itemDropList;
    [Range(0, 100)] [SerializeField] int baseDropChance;
    [SerializeField] int enemyType;     // 0 = rank-and-file, 1 = major, 2 = boss

    [SerializeField] int animTransSpeed;

    [Header("--- Enemy Gun Stats ---")]
    [Range(1, 2)] [SerializeField] int gunType; // 1 = rifle, 2 = shotgun
    [SerializeField] float fireRate;
    [SerializeField] GameObject bulletType;
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
    Vector3 pushBack;




    // If the enemy has seen the player and deemed them a threat...
    bool playerThreat;

    // Start is called before the first frame update
    void Start()
    {
        HP = scalingFunction(HP);
        HPOriginal = HP;
        enemyMaterialOriginal = model.material.color;
        gameManager.instance.updateEnemyCount(1);

    }

    // Update is called once per frame
    void Update()
    {
        anim.SetFloat("Speed", Mathf.Lerp(anim.GetFloat("Speed"), agent.velocity.normalized.magnitude, Time.deltaTime * animTransSpeed));
        pushBack = Vector3.Lerp(pushBack, Vector3.zero, Time.deltaTime);
        agent.Move((agent.velocity + pushBack) * Time.deltaTime);

        if (playerInRange)
        {
            canSeePlayer();
        }

    }

    // function that makes the enemy track and attack the player
    void targetPlayer()
    {
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (!isShooting && playerInRange)
            StartCoroutine(shoot());

        if (agent.remainingDistance <= agent.stoppingDistance || playerThreat)
        {
            facePlayer();
        }
    }

    void canSeePlayer()
    {
        playerDirection = (gameManager.instance.player.transform.position - headPosition.position);
        angleToPlayer = Vector3.Angle(playerDirection, transform.forward);

        //Debug.Log(angleToPlayer);
        Debug.DrawRay(headPosition.position, playerDirection);

        RaycastHit hit;
        if (Physics.Raycast(headPosition.position, playerDirection, out hit))
        {
            if (hit.collider.CompareTag("Player") && angleToPlayer <= sightAngle)
            {
                playerThreat = true;    // enemy views player as a viable threat
            }

            if (playerThreat && angleToPlayer >= (sightAngle * dangerSightModifier))        // enemy will lose aggro if player leaves the "battle vision angle"
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
        updateEnemyHPBar();
        enemyUI.gameObject.SetActive(true);
        agent.SetDestination(gameManager.instance.player.transform.position);
        if (!playerThreat)
            playerThreat = true;
        StartCoroutine(flashDamage());

        if (HP <= 0)
        {
            // Update enemy count
            gameManager.instance.updateEnemyCount(-1);

            gameManager.instance.enemiesKilled++;

            droppedZoinsAmt = scalingFunction(droppedZoinsAmt);

            gameManager.instance.addZoins(droppedZoinsAmt);

            rollDropItem(scalingFunction(baseDropChance));


            Destroy(gameObject);
        }
    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        model.material.color = enemyMaterialOriginal;
    }

    IEnumerator shoot()
    {
        isShooting = true;

        anim.SetTrigger("Shoot");

        if (gunType == 1)
            Instantiate(bulletType, shootPosition.position, transform.rotation);

        if (gunType == 2)
        {
            for (int i = 0; i < pelletCount; i++)
            {
                // Vector that will apply force to the bullet to imitate shotgun spread
                Vector3 spreadForce = transform.forward +
                    new Vector3(Random.Range(-pelletSpread, pelletSpread), Random.Range(-pelletSpread, pelletSpread), Random.Range(-pelletSpread, pelletSpread));

                // a pellet being shot straight like a bullet
                GameObject pellet = Instantiate(bulletType, shootPosition.position, transform.rotation);

                // spread being applied to the pellet
                pellet.GetComponent<Rigidbody>().AddForce(spreadForce);
            }
        }
        yield return new WaitForSeconds(fireRate);

        isShooting = false;
    }

    void updateEnemyHPBar()
    {
        if (HP > 0)
            enemyHPBar.fillAmount = (float)HP / (float)HPOriginal;
        else
            enemyHPBar.fillAmount = 0.0f;
    }

    public void pushObject(Vector3 pushDir)
    {
        pushBack = pushDir;
    }

    public int scalingFunction(int var)
    {
        return var + Mathf.FloorToInt(var * (gameManager.instance.getScalingModifier() * Mathf.Floor(gameManager.instance.roomCount * 0.2f)));
    }

    void rollDropItem(int chance)
    {
        if (chance > Random.Range(0, 100) && itemDropList.Count > 0)
        {
            Instantiate(itemDropList[Random.Range(0, itemDropList.Count)], headPosition.position, transform.rotation);
            gameManager.instance.updateItemCount(1);

            if (enemyType == 2)
            {
                Instantiate(itemDropList[Random.Range(0, itemDropList.Count)], headPosition.position, transform.rotation);
                gameManager.instance.updateItemCount(1);
            }
            else if (enemyType == 1 & chance > Random.Range(0, 100))
            {
                Instantiate(itemDropList[Random.Range(0, itemDropList.Count)], headPosition.position, transform.rotation);
                gameManager.instance.updateItemCount(1);
            }
        }
    }
}
