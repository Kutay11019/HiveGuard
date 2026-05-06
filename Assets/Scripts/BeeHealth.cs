using UnityEngine;

public class BeeHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 5;

    [Header("UI")]
    [SerializeField] private HealthBarUI healthBarUI;

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

        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthBar();

        Debug.Log("Bee took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
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
        isDead = true;

        Debug.Log("Bee died.");

        if (healthBarUI != null)
        {
            healthBarUI.Hide();
        }

        // Şimdilik sadece debug bırakıyoruz.
        // İstersen sonra buraya Game Over, respawn veya movement disable ekleriz.
    }
}