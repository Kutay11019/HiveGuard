using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Scene Settings")]
    [SerializeField] private string gameplaySceneName = "GameScene";

    private static MainMenuController activeInstance;
    private static Sprite cachedHoneyDropSprite;

    private GameObject mainMenuPanel;
    private GameObject howToPlayPanel;

    private bool uiBuilt;

    private void Awake()
    {
        if (activeInstance != null && activeInstance != this)
        {
            Destroy(this);
            return;
        }

        activeInstance = this;

        BuildUI();
    }

    private void Start()
    {
        ShowMainMenu();
    }

    private void OnDestroy()
    {
        if (activeInstance == this)
        {
            activeInstance = null;
        }
    }

    public void StartGame()
    {
        if (string.IsNullOrWhiteSpace(gameplaySceneName))
        {
            Debug.LogError("Gameplay scene name is empty! Please set it in the Inspector.");
            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ShowHowToPlay()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
        if (howToPlayPanel != null) howToPlayPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Debug.Log("Exit button pressed.");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // ---------- Procedural UI builder ----------

    private void BuildUI()
    {
        if (uiBuilt) return;

        Canvas canvas = FindFirstObjectByType<Canvas>();

        if (canvas == null)
        {
            GameObject canvasGo = new GameObject("MenuCanvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas = canvasGo.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            CanvasScaler scaler = canvasGo.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.matchWidthOrHeight = 0.5f;
        }

        if (FindFirstObjectByType<UnityEngine.EventSystems.EventSystem>() == null)
        {
            new GameObject("EventSystem",
                typeof(UnityEngine.EventSystems.EventSystem),
                typeof(UnityEngine.EventSystems.StandaloneInputModule));
        }

        // Only remove the legacy scene-authored panels we're replacing.
        DestroyByName(canvas.transform, "MainMenuPanel");
        DestroyByName(canvas.transform, "HowToPlayPanel");
        DestroyByName(canvas.transform, "ExitButton");
        DestroyByName(canvas.transform, "TitleText");
        DestroyByName(canvas.transform, "TitleHalo");

        BuildMainMenuPanel(canvas.transform);
        BuildHowToPlayPanel(canvas.transform);

        uiBuilt = true;
    }

    private void DestroyByName(Transform parent, string targetName)
    {
        for (int i = parent.childCount - 1; i >= 0; i--)
        {
            Transform child = parent.GetChild(i);
            if (child.name == targetName)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    private void BuildMainMenuPanel(Transform parent)
    {
        mainMenuPanel = new GameObject("MainMenuPanel", typeof(RectTransform));
        mainMenuPanel.transform.SetParent(parent, false);
        ConfigureFullScreen(mainMenuPanel.GetComponent<RectTransform>());

        // Honey halo glow behind the plaque.
        GameObject halo = CreatePanel("TitleHalo", mainMenuPanel.transform, new Vector2(1200f, 800f), new Color(1f, 0.62f, 0.10f, 0.22f));
        halo.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Dark wax-brown plaque that gives the title and buttons proper contrast.
        GameObject plaque = CreatePanel(
            "TitlePlaque",
            mainMenuPanel.transform,
            new Vector2(920f, 720f),
            new Color(0.16f, 0.10f, 0.06f, 0.92f));
        plaque.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Inner border accent (lighter brown ring).
        GameObject innerBorder = CreatePanel(
            "PlaqueInner",
            plaque.transform,
            new Vector2(880f, 680f),
            new Color(0.22f, 0.14f, 0.08f, 0.85f));
        innerBorder.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Title drop-shadow (bigger offset + darker so it reads as deliberate).
        CreateText(
            "TitleShadow",
            plaque.transform,
            "HIVEGUARD",
            120f,
            new Vector2(6f, 240f - 10f),
            new Vector2(1200f, 180f),
            new Color(0.05f, 0.02f, 0f, 0.85f));

        TextMeshProUGUI title = CreateText(
            "Title",
            plaque.transform,
            "HIVEGUARD",
            120f,
            new Vector2(0f, 240f),
            new Vector2(1200f, 180f),
            new Color(1f, 0.78f, 0.20f, 1f));
        title.fontStyle = FontStyles.Bold;
        title.characterSpacing = 4f;

        CreateText(
            "Subtitle",
            plaque.transform,
            "Defend the Hive!",
            38f,
            new Vector2(0f, 150f),
            new Vector2(800f, 50f),
            new Color(1f, 0.85f, 0.30f, 1f));

        // Honey accent divider under the subtitle.
        GameObject divider = new GameObject("Divider", typeof(RectTransform), typeof(Image));
        divider.transform.SetParent(plaque.transform, false);
        RectTransform divRt = divider.GetComponent<RectTransform>();
        divRt.anchorMin = new Vector2(0.5f, 0.5f);
        divRt.anchorMax = new Vector2(0.5f, 0.5f);
        divRt.pivot = new Vector2(0.5f, 0.5f);
        divRt.sizeDelta = new Vector2(520f, 4f);
        divRt.anchoredPosition = new Vector2(0f, 100f);
        divider.GetComponent<Image>().color = new Color(1f, 0.78f, 0.20f, 0.6f);

        Color primaryBg = new Color(0.95f, 0.65f, 0.15f, 1f);
        Color primaryTxt = new Color(0.15f, 0.08f, 0.02f, 1f);
        Color secondaryBg = new Color(0.30f, 0.55f, 0.55f, 1f);
        Color tertiaryBg = new Color(0.65f, 0.22f, 0.18f, 1f);
        Color cream = new Color(1f, 0.95f, 0.85f, 1f);

        Sprite honeyDrop = GetHoneyDropSprite();

        Color darkDropTint = new Color(0.45f, 0.25f, 0.05f, 1f);

        Button startBtn = CreateButtonWithIcon(
            "StartButton", plaque.transform, "Start Game",
            new Vector2(0f, -10f), new Vector2(520f, 95f),
            primaryBg, primaryTxt, honeyDrop, darkDropTint);
        startBtn.onClick.AddListener(StartGame);

        Button howBtn = CreateButtonWithIcon(
            "HowToPlayButton", plaque.transform, "How to Play",
            new Vector2(0f, -125f), new Vector2(520f, 85f),
            secondaryBg, cream, honeyDrop, Color.white);
        howBtn.onClick.AddListener(ShowHowToPlay);

        Button exitBtn = CreateButtonWithIcon(
            "ExitButton", plaque.transform, "Exit",
            new Vector2(0f, -235f), new Vector2(520f, 85f),
            tertiaryBg, cream, honeyDrop, Color.white);
        exitBtn.onClick.AddListener(ExitGame);

        CreateText(
            "Footer",
            plaque.transform,
            "Ceng 361 - HiveGuard",
            22f,
            new Vector2(0f, -320f),
            new Vector2(600f, 30f),
            new Color(1f, 0.85f, 0.50f, 0.75f));
    }

    private void BuildHowToPlayPanel(Transform parent)
    {
        howToPlayPanel = new GameObject("HowToPlayPanel", typeof(RectTransform));
        howToPlayPanel.transform.SetParent(parent, false);
        ConfigureFullScreen(howToPlayPanel.GetComponent<RectTransform>());

        GameObject card = CreatePanel(
            "HowToPlayCard",
            howToPlayPanel.transform,
            new Vector2(920f, 680f),
            new Color(0.16f, 0.10f, 0.06f, 0.94f));

        GameObject inner = CreatePanel(
            "HowToPlayInner",
            card.transform,
            new Vector2(880f, 640f),
            new Color(0.22f, 0.14f, 0.08f, 0.85f));
        inner.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        CreateText(
            "HowTitleShadow",
            card.transform,
            "How to Play",
            64f,
            new Vector2(4f, 240f - 8f),
            new Vector2(800f, 80f),
            new Color(0.05f, 0.02f, 0f, 0.85f));

        TextMeshProUGUI howTitle = CreateText(
            "HowTitle",
            card.transform,
            "How to Play",
            64f,
            new Vector2(0f, 240f),
            new Vector2(800f, 80f),
            new Color(1f, 0.78f, 0.20f, 1f));
        howTitle.fontStyle = FontStyles.Bold;
        howTitle.characterSpacing = 2f;

        string instructions =
            "<color=#FFD040>•</color>  Use <b>WASD</b> or arrow keys to fly the bee.\n" +
            "<color=#FFD040>•</color>  Collect pollen from flowers during the day.\n" +
            "<color=#FFD040>•</color>  Deliver pollen to the hive before night falls.\n" +
            "<color=#FFD040>•</color>  Defend the hive from bears and wasps at night.\n" +
            "<color=#FFD040>•</color>  Spend hive pollen on upgrades between waves.\n" +
            "<color=#FFD040>•</color>  Survive all 5 days to win the game.";

        TextMeshProUGUI body = CreateText(
            "HowBody",
            card.transform,
            instructions,
            28f,
            new Vector2(0f, 30f),
            new Vector2(820f, 340f),
            new Color(1f, 0.92f, 0.78f, 1f));
        body.alignment = TextAlignmentOptions.TopLeft;
        body.lineSpacing = 10f;
        body.richText = true;

        Button backBtn = CreateButtonWithIcon(
            "BackButton", card.transform, "Back",
            new Vector2(0f, -260f), new Vector2(360f, 80f),
            new Color(0.30f, 0.55f, 0.55f, 1f),
            new Color(1f, 0.95f, 0.85f, 1f),
            GetHoneyDropSprite(), Color.white);
        backBtn.onClick.AddListener(ShowMainMenu);

        howToPlayPanel.SetActive(false);
    }

    // ---------- Honey drop sprite ----------

    private Sprite GetHoneyDropSprite()
    {
        if (cachedHoneyDropSprite != null)
        {
            return cachedHoneyDropSprite;
        }

        const int w = 48;
        const int h = 72;
        const float halfW = w * 0.5f;
        const float bulbHeight = halfW;
        const float topY = h - 1f;

        Texture2D tex = new Texture2D(w, h, TextureFormat.RGBA32, false);
        tex.filterMode = FilterMode.Bilinear;
        tex.wrapMode = TextureWrapMode.Clamp;

        Color clear = new Color(0f, 0f, 0f, 0f);
        Color topColor = new Color(1f, 0.92f, 0.40f, 1f);
        Color bottomColor = new Color(0.85f, 0.58f, 0.08f, 1f);
        Color highlight = new Color(1f, 0.98f, 0.85f, 1f);

        Color[] pixels = new Color[w * h];
        for (int i = 0; i < pixels.Length; i++) pixels[i] = clear;

        for (int y = 0; y < h; y++)
        {
            float rowWidth;
            if (y < bulbHeight)
            {
                float dy = bulbHeight - y;
                float inside = halfW * halfW - dy * dy;
                rowWidth = inside > 0f ? Mathf.Sqrt(inside) : 0f;
            }
            else
            {
                float t = (y - bulbHeight) / (topY - bulbHeight);
                t = Mathf.Clamp01(t);
                rowWidth = halfW * Mathf.Pow(1f - t, 0.7f);
            }

            float gradient = Mathf.Clamp01((float)y / topY);
            Color rowColor = Color.Lerp(bottomColor, topColor, gradient);

            float left = halfW - rowWidth;
            float right = halfW + rowWidth;

            for (int x = 0; x < w; x++)
            {
                if (x < left || x > right) continue;

                Color px = rowColor;

                // Subtle edge darkening near the silhouette to add roundness.
                float edgeDist = Mathf.Min(x - left, right - x);
                if (edgeDist < 1.5f)
                {
                    px = Color.Lerp(rowColor, new Color(0.45f, 0.28f, 0.05f, 1f), 0.4f);
                }

                pixels[y * w + x] = px;
            }
        }

        // Gloss highlight near upper-right of the bulb.
        int glossCx = (int)(halfW + 4f);
        int glossCy = (int)(bulbHeight + 2f);
        for (int gy = -3; gy <= 3; gy++)
        {
            for (int gx = -2; gx <= 2; gx++)
            {
                int px = glossCx + gx;
                int py = glossCy + gy;
                if (px < 0 || py < 0 || px >= w || py >= h) continue;
                if (pixels[py * w + px].a < 0.5f) continue;
                float d = Mathf.Sqrt(gx * gx + gy * gy);
                if (d > 2.5f) continue;
                pixels[py * w + px] = Color.Lerp(pixels[py * w + px], highlight, 1f - d / 2.5f);
            }
        }

        tex.SetPixels(pixels);
        tex.Apply(false);

        cachedHoneyDropSprite = Sprite.Create(tex, new Rect(0f, 0f, w, h), new Vector2(0.5f, 0.5f), 100f);
        cachedHoneyDropSprite.name = "HoneyDropSprite";

        return cachedHoneyDropSprite;
    }

    // ---------- UI primitives ----------

    private GameObject CreatePanel(string name, Transform parent, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image));
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0) go.layer = uiLayer;
        go.transform.SetParent(parent, false);

        RectTransform rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = size;
        rt.localScale = Vector3.one;
        rt.localRotation = Quaternion.identity;

        go.GetComponent<Image>().color = color;
        return go;
    }

    private TextMeshProUGUI CreateText(string name, Transform parent, string text, float fontSize, Vector2 anchoredPos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(name, typeof(RectTransform));
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0) go.layer = uiLayer;
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
        tmp.enableWordWrapping = true;

        return tmp;
    }

    private Button CreateButton(string name, Transform parent, string label, Vector2 anchoredPos, Vector2 size, Color bgColor, Color textColor)
    {
        GameObject go = new GameObject(name, typeof(RectTransform), typeof(Image), typeof(Button));
        int uiLayer = LayerMask.NameToLayer("UI");
        if (uiLayer >= 0) go.layer = uiLayer;
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

        ColorBlock cb = btn.colors;
        cb.normalColor = Color.white;
        cb.highlightedColor = new Color(1.15f, 1.15f, 1.15f, 1f);
        cb.pressedColor = new Color(0.85f, 0.85f, 0.85f, 1f);
        cb.selectedColor = Color.white;
        cb.colorMultiplier = 1f;
        btn.colors = cb;

        TextMeshProUGUI labelText = CreateText("Label", go.transform, label, 32f, Vector2.zero, size, textColor);
        labelText.fontStyle = FontStyles.Bold;
        labelText.rectTransform.anchorMin = Vector2.zero;
        labelText.rectTransform.anchorMax = Vector2.one;
        labelText.rectTransform.offsetMin = Vector2.zero;
        labelText.rectTransform.offsetMax = Vector2.zero;
        labelText.rectTransform.sizeDelta = Vector2.zero;

        return btn;
    }

    private Button CreateButtonWithIcon(
        string name,
        Transform parent,
        string label,
        Vector2 anchoredPos,
        Vector2 size,
        Color bgColor,
        Color textColor,
        Sprite icon,
        Color iconTint)
    {
        Button btn = CreateButton(name, parent, label, anchoredPos, size, bgColor, textColor);

        if (icon != null)
        {
            GameObject iconGo = new GameObject("Icon", typeof(RectTransform), typeof(Image));
            int uiLayer = LayerMask.NameToLayer("UI");
            if (uiLayer >= 0) iconGo.layer = uiLayer;
            iconGo.transform.SetParent(btn.transform, false);

            RectTransform iconRt = iconGo.GetComponent<RectTransform>();
            iconRt.anchorMin = new Vector2(0f, 0.5f);
            iconRt.anchorMax = new Vector2(0f, 0.5f);
            iconRt.pivot = new Vector2(0.5f, 0.5f);
            iconRt.sizeDelta = new Vector2(36f, 54f);
            iconRt.anchoredPosition = new Vector2(40f, 0f);
            iconRt.localScale = Vector3.one;
            iconRt.localRotation = Quaternion.identity;

            Image iconImg = iconGo.GetComponent<Image>();
            iconImg.sprite = icon;
            iconImg.color = iconTint;
            iconImg.preserveAspect = true;
            iconImg.raycastTarget = false;
        }

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
