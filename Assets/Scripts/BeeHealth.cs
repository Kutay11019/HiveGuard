using UnityEngine;

public class BeeHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 1.5f;

    [Header("Animation")]
    [SerializeField] private Animator beeAnimator;
    [SerializeField] private string damageTriggerName = "Damage";
    [SerializeField] private string deathTriggerName = "Death";
    [SerializeField] private string isDeadBoolName = "IsDead";

    [Header("UI")]
    [SerializeField] private HealthBarUI healthBarUI;

    [Header("Components To Disable On Death")]
    [SerializeField] private MonoBehaviour[] componentsToDisable;

    private int currentHealth;
    private bool isDead = false;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (healthBarUI == null)
        {
            healthBarUI = GetComponentInChildren<HealthBarUI>(true);
        }

        if (beeAnimator == null)
        {
            beeAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void Start()
    {
        UpdateHealthBar();
        Debug.Log("Bee health initialized: " + currentHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        if (damageAmount <= 0)
        {
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        Debug.Log("Bee took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayDamageAnimation();
        }
    }

    private void PlayDamageAnimation()
    {
        if (beeAnimator != null && !string.IsNullOrEmpty(damageTriggerName))
        {
            beeAnimator.SetTrigger(damageTriggerName);
        }
    }

    private void UpdateHealthBar()
    {
        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHealth, maxHealth);
        }
    }

    private void Die()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;

        Debug.Log("Bee died.");

        if (healthBarUI != null)
        {
            healthBarUI.Hide();
        }

        PlayDeathAnimation();

        DisableSelectedComponents();
        DisableColliders();
        StopRigidbodyMovement();

        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }

    private void PlayDeathAnimation()
    {
        if (beeAnimator == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(isDeadBoolName))
        {
            beeAnimator.SetBool(isDeadBoolName, true);
        }

        if (!string.IsNullOrEmpty(deathTriggerName))
        {
            beeAnimator.SetTrigger(deathTriggerName);
        }
    }

    private void DisableSelectedComponents()
    {
        foreach (MonoBehaviour component in componentsToDisable)
        {
            if (component != null)
            {
                component.enabled = false;
            }
        }
    }

    private void DisableColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }
    }

    private void StopRigidbodyMovement()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }
}