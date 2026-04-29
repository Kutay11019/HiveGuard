using UnityEngine;

public class BeeAnimatorDriver : MonoBehaviour
{
    [Header("Animator Reference")]
    [SerializeField] private Animator beeAnimator;

    [Header("Visual Model Reference")]
    [SerializeField] private Transform beeVisual;

    [Header("Movement Animation Settings")]
    [SerializeField] private float moveThreshold = 0.05f;

    [Header("Rotation Settings")]
    [SerializeField] private bool rotateVisualToMovementDirection = true;
    [SerializeField] private float rotationSpeed = 12f;
    [SerializeField] private float rotationThreshold = 0.01f;
    [SerializeField] private Vector3 rotationOffset = Vector3.zero;

    [Header("Animator Parameter Names")]
    [SerializeField] private string speedParameter = "Speed";
    [SerializeField] private string attackTriggerParameter = "Attack";

    private Vector3 lastPosition;

    private void Start()
    {
        lastPosition = transform.position;

        if (beeAnimator == null)
            beeAnimator = GetComponentInChildren<Animator>();

        if (beeVisual == null && beeAnimator != null)
            beeVisual = beeAnimator.transform;

        if (beeAnimator == null)
            Debug.LogWarning("BeeAnimatorDriver: Animator reference is missing.");
    }

    private void Update()
    {
        if (beeAnimator == null)
            return;

        Vector3 currentPosition = transform.position;
        Vector3 movementDelta = currentPosition - lastPosition;

        UpdateMovementAnimation(movementDelta);
        UpdateVisualDirection(movementDelta);

        lastPosition = currentPosition;
    }

    private void UpdateMovementAnimation(Vector3 movementDelta)
    {
        float currentSpeed = movementDelta.magnitude / Mathf.Max(Time.deltaTime, 0.0001f);

        if (currentSpeed < moveThreshold)
            currentSpeed = 0f;

        beeAnimator.SetFloat(speedParameter, currentSpeed);
    }

    private void UpdateVisualDirection(Vector3 movementDelta)
    {
        if (!rotateVisualToMovementDirection || beeVisual == null)
            return;

        Vector3 flatMovement = new Vector3(movementDelta.x, 0f, movementDelta.z);

        if (flatMovement.magnitude < rotationThreshold)
            return;

        Quaternion targetRotation = Quaternion.LookRotation(flatMovement.normalized, Vector3.up);
        targetRotation *= Quaternion.Euler(rotationOffset);

        beeVisual.rotation = Quaternion.Slerp(
            beeVisual.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    public void PlayAttackAnimation()
    {
        if (beeAnimator == null)
            return;

        beeAnimator.SetTrigger(attackTriggerParameter);
    }
}