using UnityEngine;

public class BearEnemyAI : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform hiveTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float rotationSpeed = 8f;

    [Header("Attack")]
    [SerializeField] private float attackRange = 1.5f;
    [SerializeField] private float attackCooldown = 1f;
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

    private void Start()
    {
        if (hiveTarget == null)
        {
            GameObject hiveObject = GameObject.FindGameObjectWithTag("Hive");

            if (hiveObject != null)
            {
                hiveTarget = hiveObject.transform;
            }
            else
            {
                Debug.LogWarning("BearEnemyAI could not find Hive. Assign Hive Target manually or set Hive object tag to Hive.");
            }
        }
    }

    private void Update()
    {
        if (hiveTarget == null)
        {
            PlayIdleAnimation();
            return;
        }

        MoveOrAttackHive();
    }

    private void MoveOrAttackHive()
    {
        float distanceToHive = Vector3.Distance(transform.position, hiveTarget.position);

        FaceTarget(hiveTarget);

        if (distanceToHive > attackRange)
        {
            MoveTowardsTarget(hiveTarget);
            PlayMoveAnimation();
        }
        else
        {
            PlayIdleAnimation();
            TryAttackHive();
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

    private void TryAttackHive()
    {
        if (Time.time < lastAttackTime + attackCooldown)
        {
            return;
        }

        lastAttackTime = Time.time;

        PlayRandomAttackAnimation();
        AttackHive();
    }

    private void AttackHive()
    {
        if (hiveTarget == null)
        {
            return;
        }

        Debug.Log("Bear attacked hive.");

        HiveHealth hiveHealth = hiveTarget.GetComponent<HiveHealth>();

        if (hiveHealth == null)
        {
            hiveHealth = hiveTarget.GetComponentInParent<HiveHealth>();
        }

        if (hiveHealth == null)
        {
            hiveHealth = hiveTarget.GetComponentInChildren<HiveHealth>();
        }

        if (hiveHealth != null)
        {
            hiveHealth.TakeDamage(hiveDamage);
        }
        else
        {
            Debug.LogWarning("Bear tried to attack hive, but HiveHealth was not found.");
        }
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}