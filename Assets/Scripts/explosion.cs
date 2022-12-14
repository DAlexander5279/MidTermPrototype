using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class explosion : MonoBehaviour
{
    [Header("Explosion Stats")]
    // Start is called before the first frame update
    [SerializeField] int pushBackAmount;
    //[SerializeField] bool physicsType;          // true == push | false == pull
    [SerializeField] int damage;
    [SerializeField] float delayTime;
    [SerializeField] float radius;

    [Header("Other Components")]
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] AudioSource expAud;
    [SerializeField] AudioClip explosionSound;
    [Range(0, 3)] [SerializeField] float explosionSoundVol;

    Vector3 pushForce;
    Renderer itemModel;

    void Start()
    {
        StartCoroutine(Explode());
    }

    IEnumerator Explode()
    {
        yield return new WaitForSeconds(delayTime);
        // Checking nearby colliders
        Collider[] colliders = Physics.OverlapSphere(transform.position, radius);

        foreach (Collider nearExplosion in colliders)
        {
            if (nearExplosion.CompareTag("Player"))
            {
                pushForce = (nearExplosion.transform.position - transform.position) * pushBackAmount;
                gameManager.instance.playerScript.takeDamage(damage);
                gameManager.instance.playerScript.inputPushBack(pushForce);
            }
            else if (nearExplosion.CompareTag("Enemy"))
            {
                pushForce = (nearExplosion.transform.position - transform.position) * pushBackAmount;
                nearExplosion.GetComponent<IDamage>().takeDamage(damage);
                nearExplosion.GetComponent<IDamage>().pushObject(pushForce);
            }
        }

        expAud.pitch = Random.Range(0.65f, 1.0f);    // ONLY USE IN A PINCH --- 1 == normal pitch
        expAud.PlayOneShot(explosionSound, explosionSoundVol);
        itemModel = GetComponent<MeshRenderer>();
        itemModel.enabled = false;
        //Instantiate(explosionEffect, other.transform.position, other.transform.rotation);
        yield return new WaitForSeconds(5.0f);
        Destroy(gameObject);
    }
}
