using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This script is used for standard bullet functions. Can be copied for other types of bullet/weapon types
/// </summary>
public class bullet : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("--- Bullet Stats ---")]
    [SerializeField] int damage;
    [SerializeField] int speed;
    [SerializeField] int timer;
    [SerializeField] float inaccuracyAmt;
    // Start is called before the first frame update
    void Start()
    {
        //rb.velocity = transform.forward * speed;
        Vector3 target = (gameManager.instance.player.transform.position - transform.position).normalized;
        target += Random.insideUnitSphere * inaccuracyAmt;
        rb.velocity = target * speed;

        Destroy(gameObject, timer);
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(damage);
        }
        Destroy(gameObject);
    }
}
