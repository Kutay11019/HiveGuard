using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BeeAttack3D : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.6f;

    [Header("Enemy Detection")]
    [FormerlySerializedAs("bearLayer")]
    [SerializeField] private LayerMask enemyLayer;

    [Header("Input Settings")]
    [SerializeField] private bool useLeftMouse = false;
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
        if (WasKeyboardAttackPressed())
        {
            TryAttack();
            return;
        }

        if (WasMouseAttackPressed())
        {
            TryAttack();
        }
    }

    private bool WasKeyboardAttackPressed()
    {
        if (!useFKey || Keyboard.current == null)
        {
            return false;
        }

        return Keyboard.current.fKey.wasPressedThisFrame;
    }

    private bool WasMouseAttackPressed()
    {
        if (!useLeftMouse || Mouse.current == null)
        {
            return false;
        }

        if (!Mouse.current.leftButton.wasPressedThisFrame)
        {
            return false;
        }

        if (IsPointerOverUI())
        {
            return false;
        }

        return true;
    }

    private bool IsPointerOverUI()
    {
        if (EventSystem.current == null)
        {
            return false;
        }

        return EventSystem.current.IsPointerOverGameObject();
    }

    // Attack Button OnClick burayı çağırmalı.
    public void Attack()
    {
        TryAttack();
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

        DealDamageToSingleEnemyInRange();
    }

    private void DealDamageToSingleEnemyInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer,
            QueryTriggerInteraction.Collide
        );

        BearHealth closestBear = null;
        WaspHealth closestWasp = null;

        float closestDistance = Mathf.Infinity;

        HashSet<BearHealth> checkedBears = new HashSet<BearHealth>();
        HashSet<WaspHealth> checkedWasps = new HashSet<WaspHealth>();

        foreach (Collider hitCollider in hitColliders)
        {
            BearHealth bearHealth = hitCollider.GetComponentInParent<BearHealth>();

            if (bearHealth != null)
            {
                if (bearHealth.IsDead)
                {
                    continue;
                }

                if (checkedBears.Contains(bearHealth))
                {
                    continue;
                }

                checkedBears.Add(bearHealth);

                float distanceToBear = Vector3.Distance(
                    transform.position,
                    bearHealth.transform.position
                );

                if (distanceToBear < closestDistance)
                {
                    closestDistance = distanceToBear;
                    closestBear = bearHealth;
                    closestWasp = null;
                }

                continue;
            }

            WaspHealth waspHealth = hitCollider.GetComponentInParent<WaspHealth>();

            if (waspHealth != null)
            {
                if (waspHealth.IsDead)
                {
                    continue;
                }

                if (checkedWasps.Contains(waspHealth))
                {
                    continue;
                }

                checkedWasps.Add(waspHealth);

                float distanceToWasp = Vector3.Distance(
                    transform.position,
                    waspHealth.transform.position
                );

                if (distanceToWasp < closestDistance)
                {
                    closestDistance = distanceToWasp;
                    closestBear = null;
                    closestWasp = waspHealth;
                }

                continue;
            }

            Debug.LogWarning(
                "Object was in enemy layer, but has no BearHealth or WaspHealth: "
                + hitCollider.name
            );
        }

        if (closestBear != null)
        {
            closestBear.TakeDamage(attackDamage);
            Debug.Log("Bee attacked single bear target: " + closestBear.gameObject.name);
            return;
        }

        if (closestWasp != null)
        {
            closestWasp.TakeDamage(attackDamage);
            Debug.Log("Bee attacked single wasp target: " + closestWasp.gameObject.name);
            return;
        }

        Debug.Log("Bee attacked, but no enemy was in range.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}