using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class StatusEffect : PoolObjectBase
{
    protected Damageable _damageable;

    [SerializeField] private float totalDuration = 1f;
    private float _durationTimeLeft;

    [SerializeField] private float effectRepeatDelayTime = 0.2f;
    private float _nextEffectTime;
    
    private Coroutine _effectCoroutine;

    public void Initialize(Damageable damageable)
    {
        _durationTimeLeft = totalDuration;
        _nextEffectTime = totalDuration - effectRepeatDelayTime;
        if (_damageable != null)
            return;
        _damageable = damageable;
        StartCoroutine(EffectCoroutine());
    }

    IEnumerator EffectCoroutine()
    {
        OnStatusStarted();
        
        while (_durationTimeLeft > 0f && _damageable.IsAlive)
        {
            _durationTimeLeft -= Time.deltaTime;
            if (_durationTimeLeft < _nextEffectTime)
            {
                ApplyTheEffect();
                _nextEffectTime -= effectRepeatDelayTime;
            }
            yield return null;
        }
        
        OnStatusEnded();
        
        _damageable = null;
    }

    protected virtual void OnStatusStarted()
    {
        _damageable.AStatusEffectStarted(this);
    }
    
    protected virtual void ApplyTheEffect()
    {
    }

    protected virtual void OnStatusEnded()
    {
        _damageable.AStatusEffectEnded(this);
        ObjectPoolingManager.Instance.ReturnObjectToPool(this, 1.5f);
    }
}
