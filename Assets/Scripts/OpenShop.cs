using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject shop;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip openShop;
    [Range(0, 3)] [SerializeField] float openShopVolume;
    public KeyCode _key;
    string presskeytoOpenShop = "Press E to open shop";

    bool enterbox = false;
    bool pressKeycheck = false;


    private void Update()
    {
        if (enterbox)
        {
            if (Input.GetButtonDown("Open Buy Station"))
            {
                gameManager.instance.activeMenu = gameManager.instance.shopMenu;
                shop.SetActive(true);
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Player" && !pressKeycheck)
        {
            enterbox = true;
            StartCoroutine(gameManager.instance.PressketoOpenShop(presskeytoOpenShop, 1.0f));
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            enterbox = false;
            pressKeycheck = false;
            gameManager.instance.activeMenu = null;
            shop.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }

}
