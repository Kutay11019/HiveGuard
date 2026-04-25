using TMPro;
using UnityEngine;

public class GameplayUIManager : MonoBehaviour
{
    [SerializeField] private PollenInventory pollenInventory;
    [SerializeField] private TextMeshProUGUI pollenCounterText;

    private void Update()
    {
        if (pollenInventory != null && pollenCounterText != null)
        {
            pollenCounterText.text = "Pollen: " + pollenInventory.CurrentPollen;
        }
    }
}