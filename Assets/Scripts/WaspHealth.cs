using UnityEngine;

public class WaspHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 1;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTriggerName = "Die";

    [Header("Death")]
    [SerializeField] private float destroyDelay = 1.5f;

    private int currentHealth;
    private bool isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log("Wasp took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        WaspEnemy waspEnemy = GetComponent<WaspEnemy>();

        if (waspEnemy != null)
        {
            waspEnemy.enabled = false;
        }

        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        if (animator != null)
        {
            animator.SetTrigger(deathTriggerName);
        }

        Debug.Log("Wasp died.");

        Destroy(gameObject, destroyDelay);
    }
}