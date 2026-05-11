using UnityEngine;

public class WaspEnemy : MonoBehaviour
{
    private enum TargetType
    {
        None,
        Bee,
        Hive
    }

    [Header("Targets")]
    [SerializeField] private Transform playerBeeTarget;
    [SerializeField] private Transform hiveTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float attackDistance = 1.2f;
    [SerializeField] private float rotationSpeed = 10f;

    [Header("Combat")]
    [SerializeField] private int damageToBee = 1;
    [SerializeField] private int damageToHive = 5;
    [SerializeField] private float attackCooldown = 1f;

    [Header("Animation")]
    [SerializeField] private Animator animator;
    [SerializeField] private string attackTriggerName = "Attack";
    [SerializeField] private string isMovingBoolName = "IsMoving";

    private BeeHealth beeHealth;
    private HiveHealth hiveHealth;

    private float attackTimer;
    private bool isDead;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

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
        FindTargets();
        FindBeeHealth();
        FindHiveHealth();
    }

    private void Update()
    {
        if (isDead)
        {
            return;
        }

        if (playerBeeTarget == null || hiveTarget == null)
        {
            FindTargets();
        }

        TargetType targetType = ChooseNearestTarget(out Transform selectedTarget);

        if (targetType == TargetType.None || selectedTarget == null)
        {
            SetMoving(false);
            return;
        }

        float distanceToTarget = GetHorizontalDistance(transform.position, selectedTarget.position);

        if (distanceToTarget > attackDistance)
        {
            MoveToTarget(selectedTarget);
        }
        else
        {
            AttackTarget(targetType);
        }
    }

    private void FindTargets()
    {
        if (playerBeeTarget == null)
        {
            GameObject playerBeeObject = GameObject.FindGameObjectWithTag("Player");

            if (playerBeeObject != null)
            {
                playerBeeTarget = playerBeeObject.transform;
            }
        }

        if (hiveTarget == null)
        {
            GameObject hiveObject = GameObject.FindGameObjectWithTag("Hive");

            if (hiveObject != null)
            {
                hiveTarget = hiveObject.transform;
            }
        }

        if (playerBeeTarget == null)
        {
            Debug.LogWarning("WaspEnemy could not find PlayerBee. Assign PlayerBee manually or set PlayerBee tag to Player.");
        }

        if (hiveTarget == null)
        {
            Debug.LogWarning("WaspEnemy could not find Hive. Assign Hive manually or set Hive tag to Hive.");
        }
    }

    private TargetType ChooseNearestTarget(out Transform selectedTarget)
    {
        selectedTarget = null;

        if (playerBeeTarget == null && hiveTarget == null)
        {
            return TargetType.None;
        }

        if (playerBeeTarget != null && hiveTarget == null)
        {
            selectedTarget = playerBeeTarget;
            return TargetType.Bee;
        }

        if (playerBeeTarget == null && hiveTarget != null)
        {
            selectedTarget = hiveTarget;
            return TargetType.Hive;
        }

        float distanceToBee = GetHorizontalDistance(transform.position, playerBeeTarget.position);
        float distanceToHive = GetHorizontalDistance(transform.position, hiveTarget.position);

        if (distanceToBee < distanceToHive)
        {
            selectedTarget = playerBeeTarget;
            return TargetType.Bee;
        }

        selectedTarget = hiveTarget;
        return TargetType.Hive;
    }

    private float GetHorizontalDistance(Vector3 firstPosition, Vector3 secondPosition)
    {
        firstPosition.y = 0f;
        secondPosition.y = 0f;

        return Vector3.Distance(firstPosition, secondPosition);
    }

    private void MoveToTarget(Transform target)
    {
        SetMoving(true);

        Vector3 targetPosition = target.position;

        // Yaban arısı kendi yüksekliğini korusun.
        // Böylece hedefe giderken yere dalmaz veya havaya zıplamaz.
        targetPosition.y = transform.position.y;

        Vector3 direction = targetPosition - transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        Vector3 moveDirection = direction.normalized;

        transform.position += moveDirection * moveSpeed * Time.deltaTime;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void AttackTarget(TargetType targetType)
    {
        SetMoving(false);

        attackTimer -= Time.deltaTime;

        if (attackTimer > 0f)
        {
            return;
        }

        attackTimer = attackCooldown;

        if (animator != null && !string.IsNullOrEmpty(attackTriggerName))
        {
            animator.SetTrigger(attackTriggerName);
        }

        if (targetType == TargetType.Bee)
        {
            AttackBee();
        }
        else if (targetType == TargetType.Hive)
        {
            AttackHive();
        }
    }

    private void AttackBee()
    {
        if (beeHealth == null)
        {
            FindBeeHealth();
        }

        if (beeHealth != null)
        {
            beeHealth.TakeDamage(damageToBee);
            Debug.Log("Wasp attacked bee. Damage: " + damageToBee);
        }
        else
        {
            Debug.LogWarning("Wasp tried to attack bee, but BeeHealth was not found.");
        }
    }

    private void AttackHive()
    {
        if (hiveHealth == null)
        {
            FindHiveHealth();
        }

        if (hiveHealth != null)
        {
            hiveHealth.TakeDamage(damageToHive);
            Debug.Log("Wasp attacked hive. Damage: " + damageToHive);
        }
        else
        {
            Debug.LogWarning("Wasp tried to attack hive, but HiveHealth was not found.");
        }
    }

    private void FindBeeHealth()
    {
        if (playerBeeTarget == null)
        {
            return;
        }

        beeHealth = playerBeeTarget.GetComponent<BeeHealth>();

        if (beeHealth == null)
        {
            beeHealth = playerBeeTarget.GetComponentInParent<BeeHealth>();
        }

        if (beeHealth == null)
        {
            beeHealth = playerBeeTarget.GetComponentInChildren<BeeHealth>();
        }
    }

    private void FindHiveHealth()
    {
        if (hiveTarget == null)
        {
            return;
        }

        hiveHealth = hiveTarget.GetComponent<HiveHealth>();

        if (hiveHealth == null)
        {
            hiveHealth = hiveTarget.GetComponentInParent<HiveHealth>();
        }

        if (hiveHealth == null)
        {
            hiveHealth = hiveTarget.GetComponentInChildren<HiveHealth>();
        }
    }

    private void SetMoving(bool isMoving)
    {
        if (animator == null || string.IsNullOrEmpty(isMovingBoolName))
        {
            return;
        }

        animator.SetBool(isMovingBoolName, isMoving);
    }

    public void MarkDead()
    {
        isDead = true;
        SetMoving(false);

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);
    }
}