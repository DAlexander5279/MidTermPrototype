using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAnimation : MonoBehaviour
{
    public Animator anime;
    public GameObject gameObjectSword;
    bool canAttack = true;
    public float attackCoolDown = 1.0f;
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (canAttack == true)
            {
                swordAttack();
            }
        }
    }
    public void swordAttack()
    {
        canAttack= true;
        //anime = gameObjectSword.GetComponent<Animator>();
        anime.SetTrigger("Attack");
        StartCoroutine(resetAttackCoolDown());
    }
    IEnumerator resetAttackCoolDown()
    {
        canAttack = false;
        yield return new WaitForSeconds(attackCoolDown);
        canAttack = true;
    }
}
