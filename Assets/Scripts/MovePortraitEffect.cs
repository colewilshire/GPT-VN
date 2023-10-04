using System.Collections;
using UnityEngine;

public class MovePortraitEffect : UIEffect
{
    private RectTransform rectTransform;
    [SerializeField] private Vector3 startPosition;
    [SerializeField] private Vector3 endPosition;
    [SerializeField] private float duration = .5f;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.position;
        endPosition = new Vector3(0, 0, 0);
    }

    private IEnumerator LerpPosition()
    {
        float elapsed = 0.0f;

        while (elapsed < duration && terminateEffectOnNextFrame == false)
        {
            float t = elapsed / duration;

            rectTransform.anchoredPosition3D = Vector3.Lerp(startPosition, endPosition, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.anchoredPosition3D = endPosition;
        isActive = false;
        terminateEffectOnNextFrame = false;
    }

    public override void PlayEffect()
    {
        base.PlayEffect();
        StartCoroutine(LerpPosition());
    }
}
