 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenShop : MonoBehaviour
{
    public GameObject shop;
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip  openShop;
    [Range(0, 3)][SerializeField] float openShopVolume;
    public KeyCode _key;
    string presskeytoOpenShop = "Press E to open shop";

    bool enterbox = false;
    bool pressKeycheck = false;
    // Start is called before the first frame update
    void Start()
    {
        // shop.SetActive(false);
    }

    private void Update()
    {
        if (enterbox)
        {
            if (Input.GetKeyUp(_key))
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
        enterbox = true;
        if (other.gameObject.tag == "Player" && !pressKeycheck)
        {
            StartCoroutine(gameManager.instance.PressketoOpenShop(presskeytoOpenShop, 1.0f));
        }

    }

    private void OnTriggerExit(Collider other)
    {

        enterbox = false;
        pressKeycheck = false;
        gameManager.instance.activeMenu = null;
        shop.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

}
