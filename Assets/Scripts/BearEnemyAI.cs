using System.Collections;
using UnityEngine;

public class BearEnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform hiveAttackPoint;

    [Header("References")]
    [SerializeField] private Animator bearAnimator;
    [SerializeField] private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2.2f;
    [SerializeField] private float rotationSpeed = 8f;
    [SerializeField] private float stopDistance = 0.35f;

    [Header("Attack")]
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float attackDamageDelay = 0.35f;
    [SerializeField] private int hiveDamageAmount = 1;

    [Header("Animation State Names")]
    [SerializeField] private string idleStateName = "Idle";
    [SerializeField] private string runStateName = "Walk Forward";
    [SerializeField] private string attackStateName = "Attack1";

    private bool isAttacking;
    private float lastAttackTime;

    private void Awake()
    {
        if (rb == null)
            rb = GetComponent<Rigidbody>();

        if (bearAnimator == null)
            bearAnimator = GetComponentInChildren<Animator>();

        if (bearAnimator != null)
            bearAnimator.applyRootMotion = false;
    }

    private void Start()
    {
        if (hiveAttackPoint == null)
        {
            GameObject point = GameObject.Find("BearAttackPoint");

            if (point != null)
                hiveAttackPoint = point.transform;
            else
                Debug.LogWarning("BearEnemyAI: BearAttackPoint bulunamadı.");
        }
    }

    private void FixedUpdate()
    {
        if (hiveAttackPoint == null || isAttacking)
            return;

        Vector3 targetPosition = hiveAttackPoint.position;
        Vector3 direction = targetPosition - transform.position;
        direction.y = 0f;

        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            MoveToTarget(direction);
        }
        else
        {
            StopBear();
            FaceHive();
            TryAttackHive();
        }
    }

    private void MoveToTarget(Vector3 direction)
    {
        Vector3 moveDirection = direction.normalized;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );

        Vector3 nextPosition = transform.position + moveDirection * moveSpeed * Time.fixedDeltaTime;

        if (rb != null)
            rb.MovePosition(nextPosition);
        else
            transform.position = nextPosition;

        PlayAnimation(runStateName, 0.15f);
    }

    private void FaceHive()
    {
        HiveHealth hiveHealth = hiveAttackPoint.GetComponentInParent<HiveHealth>();

        if (hiveHealth == null)
            return;

        Vector3 direction = hiveHealth.transform.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(direction.normalized);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.fixedDeltaTime
        );
    }

    private void TryAttackHive()
    {
        if (Time.time < lastAttackTime + attackCooldown)
        {
            PlayAnimation(idleStateName, 0.15f);
            return;
        }

        lastAttackTime = Time.time;
        StartCoroutine(AttackHiveRoutine());
    }

    private IEnumerator AttackHiveRoutine()
    {
        isAttacking = true;

        StopBear();
        FaceHive();

        PlayAnimation(attackStateName, 0.08f);

        yield return new WaitForSeconds(attackDamageDelay);

        HiveHealth hiveHealth = hiveAttackPoint.GetComponentInParent<HiveHealth>();

        if (hiveHealth != null)
            hiveHealth.TakeDamage(hiveDamageAmount);
        else
            Debug.LogWarning("BearEnemyAI: BearAttackPoint parentında HiveHealth yok.");

        yield return new WaitForSeconds(0.8f);

        isAttacking = false;
    }

    private void StopBear()
    {
        if (rb != null)
            rb.linearVelocity = Vector3.zero;
    }

    private void PlayAnimation(string stateName, float blendTime)
    {
        if (bearAnimator != null && !string.IsNullOrEmpty(stateName))
            bearAnimator.CrossFade(stateName, blendTime);
    }
}