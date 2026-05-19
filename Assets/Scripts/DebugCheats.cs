using UnityEngine;
using UnityEngine.InputSystem;

public class DebugCheats : MonoBehaviour
{
    [Header("Master")]
    [SerializeField] private bool cheatsEnabled = true;

    [Header("Bonus Pollen")]
    [SerializeField] private Key bonusPollenKey = Key.O;
    [SerializeField] private int bonusPollenAmount = 40;
    [SerializeField] private bool requireDayPhase = true;

    [Header("References (auto-found if empty)")]
    [SerializeField] private PollenInventory pollenInventory;
    [SerializeField] private DayNightCycleManager dayNightCycle;

    private void Awake()
    {
        if (pollenInventory == null)
        {
            pollenInventory = FindFirstObjectByType<PollenInventory>();
        }

        if (dayNightCycle == null)
        {
            dayNightCycle = FindFirstObjectByType<DayNightCycleManager>();
        }
    }

    private void Update()
    {
        if (!cheatsEnabled)
        {
            return;
        }

        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return;
        }

        if (!keyboard[bonusPollenKey].wasPressedThisFrame)
        {
            return;
        }

        TryGrantBonusPollen();
    }

    private void TryGrantBonusPollen()
    {
        if (pollenInventory == null)
        {
            pollenInventory = FindFirstObjectByType<PollenInventory>();
        }

        if (pollenInventory == null)
        {
            Debug.LogWarning("DebugCheats: PollenInventory not found in scene.");
            return;
        }

        if (requireDayPhase)
        {
            if (dayNightCycle == null)
            {
                dayNightCycle = FindFirstObjectByType<DayNightCycleManager>();
            }

            if (dayNightCycle == null)
            {
                Debug.LogWarning("DebugCheats: DayNightCycleManager not found, cannot validate phase.");
                return;
            }

            if (!dayNightCycle.IsDayPhase)
            {
                Debug.Log("DebugCheats: Bonus pollen only granted during day phase. Ignored.");
                return;
            }
        }

        int newTotal = pollenInventory.StoredPollen + bonusPollenAmount;
        pollenInventory.SetStoredPollen(newTotal);
        Debug.Log("Cheat: +" + bonusPollenAmount + " stored pollen → total " + newTotal);
    }
}
