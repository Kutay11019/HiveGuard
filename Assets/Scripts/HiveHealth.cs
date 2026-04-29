using TMPro;
using UnityEngine;

public class HiveHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hiveHealthText;

    private int currentHealth;

    public bool IsDestroyed => currentHealth <= 0;

    private void Start()
    {
        currentHealth = maxHealth;
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
    }

    private void HandleHiveDestroyed()
    {
        Debug.Log("Hive collapsed! Game Over.");
    }
}