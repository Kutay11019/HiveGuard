using TMPro;
using UnityEngine;

public class HiveHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hiveHealthText;
    [SerializeField] private HealthBarUI healthBarUI;

    [Header("Result Manager")]
    [SerializeField] private PrototypeResultManager resultManager;

    private int currentHealth;

    public bool IsDestroyed => currentHealth <= 0;
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

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
        UpdateHealthUI();
    }

    public void TakeDamage(int damageAmount)
    {
        if (IsDestroyed)
        {
            return;
        }

        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        UpdateHealthUI();

        Debug.Log("Hive took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            HandleHiveDestroyed();
        }
    }

    private void UpdateHealthUI()
    {
        if (hiveHealthText != null)
        {
            hiveHealthText.text = "Hive Health: " + currentHealth;
        }

        if (healthBarUI != null)
        {
            healthBarUI.SetHealth(currentHealth, maxHealth);
        }
    }

    private void HandleHiveDestroyed()
    {
        Debug.Log("Hive collapsed! Game Over.");

        if (healthBarUI != null)
        {
            healthBarUI.Hide();
        }

        if (resultManager != null)
        {
            resultManager.ShowGameOver();
        }
    }
}