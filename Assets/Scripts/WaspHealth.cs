using UnityEngine;

public class WaspHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 3;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string deathTriggerName = "Die";

    [Header("Death")]
    [SerializeField] private float destroyDelay = 1.5f;

    [Header("References")]
    [SerializeField] private HealthBarUI healthBarUI;

    private int currentHealth;
    private bool isDead;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (healthBarUI == null)
        {
            healthBarUI = GetComponentInChildren<HealthBarUI>(true);
        }

        UpdateHealthBar();
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        Debug.Log("Wasp took damage. Current health: " + currentHealth);

        UpdateHealthBar();

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarUI == null)
        {
            return;
        }

        healthBarUI.SetHealth(currentHealth, maxHealth);
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

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

        UpdateHealthBar();

        if (animator != null)
        {
            animator.SetTrigger(deathTriggerName);
        }

        Debug.Log("Wasp died.");

        Destroy(gameObject, destroyDelay);
    }
}