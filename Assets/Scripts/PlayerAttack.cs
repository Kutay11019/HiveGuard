using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask bearLayer;

    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(
            transform.position,
            attackRange,
            bearLayer,
            QueryTriggerInteraction.Collide
        );

        foreach (Collider hit in hits)
        {
            WaspHealth waspHealth = hit.GetComponent<WaspHealth>();

            if (waspHealth == null)
            {
                waspHealth = hit.GetComponentInParent<WaspHealth>();
            }

            if (waspHealth != null)
            {
                waspHealth.TakeDamage(attackDamage);
                Debug.Log("Player attacked wasp.");
                break;
            }

            BearHealth bearHealth = hit.GetComponent<BearHealth>();

            if (bearHealth == null)
            {
                bearHealth = hit.GetComponentInParent<BearHealth>();
            }

            if (bearHealth != null)
            {
                bearHealth.TakeDamage(attackDamage);
                Debug.Log("Player attacked bear.");
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}