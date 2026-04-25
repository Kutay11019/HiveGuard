using UnityEngine;
using UnityEngine.InputSystem;

public class BeeMovement3D : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float verticalSpeed = 3f;

    private Rigidbody rb;
    private Vector3 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard == null)
        {
            movementInput = Vector3.zero;
            return;
        }

        float moveX = 0f;
        float moveY = 0f;
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

        // Up - Down movement
        if (keyboard.spaceKey.isPressed)
        {
            moveY = 1f;
        }
        else if (keyboard.leftCtrlKey.isPressed || keyboard.rightCtrlKey.isPressed)
        {
            moveY = -1f;
        }

        movementInput = new Vector3(moveX, moveY, moveZ).normalized;
    }

    private void FixedUpdate()
    {
        Vector3 targetVelocity = new Vector3(
            movementInput.x * moveSpeed,
            movementInput.y * verticalSpeed,
            movementInput.z * moveSpeed
        );

        rb.linearVelocity = targetVelocity;
    }
}