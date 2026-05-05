using UnityEngine;

public class HiveDelivery3D : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        PollenInventory inventory = other.GetComponent<PollenInventory>();

        if (inventory == null)
        {
            inventory = other.GetComponentInParent<PollenInventory>();
        }

        if (inventory != null && inventory.CurrentPollen > 0)
        {
            Debug.Log("Delivered pollen to hive: " + inventory.CurrentPollen);

            inventory.RemoveAllPollen();

            DayObjectiveUI objectiveUI = FindFirstObjectByType<DayObjectiveUI>();

            if (objectiveUI != null)
            {
                objectiveUI.ShowPollenDeliveredObjective();
            }
        }
    }
}