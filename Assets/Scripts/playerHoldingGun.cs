using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerHoldingGun : MonoBehaviour
{
    [SerializeField] gunStats stats;
    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.equippedGuns(stats);
            Destroy(gameObject);
        }
    }
}
