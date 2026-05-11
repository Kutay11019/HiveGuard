using UnityEngine;

public class WaspEnemy : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform hiveTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 7f;
    [SerializeField] private float attackDistance = 1.4f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private int damageToHive = 5;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private string deathTriggerName = "Die";
    [SerializeField] private string isMovingBoolName = "IsMoving";

    private int currentHealth;
    private float attackTimer;
    private bool isDead;

    private Rigidbody rb;
    private Collider waspCollider;

    private void Awake()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody>();
        waspCollider = GetComponent<Collider>();

        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }

        if (rb != null)
        {
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    private void Start()
    {
        if (hiveTarget == null)
        {
            GameObject hive = GameObject.FindGameObjectWithTag("Hive");

            if (hive != null)
            {
                hiveTarget = hive.transform;
            }
            else
            {
                Debug.LogError("WaspEnemy could not find Hive. Make sure Hive object has the Hive tag.");
            }
        }
    }

    private void Update()
    {
        if (isDead || hiveTarget == null)
        {
            return;
        }

        float distanceToHive = Vector3.Distance(transform.position, hiveTarget.position);

        if (distanceToHive > attackDistance)
        {
            MoveToHive();
        }
        else
        {
            AttackHive();
        }
    }

    private void MoveToHive()
    {
        if (animator != null)
        {
            animator.SetBool(isMovingBoolName, true);
        }

        Vector3 direction = (hiveTarget.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            transform.rotation = Quaternion.Slerp(
                transform.rotation,
                targetRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private void AttackHive()
    {
        if (animator != null)
        {
            animator.SetBool(isMovingBoolName, false);
        }

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (animator != null)
            {
                animator.SetTrigger(attackTriggerName);
            }

            HiveHealth hiveHealth = hiveTarget.GetComponent<HiveHealth>();

            if (hiveHealth != null)
            {
                hiveHealth.TakeDamage(damageToHive);
                Debug.Log("Wasp attacked hive. Damage: " + damageToHive);
            }

            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log("Wasp took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        if (animator != null)
        {
            animator.SetBool(isMovingBoolName, false);
            animator.SetTrigger(deathTriggerName);
        }

        if (waspCollider != null)
        {
            waspCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Debug.Log("Wasp died.");

        Destroy(gameObject, 1.5f);
    }
}