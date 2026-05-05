using TMPro;
using UnityEngine;

public class DayObjectiveUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject objectivePanel;
    [SerializeField] private TextMeshProUGUI objectiveText;

    [Header("Objective Messages")]
    [SerializeField] private string collectPollenMessage = "Objective: Collect pollen from flowers.";
    [SerializeField] private string deliverPollenMessage = "Objective: Deliver pollen to the hive.";
    [SerializeField] private string pollenDeliveredMessage = "Pollen delivered! Collect more pollen before night.";
    [SerializeField] private string nightMessage = "Night Objective: Defend the hive from bears.";

    private bool isDayPhase = true;

    private void Awake()
    {
        if (objectiveText == null)
        {
            objectiveText = GetComponentInChildren<TextMeshProUGUI>();
        }

        if (objectivePanel == null)
        {
            objectivePanel = gameObject;
        }
    }

    private void Start()
    {
        ShowCollectPollenObjective();
    }

    public void ShowDayObjective()
    {
        isDayPhase = true;
        ShowCollectPollenObjective();
    }

    public void ShowCollectPollenObjective()
    {
        if (!isDayPhase)
        {
            return;
        }

        SetObjectiveText(collectPollenMessage);
    }

    public void ShowDeliverPollenObjective()
    {
        if (!isDayPhase)
        {
            return;
        }

        SetObjectiveText(deliverPollenMessage);
    }

    public void ShowPollenDeliveredObjective()
    {
        if (!isDayPhase)
        {
            return;
        }

        SetObjectiveText(pollenDeliveredMessage);

        CancelInvoke(nameof(ShowCollectPollenObjective));
        Invoke(nameof(ShowCollectPollenObjective), 1.5f);
    }

    public void ShowNightObjective()
    {
        isDayPhase = false;
        SetObjectiveText(nightMessage);
    }

    public void HideObjective()
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(false);
        }
    }

    private void SetObjectiveText(string message)
    {
        if (objectivePanel != null)
        {
            objectivePanel.SetActive(true);
        }

        if (objectiveText != null)
        {
            objectiveText.text = message;
        }
    }
}