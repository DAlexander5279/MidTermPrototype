using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject shop;


    // Start is called before the first frame update
    void Start()
    {

        shop.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            gameManager.instance.activeMenu = gameManager.instance.shopMenu;
            gameManager.instance.gamePause();
            shop.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            shop.SetActive(false);
        }
    }


}
