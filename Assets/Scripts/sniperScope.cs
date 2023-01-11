using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sniperScope : MonoBehaviour
{
    public Animator sniperAnimator;
    public GameObject scopeOverlay;
    public GameObject weaponCamera;
    public Camera mainCamera;

    private bool isScopedIn = false;
    public float fovScoped = 25f;
    private float fovOriginal;

    private void Start()
    {
        fovOriginal = mainCamera.fieldOfView;
    }

    private void Update()
    {
        if(Input.GetButtonDown("Fire2") && gameManager.instance.playerScript.getScopeStatus())
        {
            isScopedIn = !isScopedIn;
            sniperAnimator.SetBool("isScoped", isScopedIn);

            if (isScopedIn)
                StartCoroutine(showScopeOverlay());
            else if (!isScopedIn)
            {
                weaponCamera.SetActive(true);
                scopeOverlay.SetActive(false);
                mainCamera.fieldOfView = fovOriginal;
            }
        }
    }

    IEnumerator showScopeOverlay()
    {
        yield return new WaitForSeconds(0.15f);
        weaponCamera.SetActive(false);
        scopeOverlay.SetActive(true);
        mainCamera.fieldOfView = fovScoped;
        
    }
}
