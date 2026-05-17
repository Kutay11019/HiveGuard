using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class NightTransitionPanel : MonoBehaviour
{
    [Serializable]
    public class UpgradeRow
    {
        public UpgradeType type;
        public TextMeshProUGUI nameText;
        public TextMeshProUGUI levelText;
        public TextMeshProUGUI costText;
        public Button purchaseButton;
    }

    public GameObject rootPanel;
    public GameObject choicePanel;
    public GameObject upgradePanel;

    public Button continueButton;
    public Button openUpgradesButton;
    public Button closeUpgradesButton;

    public UpgradeRow[] upgradeRows;

    public UpgradeManager upgradeManager;
    public PollenInventory pollenInventory;
    public TextMeshProUGUI pollenAmountText;

    public event Action OnContinueRequested;

    private bool isShowing;
    private bool uiBuilt;

    private void Awake()
    {
        BuildUI();
        WireButtons();

        if (upgradeManager == null)
        {
            upgradeManager = FindFirstObjectByType<UpgradeManager>();
        }

        if (pollenInventory == null)
        {
            pollenInventory = FindFirstObjectByType<PollenInventory>();
        }
    }

    private void OnEnable()
    {
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradePurchased += RefreshUpgradeUI;
        }
    }

    private void OnDisable()
    {
        if (upgradeManager != null)
        {
            upgradeManager.OnUpgradePurchased -= RefreshUpgradeUI;
        }
    }

    private void Start()
    {
        if (rootPanel != null)
        {
            rootPanel.SetActive(false);
        }
    }

    public void Show()
    {
        if (isShowing)
        {
            return;
        }

        isShowing = true;
        Time.timeScale = 0f;

        if (rootPanel != null) rootPanel.SetActive(true);
        if (choicePanel != null) choicePanel.SetActive(true);
        if (upgradePanel != null) upgradePanel.SetActive(false);

        RefreshUpgradeUI();
    }

    public void Hide()
    {
        isShowing = false;
        Time.timeScale = 1f;

        if (rootPanel != null) rootPanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(false);
    }

    private void HandleContinueClicked()
    {
        Hide();
        OnContinueRequested?.Invoke();
    }

    private void HandleOpenUpgradesClicked()
    {
        if (choicePanel != null) choicePanel.SetActive(false);
        if (upgradePanel != null) upgradePanel.SetActive(true);
        RefreshUpgradeUI();
    }

    private void HandleCloseUpgradesClicked()
    {
        if (upgradePanel != null) upgradePanel.SetActive(false);
        if (choicePanel != null) choicePanel.SetActive(true);
    }

    private void HandlePurchaseClicked(UpgradeType type)
    {
        if (upgradeManager == null || pollenInventory == null)
        {
            return;
        }

        upgradeManager.TryPurchase(type, pollenInventory);
    }

    private void RefreshUpgradeUI()
    {
        if (pollenAmountText != null && pollenInventory != null)
        {
            pollenAmountText.text = "Pollen: " + pollenInventory.CurrentPollen;
        }

        if (upgradeRows == null || upgradeManager == null)
        {
            return;
        }

        foreach (UpgradeRow row in upgradeRows)
        {
            if (row == null) continue;

            int level = upgradeManager.GetLevel(row.type);
            int maxLevel = upgradeManager.GetMaxLevel(row.type);
            bool isMax = upgradeManager.IsMaxedOut(row.type);

            if (row.nameText != null) row.nameText.text = upgradeManager.GetDisplayName(row.type);
            if (row.levelText != null) row.levelText.text = "Lv " + level + " / " + maxLevel;
            if (row.costText != null) row.costText.text = isMax ? "MAX" : ("Cost: " + upgradeManager.GetCost(row.type));

            if (row.purchaseButton != null)
            {
                bool canAfford = pollenInventory != null && pollenInventory.CurrentPollen >= upgradeManager.GetCost(row.type);
                row.purchaseButton.interactable = !isMax && canAfford;
            }
        }
    }

    private void WireButtons()
    {
        if (continueButton != null)
        {
            continueButton.onClick.RemoveListener(HandleContinueClicked);
            continueButton.onClick.AddListener(HandleContinueClicked);
        }

        if (openUpgradesButton != null)
        {
            openUpgradesButton.onClick.RemoveListener(HandleOpenUpgradesClicked);
            openUpgradesButton.onClick.AddListener(HandleOpenUpgradesClicked);
        }

        if (closeUpgradesButton != null)
        {
            closeUpgradesButton.onClick.RemoveListener(HandleCloseUpgradesClicked);
            closeUpgradesButton.onClick.AddListener(HandleCloseUpgradesClicked);
        }

        if (upgradeRows != null)
        {
            foreach (UpgradeRow row in upgradeRows)
            {
                if (row == null || row.purchaseButton == null) continue;

                UpgradeType captured = row.type;
                row.purchaseButton.onClick.RemoveAllListeners();
                row.purchaseButton.onClick.AddListener(() => HandlePurchaseClicked(captured));
            }
        }
    }

    // ---------- Procedural UI builder ----------

    private void BuildUI()
    {
        if (uiBuilt) return;
        if (rootPanel != null && choicePanel != null && upgradePanel != null)
        {
            uiBuilt = true;
            return;
        }

        rootPanel = gameObject;
        ConfigureFullScreen(GetComponent<RectTransform>());

        Image rootImage = GetComponent<Image>();
        if (rootImage == null) rootImage = gameObject.AddComponent<Image>();
        rootImage.color = new Color(0f, 0f, 0f, 0.65f);
        rootImage.raycastTarget = true;

        choicePanel = CreatePanel("ChoicePanel", rootPanel.transform, new Vector2(600, 400), new Color(0.12f, 0.1f, 0.18f, 0.95f));
        CreateText("Title", choicePanel.transform, "The night is coming!", 42, new Vector2(0, 130), new Vector2(520, 80), new Color(1f, 0.95f, 0.8f));
        continueButton = CreateButton("ContinueButton", choicePanel.transform, "Continue to Wave", new Vector2(0, 20), new Vector2(420, 90), new Color(0.85f, 0.65f, 0.15f), new Color(0.1f, 0.1f, 0.1f));
        openUpgradesButton = CreateButton("UpgradesButton", choicePanel.transform, "View Upgrades", new Vector2(0, -90), new Vector2(420, 90), new Color(0.35f, 0.55f, 0.85f), Color.white);

        upgradePanel = CreatePanel("UpgradePanel", rootPanel.transform, new Vector2(700, 520), new Color(0.12f, 0.1f, 0.18f, 0.95f));
        CreateText("Title", upgradePanel.transform, "Upgrades", 38, new Vector2(0, 215), new Vector2(500, 60), new Color(1f, 0.95f, 0.8f));
        pollenAmountText = CreateText("PollenAmountText", upgradePanel.transform, "Pollen: 0", 26, new Vector2(-30, 160), new Vector2(320, 40), new Color(1f, 0.85f, 0.4f));
        closeUpgradesButton = CreateButton("CloseButton", upgradePanel.transform, "X", new Vector2(295, 215), new Vector2(60, 60), new Color(0.8f, 0.25f, 0.25f), Color.white);

        upgradeRows = new UpgradeRow[]
        {
            BuildRow(UpgradeType.BeeSpeed, "SpeedRow", new Vector2(0, 70)),
            BuildRow(UpgradeType.AttackDamage, "AttackRow", new Vector2(0, -30)),
            BuildRow(UpgradeType.MaxHealth, "HealthRow", new Vector2(0, -130)),
        };

        uiBuilt = true;
    }

    private UpgradeRow BuildRow(UpgradeType type, string rowName, Vector2 anchoredPos)
    {
        GameObject rowGo = CreatePanel(rowName, upgradePanel.transform, new Vector2(640, 80), new Color(0.2f, 0.2f, 0.3f, 0.7f));
        rowGo.GetComponent<RectTransform>().anchoredPosition = anchoredPos;

        TextMeshProUGUI nameTxt = CreateText("NameText", rowGo.transform, "Name", 24, new Vector2(-200, 0), new Vector2(200, 60), Color.white);
        nameTxt.alignment = TextAlignmentOptions.MidlineLeft;
        TextMeshProUGUI levelTxt = CreateText("LevelText", rowGo.transform, "Lv 0/5", 22, new Vector2(-20, 0), new Vector2(120, 60), new Color(0.8f, 0.85f, 1f));
        TextMeshProUGUI costTxt = CreateText("CostText", rowGo.transform, "Cost: 0", 22, new Vector2(110, 0), new Vector2(140, 60), new Color(1f, 0.85f, 0.4f));
        Button btn = CreateButton("PurchaseButton", rowGo.transform, "Buy", new Vector2(250, 0), new Vector2(130, 60), new Color(0.35f, 0.7f, 0.4f), Color.white);

        return new UpgradeRow
        {
            type = type,
            nameText = nameTxt,
            levelText = levelTxt,
            costText = costTxt,
            purchaseButton = btn,
        };
    }

    private GameObject CreatePanel(string name, Transform parent, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        Image img = go.GetComponent<Image>();
        img.color = color;

        return go;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        TextMeshProUGUI tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text = text;
        tmp.fontSize = fontSize;
        tmp.color = color;
        tmp.alignment = TextAlignmentOptions.Center;

        return tmp;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 anchoredPos, Vector2 size, Color bgColor, Color textColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        go.layer = LayerMask.NameToLayer("UI");
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        Image img = go.GetComponent<Image>();
        img.color = bgColor;

        Button btn = go.GetComponent<Button>();
        btn.targetGraphic = img;

        TextMeshProUGUI labelText = CreateText("Label", go.transform, label, 28f, Vector2.zero, size, textColor);
        labelText.rectTransform.anchorMin = Vector2.zero;
        labelText.rectTransform.anchorMax = Vector2.one;
        labelText.rectTransform.offsetMin = Vector2.zero;
        labelText.rectTransform.offsetMax = Vector2.zero;
        labelText.rectTransform.sizeDelta = Vector2.zero;

        return btn;
    }

    private void ConfigureFullScreen(RectTransform rt)
    {
        if (rt == null) return;

        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;
    }
}
