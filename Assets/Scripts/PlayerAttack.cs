using UnityEngine;
using UnityEngine.Serialization;

public class PlayerAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 1;

    [Header("Enemy Detection")]
    [FormerlySerializedAs("bearLayer")]
    [SerializeField] private LayerMask enemyLayer;

    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            enemyLayer,
            QueryTriggerInteraction.Collide
        );

        if (hits.Length == 0)
        {
            Debug.Log("Player attacked, but no enemy was in range.");
            return;
        }

        foreach (Collider hit in hits)
        {
            WaspHealth waspHealth = hit.GetComponentInParent<WaspHealth>();

            if (waspHealth != null)
            {
                waspHealth.TakeDamage(attackDamage);
                Debug.Log("Player attacked wasp: " + waspHealth.gameObject.name);
                return;
            }

            BearHealth bearHealth = hit.GetComponentInParent<BearHealth>();

            if (bearHealth != null)
            {
                bearHealth.TakeDamage(attackDamage);
                Debug.Log("Player attacked bear: " + bearHealth.gameObject.name);
                return;
            }

            Debug.LogWarning("Enemy layer object was found, but no BearHealth or WaspHealth was found on parent: " + hit.name);
        }

        Debug.Log("Player attacked, but no damageable enemy was found.");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}