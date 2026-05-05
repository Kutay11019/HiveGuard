using UnityEngine;

public class PollenCollectible3D : MonoBehaviour
{
    [SerializeField] private int pollenValue = 1;

    private bool isCollected = false;

    private void OnTriggerEnter(Collider other)
    {
        if (isCollected)
        {
            return;
        }

        PollenInventory inventory = other.GetComponent<PollenInventory>();

        if (inventory == null)
        {
            inventory = other.GetComponentInParent<PollenInventory>();
        }

        if (inventory != null)
        {
            isCollected = true;

            inventory.AddPollen(pollenValue);

            DayObjectiveUI objectiveUI = FindFirstObjectByType<DayObjectiveUI>();

            if (objectiveUI != null)
            {
                objectiveUI.ShowDeliverPollenObjective();
            }

            Destroy(gameObject);
        }
    }
}