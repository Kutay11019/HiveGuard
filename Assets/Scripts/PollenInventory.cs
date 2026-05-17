using UnityEngine;

public class PollenInventory : MonoBehaviour
{
    public int CurrentPollen { get; private set; }
    public int StoredPollen { get; private set; }

    public void AddPollen(int amount)
    {
        CurrentPollen += amount;
        Debug.Log("Pollen collected. Current pollen: " + CurrentPollen);
    }

    public int DepositCarriedToHive()
    {
        int deposited = CurrentPollen;
        StoredPollen += deposited;
        CurrentPollen = 0;
        Debug.Log("Pollen delivered. Deposited: " + deposited + ", hive total: " + StoredPollen);
        return deposited;
    }

    public void RemoveAllPollen()
    {
        CurrentPollen = 0;
        Debug.Log("Pollen carried inventory cleared.");
    }

    public void SetStoredPollen(int amount)
    {
        StoredPollen = Mathf.Max(0, amount);
        Debug.Log("Hive pollen set to " + StoredPollen);
    }

    public bool TrySpendStoredPollen(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (StoredPollen < amount)
        {
            return false;
        }

        StoredPollen -= amount;
        Debug.Log("Spent " + amount + " stored pollen. Hive remaining: " + StoredPollen);
        return true;
    }

    public bool TrySpendPollen(int amount)
    {
        if (amount <= 0)
        {
            return false;
        }

        if (CurrentPollen < amount)
        {
            return false;
        }

        CurrentPollen -= amount;
        Debug.Log("Spent " + amount + " carried pollen. Remaining: " + CurrentPollen);
        return true;
    }
}
