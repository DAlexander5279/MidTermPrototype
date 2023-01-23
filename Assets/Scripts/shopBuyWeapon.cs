using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class shopBuyWeapon : MonoBehaviour
{
    [SerializeField] gunStats gunToBuy;
    [SerializeField] GameObject gunDrop;

    string alreadyOwnWeaponText = "Already own weapon!!!";
    string doesntOwnWeaponText = "You don't own that weapon yet!!!";
    string notEnoughCashText = "Not enough cash!!!";

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip buyWeapon;
    [Range(0, 3)][SerializeField] float buyWeaponVol;
    [SerializeField] AudioClip denyWeapon;
    [Range(0, 3)][SerializeField] float denyWeaponVol;

    public void OnButtonClick()
    {
        if (gameManager.instance.zoins >= gunToBuy.CostofWeapon)
        {
            if (gameManager.instance.playerScript.gunList.Count == 0)
            {
                aud.PlayOneShot(buyWeapon, buyWeaponVol);
                gameManager.instance.playerScript.BuyWeapon(gunDrop);
            }
            else
            {
                bool boughtWeapon = true;
                for (int i = 0; i < gameManager.instance.playerScript.gunList.Count; i++)
                {
                    if (gunToBuy == gameManager.instance.playerScript.gunList[i] && boughtWeapon)
                    {
                        boughtWeapon = false;
                    }
                }
                if (!boughtWeapon)
                {
                    aud.PlayOneShot(denyWeapon, denyWeaponVol);

                    StartCoroutine(gameManager.instance.ShopErrorNotif(alreadyOwnWeaponText, 2.0f));
                }
                else
                {
                    aud.PlayOneShot(buyWeapon, buyWeaponVol);
                    gameManager.instance.playerScript.BuyWeapon(gunDrop);
                    gameManager.instance.addZoins(-gunToBuy.CostofWeapon);
                }
            }

        }
        else
        {
            StartCoroutine(gameManager.instance.ShopErrorNotif(notEnoughCashText, 2.0f));
        }
    }
}

