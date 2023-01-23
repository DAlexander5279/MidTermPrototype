using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Buy_UpgradeWeapon : MonoBehaviour
{
    [SerializeField] gunStats gunToBuy;
    [SerializeField] GameObject gunDrop;
   
    public TMP_Text AlreadyownWeapon; 
    string ownWeapon = "Already own weapon.";

    public void OnButtonClick()
    {
        for (int i = 0; i < gameManager.instance.playerScript.gunList.Count;  i++)
        {
            if(gunToBuy == gameManager.instance.playerScript.gunList[i])
            {
                StartCoroutine(sendWeaponDuplicationNote(ownWeapon, 2));
            }
            else
            {
                gameManager.instance.playerScript.BuyWeapon(gunDrop); 
            }
        }
    }

    IEnumerator sendWeaponDuplicationNote(string text, int time)
    {
        AlreadyownWeapon.text = ownWeapon;
        yield return new WaitForSeconds(time);
        AlreadyownWeapon.text = "";
    }
}

