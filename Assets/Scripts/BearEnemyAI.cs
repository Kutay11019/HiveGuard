using UnityEngine;

public class BearEnemyAI : MonoBehaviour
{
    private enum BearTargetType
    {
        None,
        Bee,
        Hive
    }

    [Header("Targets")]
    [SerializeField] private Transform beeTarget;
    [SerializeField] private Transform hiveTarget;

    [Header("Target Selection")]
    [SerializeField] private float beeDetectionRange = 4f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private int beeDamage = 20;
    [SerializeField] private int hiveDamage = 40;

    [Header("Animation")]
    [SerializeField] private Animator bearAnimator;

    [Header("Movement Animation States")]
    [SerializeField] private string idleStateName = "Combat Idle";
    [SerializeField] private string moveStateName = "RunForward";

    [Header("Random Attack Animation States")]
    [SerializeField] private string[] attackStateNames =
    {
        "Attack1",
        "Attack2",
        "Attack3",
        "Attack5"
    };

    [SerializeField] private float animationCrossFadeDuration = 0.08f;

    private Transform currentTarget;
    private BearTargetType currentTargetType = BearTargetType.None;

    private float lastAttackTime = -999f;
    private int lastAttackIndex = -1;
    private bool isMovingAnimationPlaying = false;

    private void Awake()
    {
        if (bearAnimator == null)
        {
            bearAnimator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        SelectTarget();

        if (currentTarget == null)
        {
            PlayIdleAnimation();
            return;
        }

        MoveOrAttackCurrentTarget();
    }

    private void SelectTarget()
    {
        if (beeTarget != null)
        {
            float distanceToBee = Vector3.Distance(transform.position, beeTarget.position);

            if (distanceToBee <= beeDetectionRange)
            {
                currentTarget = beeTarget;
                currentTargetType = BearTargetType.Bee;
                return;
            }
        }

        if (hiveTarget != null)
        {
            currentTarget = hiveTarget;
            currentTargetType = BearTargetType.Hive;
            return;
        }

        currentTarget = null;
        currentTargetType = BearTargetType.None;
    }

    private void MoveOrAttackCurrentTarget()
    {
        float distanceToTarget = Vector3.Distance(transform.position, currentTarget.position);

        FaceTarget(currentTarget);

        if (distanceToTarget > attackRange)
        {
            MoveTowardsTarget(currentTarget);
            PlayMoveAnimation();
        }
        else
        {
            PlayIdleAnimation();
            TryAttack();
        }
    }

    private void MoveTowardsTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        direction.Normalize();

        transform.position += direction * moveSpeed * Time.deltaTime;
    }

    private void FaceTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0f;

        if (direction.sqrMagnitude < 0.001f)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(direction);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            rotationSpeed * Time.deltaTime
        );
    }

    private void TryAttack()
    {
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        PlayRandomAttackAnimation();

        if (currentTargetType == BearTargetType.Bee)
        {
            AttackBee();
        }
        else if (currentTargetType == BearTargetType.Hive)
        {
            AttackHive();
        }
    }
private void AttackBee()
{
    if (beeTarget == null)
    {
        Debug.LogWarning("Bear tried to attack bee, but Bee Target is missing.");
        return;
    }

    Debug.Log("Bear attacked bee.");

    BeeHealth beeHealth = beeTarget.GetComponent<BeeHealth>();

    if (beeHealth == null)
    {
        beeHealth = beeTarget.GetComponentInParent<BeeHealth>();
    }

    if (beeHealth == null)
    {
        beeHealth = beeTarget.GetComponentInChildren<BeeHealth>();
    }

    if (beeHealth != null)
    {
        beeHealth.TakeDamage(beeDamage);
    }
    else
    {
        Debug.LogWarning("Bear tried to attack bee, but BeeHealth was not found.");
    }
}
    private void AttackHive()
    {
        if (hiveTarget == null)
        {
            return;
        }

        Debug.Log("Bear attacked hive.");

        hiveTarget.SendMessage(
            "TakeDamage",
            hiveDamage,
            SendMessageOptions.DontRequireReceiver
        );
    }

    private void PlayRandomAttackAnimation()
    {
        if (bearAnimator == null)
        {
            return;
        }

        if (attackStateNames == null || attackStateNames.Length == 0)
        {
            return;
        }

        int randomIndex = Random.Range(0, attackStateNames.Length);

        if (attackStateNames.Length > 1)
        {
            while (randomIndex == lastAttackIndex)
            {
                randomIndex = Random.Range(0, attackStateNames.Length);
            }
        }

        lastAttackIndex = randomIndex;

        string selectedAttackState = attackStateNames[randomIndex];

        bearAnimator.CrossFadeInFixedTime(
            selectedAttackState,
            animationCrossFadeDuration
        );

        isMovingAnimationPlaying = false;

        Debug.Log("Bear played attack animation: " + selectedAttackState);
    }

    private void PlayMoveAnimation()
    {
        if (bearAnimator == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(moveStateName))
        {
            return;
        }

        if (isMovingAnimationPlaying)
        {
            return;
        }

        bearAnimator.CrossFadeInFixedTime(
            moveStateName,
            animationCrossFadeDuration
        );

        isMovingAnimationPlaying = true;
    }

    private void PlayIdleAnimation()
    {
        if (bearAnimator == null)
        {
            return;
        }

        if (string.IsNullOrEmpty(idleStateName))
        {
            return;
        }

        if (!isMovingAnimationPlaying)
        {
            return;
        }

        bearAnimator.CrossFadeInFixedTime(
            idleStateName,
            animationCrossFadeDuration
        );

        isMovingAnimationPlaying = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, beeDetectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}