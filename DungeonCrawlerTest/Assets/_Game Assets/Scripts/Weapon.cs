using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    protected Character _character;
    public Character Character => _character;
    protected Damageable _target;
    
    [SerializeField] private DamageNumber damageNumberPrefab;
    private float _lastDamageNumberSpawnTime;

    public virtual void Initialize(Character character)
    {
        _character = character;
    }
    
    public void SetTarget(Damageable target)
    {
        _target = target;
    }
    
    public virtual void ApplyAttack(float damage)
    {
        if (_target)
        {
            _target.GetHit(damage);
            SpawnDamageableText(_target.transform.position, damage);
        }
    }

    public void SpawnDamageableText(Vector3 pos, float damage)
    {
        if (damageNumberPrefab)
        {
            damageNumberPrefab.Spawn(pos, damage);
            _lastDamageNumberSpawnTime = Time.time;
        }
    }
}
