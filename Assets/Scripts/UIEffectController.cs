using System;
using UnityEngine;

public class UIEffectController : Singleton<UIEffectController>
{
    public delegate void OnEndEffectsHandler();
    public event OnEndEffectsHandler OnEndEffects;

    [SerializeField] private bool tf = false;

    private void Update()
    {
        if (tf == true)
        {
            TerminateEffects();
            tf = false;
        }
    }

    public void TerminateEffects()
    {
        OnEndEffects?.Invoke();
    }

    public void PlayEffect(GameObject target, Type type)
    {
        UIEffect uiEffect = (UIEffect)target.GetComponent(type);
        if (!uiEffect) return;

        uiEffect.PlayEffect();
    }
}
