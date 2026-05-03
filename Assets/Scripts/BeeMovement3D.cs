using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class BeeMovement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    [Header("Mobile Input")]
    [SerializeField] private MobileJoystick mobileJoystick;

    [Header("Editor Testing")]
    [SerializeField] private bool allowKeyboardFallback = true;

    [Header("Rotation")]
    [SerializeField] private bool rotateTowardsMovement = true;
    [SerializeField] private float rotationSpeed = 10f;

    private Rigidbody rb;
    private Vector3 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // Mobile gameplay is ground-based, so the bee should not fall or fly vertically.
        rb.useGravity = false;

        // Keep the bee on the ground plane and prevent unwanted tipping.
        rb.constraints =
            RigidbodyConstraints.FreezePositionY |
            RigidbodyConstraints.FreezeRotationX |
            RigidbodyConstraints.FreezeRotationZ;
    }

    private void Update()
    {
        Vector2 input = Vector2.zero;

        // Main mobile input: on-screen joystick.
        if (mobileJoystick != null)
        {
            input = mobileJoystick.Direction;
        }

        // Keyboard fallback is only for easier testing in Unity Editor / PC.
        if (allowKeyboardFallback && input.sqrMagnitude < 0.01f)
        {
            input = ReadKeyboardInput();
        }

        // X-Z ground movement. No vertical Y movement.
        movementInput = new Vector3(input.x, 0f, input.y);

        if (movementInput.sqrMagnitude > 1f)
        {
            movementInput.Normalize();
        }
    }

    private Vector2 ReadKeyboardInput()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            return Vector2.zero;
        }

        float moveX = 0f;
        float moveZ = 0f;

        // Left - Right movement
        if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
        {
            moveX = -1f;
        }
        else if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
        {
            moveX = 1f;
        }

        // Forward - Backward movement
        if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
        {
            moveZ = 1f;
        }
        else if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
        {
            moveZ = -1f;
        }

        return new Vector2(moveX, moveZ).normalized;
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(
            movementInput.x * moveSpeed,
            0f,
            movementInput.z * moveSpeed
        );

        rb.linearVelocity = targetVelocity;

        if (rotateTowardsMovement && movementInput.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(movementInput, Vector3.up);

            rb.MoveRotation(
                Quaternion.Slerp(
                    rb.rotation,
                    targetRotation,
                    rotationSpeed * Time.fixedDeltaTime
                )
            );
        }
    }
}