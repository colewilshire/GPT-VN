using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    private Slider progressBar;
    private Coroutine progressBarCoroutine;

    private void Awake()
    {
        progressBar = GetComponent<Slider>();
    }

    private IEnumerator SmoothProgressBar(float targetValue, float duration)
    {
        float startValue = progressBar.value;
        float time = 0;

        while (time < duration)
        {
            time += Time.deltaTime;
            progressBar.value = Mathf.Lerp(startValue, targetValue, time / duration);
            yield return null;
        }

        progressBar.value = targetValue;
    }

    public void SetProgress(float targetValue, float duration = 5f)
    {
        if (gameObject.activeInHierarchy)
        {
            if (progressBarCoroutine != null)
            {
                StopCoroutine(progressBarCoroutine);
            }

            progressBarCoroutine = StartCoroutine(SmoothProgressBar(targetValue, duration));
        }
    }
}
