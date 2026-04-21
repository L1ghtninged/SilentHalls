using System.Collections;
using UnityEngine;

public class EnemyAnimation : MonoBehaviour
{
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetWalking(bool walking)
    {
        animator.SetBool("isWalking", walking);
    }

    public void Attack()
    {
        animator.SetBool("isAttacking", true);
        // Volitelnì po chvíli deaktivovat (napø. v coroutinì)
        StartCoroutine(ResetAttack());
    }


    private IEnumerator ResetAttack()
    {
        SoundManager.Instance.PlayAttack();

        // Získání délky animace "Attack"
        float clipLength = GetAnimationClipLength("Attack"); // Musí odpovídat jménu klipu v Animatoru
        yield return new WaitForSeconds(clipLength);

        animator.SetBool("isAttacking", false);
    }
    private float GetAnimationClipLength(string clipName)
    {
        foreach (var clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == clipName)
                return clip.length;
        }
        return 0.5f; // fallback, když nenajde
    }

}