using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] int pushBackAmount;
    [SerializeField] bool physicsType;          // true == push | false == pull
    //[SerializeField] ParticleSystem explosionEffect;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("Enemy"))
        {
            if(physicsType == true)
            {
                gameManager.instance.playerScript.inputPushBack((other.transform.position - transform.position) * pushBackAmount);
            }
            else
            {
                gameManager.instance.playerScript.inputPushBack((transform.position - other.transform.position) * pushBackAmount);

            }
            //Instantiate(explosionEffect, other.transform.position, other.transform.rotation);
        }
        Destroy(gameObject);
    }
}
