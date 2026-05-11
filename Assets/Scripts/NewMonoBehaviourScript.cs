using UnityEngine;

public class WaspAnimationTest : MonoBehaviour
{
    [SerializeField] private Animator animator;

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponentInChildren<Animator>();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.f))
        {
            animator.SetTrigger("Attack");
        }
    }
}