using System.Collections;
using UnityEngine;



public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Transform attackPointTransform;

    private Animator anim;



    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartWeaponAttack(int animationNameHash, float delayBeforeImpact, float attackPrepareTime, float attackResetDelay)
    {
        StartCoroutine(AttackSequence(animationNameHash, delayBeforeImpact, attackPrepareTime, attackResetDelay));
    }
    private IEnumerator AttackSequence(int animationNameHash, float delayBeforeImpact, float attackPrepareTime, float attackResetDelay)
    {
        float t = 0;
        Vector3 startPointTransform = transform.position;

        do
        {
            yield return null;

            t += Time.deltaTime / attackPrepareTime;
            transform.position = Vector3.Lerp(transform.position, attackPointTransform.position, t);
        }
        while (t < 1);

        float animTime = Mathf.Clamp(delayBeforeImpact - attackPrepareTime, 0, float.MaxValue);

        anim.speed = 1 / animTime;
        anim.PlayInFixedTime(animationNameHash);

        yield return new WaitForSeconds(animTime + attackResetDelay);

        t = 0;
        do
        {
            yield return null;

            t += Time.deltaTime / attackPrepareTime;
            transform.position = Vector3.Lerp(transform.position, startPointTransform, t);
        }
        while (t < 1);
    }


    public void StartWeaponSupport(int animationNameHash)
    {
        anim.speed = 1;
        anim.PlayInFixedTime(animationNameHash);
    }
}