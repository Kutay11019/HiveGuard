using UnityEngine;

public class BeeHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;

    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = false;
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

    [Header("Game Flow")]
    [SerializeField] private DayNightCycleManager dayNightCycleManager;

    private int currentHealth;
    private bool isDead = false;

    private Rigidbody rb;
    private bool hasRigidbody;
    private bool originalIsKinematic;
    private bool originalUseGravity;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => isDead;

    private void Awake()
    {
        ApplyStartingMaxHealthBonus();

        currentHealth = maxHealth;

        if (healthBarUI == null)
        {
            healthBarUI = GetComponentInChildren<HealthBarUI>(true);
        }

        if (beeAnimator == null)
        {
            beeAnimator = GetComponentInChildren<Animator>();
        }

        if (dayNightCycleManager == null)
        {
            dayNightCycleManager = FindFirstObjectByType<DayNightCycleManager>();
        }

        rb = GetComponent<Rigidbody>();

        if (rb != null)
        {
            hasRigidbody = true;
            originalIsKinematic = rb.isKinematic;
            originalUseGravity = rb.useGravity;
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

    public void RestoreHealth(int restoredHealth)
    {
        isDead = false;

        currentHealth = Mathf.Clamp(restoredHealth, 1, maxHealth);

        EnableSelectedComponents();
        EnableColliders();
        ResetRigidbody();
        ResetAnimator();

        UpdateHealthBar();

        Debug.Log("Bee health restored: " + currentHealth);
    }

    public void ResetHealthToFull()
    {
        RestoreHealth(maxHealth);
    }

    public void SetMaxHealth(int newMax)
    {
        maxHealth = Mathf.Max(1, newMax);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        Debug.Log("Bee max health set to " + maxHealth);
    }

    public void IncreaseMaxHealth(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        maxHealth += amount;
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateHealthBar();

        Debug.Log("Bee max health increased by " + amount + ". New max: " + maxHealth);
    }

    private void ApplyStartingMaxHealthBonus()
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        int bonus = UpgradeManager.Instance.GetMaxHealthBonus();

        if (bonus > 0)
        {
            maxHealth += bonus;
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

        PlayDeathAnimation();
        DisableSelectedComponents();
        DisableColliders();
        StopRigidbodyMovement();

        if (dayNightCycleManager != null)
        {
            dayNightCycleManager.GameOverBecauseBeeDied();
        }

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

    private void ResetAnimator()
    {
        if (beeAnimator == null)
        {
            return;
        }

        if (!string.IsNullOrEmpty(isDeadBoolName))
        {
            beeAnimator.SetBool(isDeadBoolName, false);
        }

        beeAnimator.Rebind();
        beeAnimator.Update(0f);
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

    private void EnableSelectedComponents()
    {
        foreach (MonoBehaviour component in componentsToDisable)
        {
            if (component != null)
            {
                component.enabled = true;
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

    private void EnableColliders()
    {
        Collider[] colliders = GetComponentsInChildren<Collider>(true);

        foreach (Collider col in colliders)
        {
            col.enabled = true;
        }
    }

    private void StopRigidbodyMovement()
    {
        if (!hasRigidbody || rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = true;
    }

    private void ResetRigidbody()
    {
        if (!hasRigidbody || rb == null)
        {
            return;
        }

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.isKinematic = originalIsKinematic;
        rb.useGravity = originalUseGravity;
    }
}