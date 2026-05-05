using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BeeAttack3D : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.6f;
    [SerializeField] private LayerMask bearLayer;

    [Header("Input Settings")]
    [SerializeField] private bool useLeftMouse = true;
    [SerializeField] private bool useFKey = true;

    [Header("References")]
    [SerializeField] private BeeAnimatorDriver beeAnimatorDriver;

    private float lastAttackTime = -999f;

    private void Awake()
    {
        if (beeAnimatorDriver == null)
        {
            beeAnimatorDriver = GetComponent<BeeAnimatorDriver>();
        }
    }

    private void Update()
    {
        if (WasAttackPressed())
        {
            TryAttack();
        }
    }

    private bool WasAttackPressed()
    {
        bool mousePressed = false;
        bool fPressed = false;

        if (useLeftMouse && Mouse.current != null)
        {
            mousePressed = Mouse.current.leftButton.wasPressedThisFrame;
        }

        if (useFKey && Keyboard.current != null)
        {
            fPressed = Keyboard.current.fKey.wasPressedThisFrame;
        }

        return mousePressed || fPressed;
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        if (beeAnimatorDriver != null)
        {
            beeAnimatorDriver.PlayAttackAnimation();
        }

        DealDamageToBearsInRange();
    }

    private void DealDamageToBearsInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            attackRange,
            bearLayer
        );

        HashSet<BearHealth> damagedBears = new HashSet<BearHealth>();

        foreach (Collider hitCollider in hitColliders)
        {
            BearHealth bearHealth = hitCollider.GetComponentInParent<BearHealth>();

            if (bearHealth == null)
            {
                continue;
            }

            if (bearHealth.IsDead)
            {
                continue;
            }

            if (damagedBears.Contains(bearHealth))
            {
                continue;
            }

            damagedBears.Add(bearHealth);
            bearHealth.TakeDamage(attackDamage);
        }

        if (damagedBears.Count == 0)
        {
            Debug.Log("Bee attacked, but no bear was in range.");
        }
        else
        {
            Debug.Log("Bee attacked bear count: " + damagedBears.Count);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}