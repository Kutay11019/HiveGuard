using UnityEngine;

public class BearHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 3;

    [Header("Animation State Names")]
    [SerializeField] private string hitStateName = "Hit Front";
    [SerializeField] private string deathStateName = "Death";

    [Header("Death Settings")]
    [SerializeField] private float destroyDelay = 2f;

    [Header("References")]
    [SerializeField] private Animator bearAnimator;
    [SerializeField] private BearEnemyAI bearEnemyAI;
    [SerializeField] private Collider bearCollider;

    private int currentHealth;
    private bool isDead = false;

    public bool IsDead => isDead;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (bearAnimator == null)
        {
            bearAnimator = GetComponentInChildren<Animator>();
        }

        if (bearEnemyAI == null)
        {
            bearEnemyAI = GetComponent<BearEnemyAI>();
        }

        if (bearCollider == null)
        {
            bearCollider = GetComponent<Collider>();
        }
    }

    public void TakeDamage(int damageAmount)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damageAmount;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Bear took damage. Current health: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            PlayHitAnimation();
        }
    }

    private void PlayHitAnimation()
    {
        if (bearAnimator != null && !string.IsNullOrEmpty(hitStateName))
        {
            bearAnimator.CrossFade(hitStateName, 0.08f);
        }
    }

    private void Die()
    {
        isDead = true;

        Debug.Log("Bear defeated.");

        if (bearEnemyAI != null)
        {
            bearEnemyAI.enabled = false;
        }

        if (bearCollider != null)
        {
            bearCollider.enabled = false;
        }

        if (bearAnimator != null && !string.IsNullOrEmpty(deathStateName))
        {
            bearAnimator.CrossFade(deathStateName, 0.1f);
        }

        Destroy(gameObject, destroyDelay);
    }
}