using UnityEngine;

public class PollenCollectible3D : MonoBehaviour
{
    [SerializeField] private int pollenValue = 1;

    private void OnTriggerEnter(Collider other)
    {
        PollenInventory inventory = other.GetComponent<PollenInventory>();

        if (inventory != null)
        {
            inventory.AddPollen(pollenValue);
            Destroy(gameObject);
        }
    }
}