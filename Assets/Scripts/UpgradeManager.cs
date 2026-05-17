using System;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    [Serializable]
    private class UpgradeConfig
    {
        public UpgradeType type;
        public string displayName = "Upgrade";
        public int baseCost = 5;
        public int maxLevel = 5;
        public float effectPerLevel = 1f;
        public float hiveEffectPerLevel = 0f;
    }

    [Header("Upgrade Definitions")]
    [SerializeField] private List<UpgradeConfig> upgrades = new List<UpgradeConfig>
    {
        new UpgradeConfig { type = UpgradeType.BeeSpeed,     displayName = "Bee Speed",     baseCost = 5, maxLevel = 5, effectPerLevel = 0.2f },
        new UpgradeConfig { type = UpgradeType.AttackDamage, displayName = "Attack Damage", baseCost = 8, maxLevel = 5, effectPerLevel = 1f },
        new UpgradeConfig { type = UpgradeType.MaxHealth,    displayName = "Max Health",    baseCost = 6, maxLevel = 5, effectPerLevel = 2f, hiveEffectPerLevel = 10f },
    };

    [Header("References")]
    [SerializeField] private BeeHealth beeHealth;
    [SerializeField] private HiveHealth hiveHealth;

    public static UpgradeManager Instance { get; private set; }

    public event Action OnUpgradePurchased;

    private readonly Dictionary<UpgradeType, int> levels = new Dictionary<UpgradeType, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        foreach (UpgradeConfig config in upgrades)
        {
            if (!levels.ContainsKey(config.type))
            {
                levels.Add(config.type, 0);
            }
        }

        if (beeHealth == null)
        {
            beeHealth = FindFirstObjectByType<BeeHealth>();
        }

        if (hiveHealth == null)
        {
            hiveHealth = FindFirstObjectByType<HiveHealth>();
        }
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public Dictionary<UpgradeType, int> SnapshotLevels()
    {
        return new Dictionary<UpgradeType, int>(levels);
    }

    public void RestoreLevels(Dictionary<UpgradeType, int> snapshot)
    {
        foreach (UpgradeConfig config in upgrades)
        {
            int restored = 0;

            if (snapshot != null && snapshot.TryGetValue(config.type, out int snapLevel))
            {
                restored = snapLevel;
            }

            levels[config.type] = restored;
        }

        OnUpgradePurchased?.Invoke();
    }

    public int GetLevel(UpgradeType type)
    {
        if (levels.TryGetValue(type, out int level))
        {
            return level;
        }

        return 0;
    }

    public int GetMaxLevel(UpgradeType type)
    {
        UpgradeConfig config = FindConfig(type);
        return config != null ? config.maxLevel : 0;
    }

    public string GetDisplayName(UpgradeType type)
    {
        UpgradeConfig config = FindConfig(type);
        return config != null ? config.displayName : type.ToString();
    }

    public int GetCost(UpgradeType type)
    {
        UpgradeConfig config = FindConfig(type);

        if (config == null)
        {
            return int.MaxValue;
        }

        int currentLevel = GetLevel(type);
        return config.baseCost * (currentLevel + 1);
    }

    public bool IsMaxedOut(UpgradeType type)
    {
        return GetLevel(type) >= GetMaxLevel(type);
    }

    public bool TryPurchase(UpgradeType type, PollenInventory inventory)
    {
        if (inventory == null)
        {
            Debug.LogWarning("UpgradeManager.TryPurchase: inventory is null.");
            return false;
        }

        if (IsMaxedOut(type))
        {
            Debug.Log("Upgrade is already at max level: " + type);
            return false;
        }

        int cost = GetCost(type);

        if (!inventory.TrySpendStoredPollen(cost))
        {
            Debug.Log("Not enough pollen for " + type + ". Need: " + cost + ", have: " + inventory.StoredPollen);
            return false;
        }

        levels[type] = GetLevel(type) + 1;

        ApplyUpgradeImmediateEffect(type);

        Debug.Log("Purchased " + type + " (now level " + levels[type] + "). Pollen left: " + inventory.StoredPollen);

        OnUpgradePurchased?.Invoke();

        return true;
    }

    public float GetSpeedMultiplier()
    {
        UpgradeConfig config = FindConfig(UpgradeType.BeeSpeed);

        if (config == null)
        {
            return 1f;
        }

        return 1f + GetLevel(UpgradeType.BeeSpeed) * config.effectPerLevel;
    }

    public int GetAttackBonus()
    {
        UpgradeConfig config = FindConfig(UpgradeType.AttackDamage);

        if (config == null)
        {
            return 0;
        }

        return Mathf.RoundToInt(GetLevel(UpgradeType.AttackDamage) * config.effectPerLevel);
    }

    public int GetMaxHealthBonus()
    {
        UpgradeConfig config = FindConfig(UpgradeType.MaxHealth);

        if (config == null)
        {
            return 0;
        }

        return Mathf.RoundToInt(GetLevel(UpgradeType.MaxHealth) * config.effectPerLevel);
    }

    public int GetHiveMaxHealthBonus()
    {
        UpgradeConfig config = FindConfig(UpgradeType.MaxHealth);

        if (config == null)
        {
            return 0;
        }

        return Mathf.RoundToInt(GetLevel(UpgradeType.MaxHealth) * config.hiveEffectPerLevel);
    }


    private void ApplyUpgradeImmediateEffect(UpgradeType type)
    {
        // MaxHealth runtime'da artırılmalı; Speed ve AttackDamage script'leri her frame okuyor.
        if (type != UpgradeType.MaxHealth)
        {
            return;
        }

        if (beeHealth == null)
        {
            beeHealth = FindFirstObjectByType<BeeHealth>();
        }

        if (beeHealth == null)
        {
            return;
        }

        UpgradeConfig config = FindConfig(UpgradeType.MaxHealth);
        int delta = config != null ? Mathf.RoundToInt(config.effectPerLevel) : 0;

        if (delta > 0)
        {
            beeHealth.IncreaseMaxHealth(delta);
        }

        int hiveDelta = config != null ? Mathf.RoundToInt(config.hiveEffectPerLevel) : 0;

        if (hiveDelta > 0)
        {
            if (hiveHealth == null)
            {
                hiveHealth = FindFirstObjectByType<HiveHealth>();
            }

            if (hiveHealth != null)
            {
                hiveHealth.IncreaseMaxHealth(hiveDelta);
            }
        }
    }

    private UpgradeConfig FindConfig(UpgradeType type)
    {
        foreach (UpgradeConfig config in upgrades)
        {
            if (config.type == type)
            {
                return config;
            }
        }

        return null;
    }
}
