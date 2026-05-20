using System.Collections;
using UnityEngine;

public class UpgradeScaler : MonoBehaviour
{
    [Header("Watch")]
    [SerializeField] private UpgradeType watchedUpgradeType = UpgradeType.MaxHealth;

    [Header("Scaling")]
    [SerializeField] private float scalePerLevel = 0.3f;
    [SerializeField] private float tweenDuration = 0.25f;
    [SerializeField] private AnimationCurve tweenCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);

    private Vector3 baseScale;
    private Coroutine activeTween;
    private bool subscribed;

    private void Awake()
    {
        baseScale = transform.localScale;
    }

    private void OnEnable()
    {
        TrySubscribe();
        ApplyScaleForCurrentLevel(instant: true);
    }

    private void OnDisable()
    {
        if (subscribed && UpgradeManager.Instance != null)
        {
            UpgradeManager.Instance.OnUpgradePurchased -= HandleUpgradePurchased;
        }

        subscribed = false;

        if (activeTween != null)
        {
            StopCoroutine(activeTween);
            activeTween = null;
        }
    }

    private void Start()
    {
        if (!subscribed)
        {
            TrySubscribe();
            ApplyScaleForCurrentLevel(instant: true);
        }
    }

    private void TrySubscribe()
    {
        if (subscribed || UpgradeManager.Instance == null)
        {
            return;
        }

        UpgradeManager.Instance.OnUpgradePurchased += HandleUpgradePurchased;
        subscribed = true;
    }

    private void HandleUpgradePurchased()
    {
        ApplyScaleForCurrentLevel(instant: false);
    }

    private void ApplyScaleForCurrentLevel(bool instant)
    {
        if (UpgradeManager.Instance == null)
        {
            return;
        }

        int level = UpgradeManager.Instance.GetLevel(watchedUpgradeType);
        Vector3 target = baseScale * (1f + level * scalePerLevel);

        if (instant || tweenDuration <= 0f)
        {
            if (activeTween != null)
            {
                StopCoroutine(activeTween);
                activeTween = null;
            }

            transform.localScale = target;
            return;
        }

        if (activeTween != null)
        {
            StopCoroutine(activeTween);
        }

        activeTween = StartCoroutine(TweenScale(target));
    }

    private IEnumerator TweenScale(Vector3 target)
    {
        Vector3 from = transform.localScale;
        float elapsed = 0f;

        while (elapsed < tweenDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / tweenDuration);
            float eased = tweenCurve.Evaluate(t);
            transform.localScale = Vector3.LerpUnclamped(from, target, eased);
            yield return null;
        }

        transform.localScale = target;
        activeTween = null;
    }
}
