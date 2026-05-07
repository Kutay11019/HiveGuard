using UnityEngine;

public class PollenCollectible3D : MonoBehaviour
{
    [Header("Collectible Settings")]
    [SerializeField] private int pollenValue = 1;

    [Header("Destroy Target")]
    [SerializeField] private GameObject objectToDestroyAfterCollect;

    private bool isCollected = false;

    private void Awake()
    {
        if (objectToDestroyAfterCollect == null)
        {
            if (transform.parent != null)
            {
                objectToDestroyAfterCollect = transform.parent.gameObject;
            }
            else
            {
                objectToDestroyAfterCollect = gameObject;
            }
        }
    }

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

            if (objectToDestroyAfterCollect != null)
            {
                Destroy(objectToDestroyAfterCollect);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}