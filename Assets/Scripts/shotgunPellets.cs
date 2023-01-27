using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is a derivative of the "bullet" script. This is only to be used for the shotgunner enemy.
/// </summary>
public class shotgunPellets : MonoBehaviour
{
    [SerializeField] Rigidbody rb;

    [Header("--- Pellet Stats ---")]
    [SerializeField] int pelletDamage;
    [SerializeField] int pelletSpeed;
    [SerializeField] int timer;
    // Start is called before the first frame update
    void Start()
    {
        //rb.velocity = transform.forward * pelletSpeed;
        Vector3 target = (gameManager.instance.player.transform.position - transform.position).normalized;
        rb.velocity = target * pelletSpeed;
        Destroy(gameObject, timer);
        pelletDamage += Mathf.FloorToInt(pelletDamage * (gameManager.instance.waveCount * 1.0025f));
    }

    // Update is called once per frame
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.takeDamage(pelletDamage);
        }
        Destroy(gameObject);
    }
}
