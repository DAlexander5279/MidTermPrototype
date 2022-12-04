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

    }

    void facePlayer()
    {

    }

    public void takeDamage(int dmgIn)
    {

    }

    IEnumerator flashDamage()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(0.2f);
        model.material.color = enemyMaterialOriginal;
    }
}
