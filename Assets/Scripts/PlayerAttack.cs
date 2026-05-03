using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    [SerializeField] private float attackRange = 2.5f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private LayerMask bearLayer;

    public void Attack()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, bearLayer);

        foreach (Collider hit in hits)
        {
            BearHealth bearHealth = hit.GetComponent<BearHealth>();

            if (bearHealth == null)
            {
                bearHealth = hit.GetComponentInParent<BearHealth>();
            }

            if (bearHealth != null)
            {
                bearHealth.TakeDamage(attackDamage);
                break;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}