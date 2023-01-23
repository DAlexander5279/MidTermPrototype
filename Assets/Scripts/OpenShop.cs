 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject shop;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip  openShop;
    [Range(0, 3)][SerializeField] float openShopVolume;

    // Start is called before the first frame update
    void Start()
    {

        shop.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player")
        {
            aud.PlayOneShot(openShop, openShopVolume);
            gameManager.instance.activeMenu = gameManager.instance.shopMenu;
            //gameManager.instance.gamePause();
            shop.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            aud.PlayOneShot(openShop, openShopVolume);
            gameManager.instance.activeMenu = null;
            shop.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

        }
    }


}
