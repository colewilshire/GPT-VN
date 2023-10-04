using UnityEngine;

public abstract class UIEffect : MonoBehaviour
{
    protected bool terminateEffectOnNextFrame = false;
    protected bool isActive = false;

    protected virtual void Start()
    {
        UIEffectController.Instance.OnEndEffects += HandleEffectEnded;
    }

    protected virtual void OnDestroy()
    {
        UIEffectController.Instance.OnEndEffects -= HandleEffectEnded;
    }

    protected virtual void HandleEffectEnded()
    {
        if (isActive == false) return;
        terminateEffectOnNextFrame = true;
    }

    public virtual void PlayEffect()
    {
        isActive = true;
    }
}
