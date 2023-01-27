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
        if (other.CompareTag("Player"))
        {
            int HPHealedTotal = gameManager.instance.playerScript.HP + Mathf.FloorToInt(addedHP * (gameManager.instance.waveCount * 0.75f));
            if ((HPHealedTotal >= gameManager.instance.playerScript.HPOriginal) && (gameManager.instance.waveCount >= 5))    // grant more HP when overhealing starting Round 5
            {
                gameManager.instance.playerScript.HPOriginal += 25;
                gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOriginal;
                gameManager.instance.playerScript.getPlayerAud().
                    PlayOneShot(gameManager.instance.playerScript.getLaserSFX(), gameManager.instance.playerScript.getLaserSoundVol());
            }
            else if ((HPHealedTotal >= gameManager.instance.playerScript.HPOriginal) && (gameManager.instance.waveCount < 5))  // if below Round 5, just grant regular HP amount for regular and overheal
            {
                gameManager.instance.playerScript.HP = gameManager.instance.playerScript.HPOriginal;
            }
            else
            {
                gameManager.instance.playerScript.HP = HPHealedTotal;
            }

            gameManager.instance.playerScript.updatePlayerHP();

            Destroy(gameObject);
        }
    }


}
