using System.Collections;
using UnityEngine;



public class PlayerAnimator : MonoBehaviour
{
    [SerializeField] private Transform attackPointTransform;

    [SerializeField] private float attackPrepareTime = 0.5f;
    [SerializeField] private float attackResetDelay = 1f;

    private Animator anim;



    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    public void StartWeaponAttack(float delayBeforeImpact)
    {
        StartCoroutine(AttackSequence(delayBeforeImpact));
    }

    private IEnumerator AttackSequence(float delayBeforeImpact)
    {
        float t = 0;
        Vector3 startPointTransform = transform.position;

        do
        {
            yield return null;

            t += Time.deltaTime / delayBeforeImpact;
            transform.position = Vector3.Lerp(transform.position, attackPointTransform.position, t);
        }
        while (transform.position != attackPointTransform.position);

        anim.speed = 1 / Mathf.Clamp(delayBeforeImpact - attackPrepareTime, 0, float.MaxValue);
        anim.SetTrigger("Attack");

        yield return new WaitForSeconds(attackResetDelay);

        t = 0;
        do
        {
            yield return null;

            t += Time.deltaTime / attackPrepareTime;
            transform.position = Vector3.Lerp(transform.position, startPointTransform, t);
        }
        while (transform.position != startPointTransform);
    }
}