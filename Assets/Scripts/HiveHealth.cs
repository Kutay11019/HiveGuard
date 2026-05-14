using TMPro;
using UnityEngine;

public class HiveHealth : MonoBehaviour
{
    [Header("Health")]
    [SerializeField] private int maxHealth = 100;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI hiveHealthText;

    [Header("Game Flow")]
    [SerializeField] private DayNightCycleManager dayNightCycleManager;

    private int currentHealth;
    private bool isDestroyed;

    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDestroyed => isDestroyed;

    private void Awake()
    {
        currentHealth = maxHealth;
        isDestroyed = false;

        if (dayNightCycleManager == null)
        {
            dayNightCycleManager = FindFirstObjectByType<DayNightCycleManager>();
        }
    }

    private void Start()
    {
        UpdateUI();
        Debug.Log("Hive health initialized: " + currentHealth);
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDestroyed)
        {
            return;
        }

        if (damageAmount <= 0)
        {
            return;
        }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        UpdateUI();

        Debug.Log("Hive took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            DestroyHive();
        }
    }

    public void RestoreHealth(int restoredHealth)
    {
        isDestroyed = false;

        currentHealth = Mathf.Clamp(restoredHealth, 1, maxHealth);

        UpdateUI();

        Debug.Log("Hive health restored: " + currentHealth);
    }

    public void ResetHealthToFull()
    {
        isDestroyed = false;
        currentHealth = maxHealth;

        UpdateUI();

        Debug.Log("Hive health reset to full: " + currentHealth);
    }

    private void DestroyHive()
    {
        if (isDestroyed)
        {
            return;
        }

        isDestroyed = true;

        Debug.Log("Hive destroyed.");

        if (dayNightCycleManager != null)
        {
            dayNightCycleManager.GameOverBecauseHiveDestroyed();
        }
    }

    private void UpdateUI()
    {
        if (hiveHealthText != null)
        {
            hiveHealthText.text = "Hive Health:\n" + currentHealth;
        }
    }
}