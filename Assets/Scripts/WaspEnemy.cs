using UnityEngine;

public class WaspEnemy : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform hiveTarget;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float attackDistance = 1.5f;

    [Header("Combat")]
    [SerializeField] private int maxHealth = 2;
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

    private void Start()
    {
        currentHealth = maxHealth;

        if (hiveTarget == null)
        {
            GameObject hive = GameObject.FindGameObjectWithTag("Hive");
            if (hive != null)
                hiveTarget = hive.transform;
        }

        if (animator == null)
            animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDead || hiveTarget == null)
            return;

        float distance = Vector3.Distance(transform.position, hiveTarget.position);

        if (distance > attackDistance)
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
            animator.SetBool(isMovingBoolName, true);

        Vector3 direction = (hiveTarget.position - transform.position).normalized;
        transform.position += direction * moveSpeed * Time.deltaTime;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 8f * Time.deltaTime);
        }
    }

    private void AttackHive()
    {
        if (animator != null)
            animator.SetBool(isMovingBoolName, false);

        attackTimer -= Time.deltaTime;

        if (attackTimer <= 0f)
        {
            if (animator != null)
                animator.SetTrigger(attackTriggerName);

            HiveHealth hiveHealth = hiveTarget.GetComponent<HiveHealth>();

            if (hiveHealth != null)
                hiveHealth.TakeDamage(damageToHive);

            attackTimer = attackCooldown;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
            return;

        currentHealth -= damage;

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

        Collider col = GetComponent<Collider>();
        if (col != null)
            col.enabled = false;

        Destroy(gameObject, 2f);
    }
}