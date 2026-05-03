using UnityEngine;
using UnityEngine.EventSystems;

public class MobileJoystick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private RectTransform background;
    [SerializeField] private RectTransform handle;
    [SerializeField] private float handleRange = 70f;

    public Vector2 Direction { get; private set; }

    private Vector2 startPosition;

    private void Awake()
    {
        if (background == null)
        {
            background = GetComponent<RectTransform>();
        }

        startPosition = background.anchoredPosition;

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }

        Direction = Vector2.zero;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        Vector2 clampedPoint = Vector2.ClampMagnitude(localPoint, handleRange);

        if (handle != null)
        {
            handle.anchoredPosition = clampedPoint;
        }

        Direction = clampedPoint / handleRange;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Direction = Vector2.zero;

        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
        }
    }
}