using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using UnityEngine;
using UnityEngine.Serialization;

public class StatusEffectContinuousDamage : StatusEffect
{
    [SerializeField] private ParticleSystem particle;
    protected float _damage;

    [SerializeField] protected DamageNumber damageNumberPrefab;

    public void Initialize(Damageable damageable, float damage)
    {
        _damage = damage;
        
        base.Initialize(damageable);
    }

    protected override void ApplyTheEffect()
    {
        base.ApplyTheEffect();

        if (damageNumberPrefab)
            damageNumberPrefab.Spawn(transform.position, _damage);
        
        _damageable.GetHit(_damage);
    }

    protected override void OnStatusStarted()
    {
        base.OnStatusStarted();
        
        particle.Play();
    }

    protected override void OnStatusEnded()
    {
        base.OnStatusEnded();
        
        particle.Stop();
    }
}
