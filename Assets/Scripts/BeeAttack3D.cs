using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class BeeAttack3D : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float attackCooldown = 0.6f;

    [FormerlySerializedAs("bearLayer")]
    [SerializeField] private LayerMask enemyLayer;

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

    // UI Button OnClick için bunu kullanabilirsin.
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

        DealDamageToEnemiesInRange();
    }

    private void DealDamageToEnemiesInRange()
    {
        Collider[] hitColliders = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer,
            QueryTriggerInteraction.Collide
        );

        HashSet<BearHealth> damagedBears = new HashSet<BearHealth>();
        HashSet<WaspHealth> damagedWasps = new HashSet<WaspHealth>();

        foreach (Collider hitCollider in hitColliders)
        {
            BearHealth bearHealth = hitCollider.GetComponentInParent<BearHealth>();

            if (bearHealth != null)
            {
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

                Debug.Log("Bee attacked bear: " + bearHealth.gameObject.name);
                continue;
            }

            WaspHealth waspHealth = hitCollider.GetComponentInParent<WaspHealth>();

            if (waspHealth != null)
            {
                if (damagedWasps.Contains(waspHealth))
                {
                    continue;
                }

                damagedWasps.Add(waspHealth);
                waspHealth.TakeDamage(attackDamage);

                Debug.Log("Bee attacked wasp: " + waspHealth.gameObject.name);
                continue;
            }

            Debug.LogWarning("Object was in enemy layer, but has no BearHealth or WaspHealth: " + hitCollider.name);
        }

        int totalDamagedEnemies = damagedBears.Count + damagedWasps.Count;

        if (totalDamagedEnemies == 0)
        {
            Debug.Log("Bee attacked, but no enemy was in range.");
        }
        else
        {
            Debug.Log("Bee attacked enemy count: " + totalDamagedEnemies);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}