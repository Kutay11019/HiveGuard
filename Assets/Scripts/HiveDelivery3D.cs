using UnityEngine;

public class HiveDelivery3D : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private DayNightCycleManager dayNightCycleManager;

    private DayObjectiveUI objectiveUI;

    private void Awake()
    {
        if (dayNightCycleManager == null)
        {
            dayNightCycleManager = FindFirstObjectByType<DayNightCycleManager>();
        }

        objectiveUI = FindFirstObjectByType<DayObjectiveUI>();
    }

    private void OnTriggerEnter(Collider other)
    {
        TryDeliverPollen(other);
    }

    private void TryDeliverPollen(Collider other)
    {
        PollenInventory inventory = other.GetComponent<PollenInventory>();

        if (inventory == null)
        {
            inventory = other.GetComponentInParent<PollenInventory>();
        }

        if (inventory == null)
        {
            return;
        }

        if (inventory.CurrentPollen <= 0)
        {
            return;
        }

        if (dayNightCycleManager != null && !dayNightCycleManager.CanDeliverPollen)
        {
            Debug.Log("Pollen cannot be delivered at night.");
            return;
        }

        Debug.Log("Delivered pollen to hive: " + inventory.CurrentPollen);

        inventory.RemoveAllPollen();

        if (objectiveUI != null)
        {
            objectiveUI.ShowPollenDeliveredObjective();
        }
    }
}