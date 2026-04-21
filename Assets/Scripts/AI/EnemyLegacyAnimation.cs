using UnityEngine;
using System.Collections;

public class EnemyLegacyAnimation : MonoBehaviour
{
    private Animation anim;
    private bool isAttacking = false;

    void Start()
    {
        anim = GetComponent<Animation>();

       
    }

    public void SetWalking(bool walking)
    {
        if (isAttacking) return; // Neanimuj chùzi bìhem útoku

        if (walking)
            anim.Play("run");
        else
            anim.Play("combat_idle");
    }

    public void Attack()
    {
        if (isAttacking) return;

        isAttacking = true;
        anim.Play("attack3");
        SoundManager.Instance.PlayAttack();

        StartCoroutine(ResetAttack());
    }

    private IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(anim["attack3"].length);
        isAttacking = false;
        anim.Play("combat_idle");
    }

    public void Die()
    {
        anim.Play("death");
        SoundManager.Instance.PlayDeath();
    }
}
