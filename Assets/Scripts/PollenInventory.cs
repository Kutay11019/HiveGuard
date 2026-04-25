using UnityEngine;

public class PollenInventory : MonoBehaviour
{
    public int CurrentPollen { get; private set; }

    public void AddPollen(int amount)
    {
        CurrentPollen += amount;
        Debug.Log("Pollen collected. Current pollen: " + CurrentPollen);
    }

    public void RemoveAllPollen()
    {
        CurrentPollen = 0;
        Debug.Log("Pollen delivered. Inventory is now empty.");
    }
}