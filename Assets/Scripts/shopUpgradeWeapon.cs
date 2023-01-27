using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class shopUpgradeWeapon : MonoBehaviour
{
    [SerializeField] gunStats gunToUpgrade;         // weapon to upgrade
    [SerializeField] GameObject gunDrop;            // weapon drop of said weapon
    [SerializeField] TMP_Text gunUpgradeCostText;   // upgrade cost button text
    int costOfUpgrade = 0;                          // initialize costOfUpgrade in case of errors

    //string alreadyOwnWeaponText = "Already own weapon!!!";
    string doesntOwnWeaponText = "You don't own that weapon yet!!!";
    string notEnoughCashText = "Not enough cash!!!";

    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip upgradeWeapon;
    [Range(0, 3)][SerializeField] float weaponUpgradeVol;
    [SerializeField] AudioClip denyWeaponUpgrade;
    [Range(0, 3)][SerializeField] float denyWeaponUpgradeVol;
    private void Start()
    {
        gunToUpgrade.weaponLevel = 1;
    }

    private void Update()
    {
        bool isShopActive = gameManager.instance.shopMenu.activeSelf;
        if (isShopActive)    // if the shop menu is active...
        {
            //get up-to-date upgrade cost in case player got an upgrade from a drop on the field
            costOfUpgrade = gunToUpgrade.CostofWeapon + Mathf.FloorToInt(gunToUpgrade.CostofWeapon * gunToUpgrade.weaponLevel * 1.33f);
            gameManager.instance.updateUpgradeCost(costOfUpgrade, gunUpgradeCostText);
        }
    }

    public void OnButtonClick()
    {
        if (gameManager.instance.zoins >= costOfUpgrade)    //if player can afford the upgrade...
        {
            if (gameManager.instance.playerScript.gunList.Count == 0)   //if player has no weapons, give "doesn't own weapon" error text
            {
                aud.PlayOneShot(upgradeWeapon, weaponUpgradeVol);
                StartCoroutine(gameManager.instance.ShopErrorNotif(doesntOwnWeaponText, 2.0f));
            }
            else
            {
                bool boughtUpgrade = false; // check to see if the upgrade has been bought/weapon to upgrade has been found
                for (int i = 0; i < gameManager.instance.playerScript.gunList.Count; i++)
                {
                    if (gunToUpgrade == gameManager.instance.playerScript.gunList[i] && !boughtUpgrade)
                    {
                        boughtUpgrade = true;   //we can buy the upgrade
                    }
                }
                if (!boughtUpgrade) //if no upgrade can be bought/weapon to upgrade is not owned by the player...
                {
                    aud.PlayOneShot(denyWeaponUpgrade, denyWeaponUpgradeVol);
                    StartCoroutine(gameManager.instance.ShopErrorNotif(doesntOwnWeaponText, 2.0f));
                }
                else
                {
                    aud.PlayOneShot(upgradeWeapon, weaponUpgradeVol);
                    gameManager.instance.addZoins(-costOfUpgrade);  // take away necessary coins
                    gameManager.instance.playerScript.BuyWeapon(gunDrop);   // spawn "upgrade" onto the player

                    costOfUpgrade = gunToUpgrade.CostofWeapon + (gunToUpgrade.CostofWeapon * gunToUpgrade.weaponLevel); // calculate new upgrade cost
                    gameManager.instance.updateUpgradeCost(costOfUpgrade, gunUpgradeCostText);  //update upgrade cost as a pre-caution
                }
            }

        }
        // if player didn't have enough cash to buy the upgrade, give correct error
        else
        {
            aud.PlayOneShot(denyWeaponUpgrade, denyWeaponUpgradeVol);
            StartCoroutine(gameManager.instance.ShopErrorNotif(notEnoughCashText, 2.0f));
        }
    }
}

