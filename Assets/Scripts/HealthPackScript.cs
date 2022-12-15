using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPackScript : MonoBehaviour
{
    [SerializeField] GameObject Cross;
    [SerializeField] float animSpeed;
    [SerializeField] int rotateSpeed;
    [SerializeField] int addedHP;
    bool isAnimPlaying;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Cross.transform.Rotate(0, 1 * Time.deltaTime * rotateSpeed, 0);


    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            gameManager.instance.playerScript.HP += addedHP;

            if (gameManager.instance.playerScript.HP > gameManager.instance.playerScript.HPOriginal)
                gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOriginal;

            gameManager.instance.playerScript.updatePlayerHP();

            if (gameManager.instance.itemCount != 0)
                gameManager.instance.updateItemCount(-1);

            Destroy(gameObject);
        }
    }

  
}
