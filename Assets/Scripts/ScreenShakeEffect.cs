using UnityEngine;
using System.Collections;

public class ScreenShakeEffect : UIEffect
{
    [SerializeField] private float shakeDuration = 0.5f;
    [SerializeField] private float shakeMagnitude = 5f;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private IEnumerator Shake()
    {
        Vector3 originalPos = rectTransform.anchoredPosition;
        float elapsed = 0.0f;

        while (elapsed < shakeDuration && terminateEffectOnNextFrame == false)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            rectTransform.anchoredPosition = new Vector2(x, y);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition = originalPos;
        isActive = false;
        terminateEffectOnNextFrame = false;
    }

    public override void PlayEffect()
    {
        base.PlayEffect();
        StartCoroutine(Shake());
    }
}
