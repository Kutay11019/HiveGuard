using UnityEngine;

public class HealthBarUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private RectTransform fillRect;
    [SerializeField] private GameObject rootObject;

    [Header("Display Settings")]
    [SerializeField] private bool hideWhenFull = false;
    [SerializeField] private bool hideWhenEmpty = true;

    private void Awake()
    {
        if (rootObject == null)
        {
            rootObject = gameObject;
        }
    }

    public void SetHealth(int currentHealth, int maxHealth)
    {
        if (maxHealth <= 0)
        {
            return;
        }

        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        float healthPercent = (float)currentHealth / maxHealth;

        if (fillRect != null)
        {
            fillRect.anchorMin = new Vector2(0f, 0f);
            fillRect.anchorMax = new Vector2(healthPercent, 1f);
            fillRect.offsetMin = Vector2.zero;
            fillRect.offsetMax = Vector2.zero;
        }

        if (rootObject != null)
        {
            if (hideWhenEmpty && currentHealth <= 0)
            {
                rootObject.SetActive(false);
            }
            else if (hideWhenFull && currentHealth >= maxHealth)
            {
                rootObject.SetActive(false);
            }
            else
            {
                rootObject.SetActive(true);
            }
        }
    }

    public void Hide()
    {
        if (rootObject != null)
        {
            rootObject.SetActive(false);
        }
    }

    public void Show()
    {
        if (rootObject != null)
        {
            rootObject.SetActive(true);
        }
    }
}