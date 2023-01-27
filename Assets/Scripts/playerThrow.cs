using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class playerThrow : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("Components")]
    public Transform cam;
    public Transform throwPoint;
    public GameObject thrownObject;
    [SerializeField] KeyCode throwKey;
    [SerializeField] Image grenadeCooldown;

    [Header("Thrown Stats")]
    [SerializeField] int totalThrows;
    [SerializeField] float rechargeRate;
    [SerializeField] float throwCooldown;
    [SerializeField] float throwForce;
    [SerializeField] float throwForceUpward;
    [SerializeField] bool canThrow;
    public bool abilityOnCooldown;
    int totalThrowsOriginal;

    void Start()
    {
        totalThrowsOriginal = totalThrows;
        cam = Camera.main.GetComponent<cameraMovement>().transform;
        // look into where to make canThrow true when picking up the Grenade ability
    }

    void Update()
    {
        if (Input.GetKeyDown(throwKey) && canThrow && totalThrows > 0 && !gameManager.instance.paused)
        {
            Throw();
        }

        if (!abilityOnCooldown && (totalThrows < totalThrowsOriginal))
        {
            StartCoroutine(abilityCooldown());
        }

        if (abilityOnCooldown)
        {
            grenadeCooldown.fillAmount -= 1 / rechargeRate * Time.deltaTime;
        }
    }

    void Throw()
    {
        canThrow = false;

        GameObject projectile = Instantiate(thrownObject, throwPoint.position, cam.rotation);

        Rigidbody thrownObjectRb = projectile.GetComponent<Rigidbody>();

        Vector3 throwingForce = (cam.transform.forward * throwForce);

        // using Impulse as we only need to apply the throwing force once
        thrownObjectRb.AddForce(throwingForce, ForceMode.Impulse);

        totalThrows--;



        StartCoroutine(throwSpamProtection());

    }

    IEnumerator abilityCooldown()
    {
        abilityOnCooldown = true;
        grenadeCooldown.fillAmount = 1;
        yield return new WaitForSeconds(rechargeRate);
        totalThrows++;
        if (!canThrow)
            canThrow = true;
        abilityOnCooldown = false;
    }

    IEnumerator throwSpamProtection()
    {
        yield return new WaitForSeconds(throwCooldown);
        if (totalThrows > 0)
            canThrow = true;
    }
}
