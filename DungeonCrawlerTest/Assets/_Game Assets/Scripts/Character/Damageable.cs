using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

[RequireComponent(typeof(HealthController))]
public class Damageable : MonoBehaviour
{
    protected HealthController _healthController;
    public bool IsAlive => _healthController.IsAlive;
    public float CurrentHealth => _healthController.CurrentHealth;
    public float MaxHealth
    {
        get => _healthController.MaxHealth;
        set => _healthController.MaxHealth = value;
    }

    public float HealthPercent => _healthController.CurrentHealth / _healthController.MaxHealth;
    public bool IsMaxHealth => _healthController.MaxHealth <= _healthController.CurrentHealth;

    protected EmissionHitEffector _emissionHitEffector;

    [SerializeField] private bool isShowsDamageAmountText = true; 
    public bool IsShowsDamageAmountText => isShowsDamageAmountText;
    
    public UnityAction OnGotHit;
    
    protected List<StatusEffect> _activeStatusEffects = new List<StatusEffect>();
    public virtual void AStatusEffectStarted(StatusEffect statusEffect) => _activeStatusEffects.Add(statusEffect);
    public virtual void AStatusEffectEnded(StatusEffect statusEffect) => _activeStatusEffects.Remove(statusEffect);

    protected bool _isReadyToGetDamage;
    [SerializeField] private Renderer _aRenderer;
    
    private void Awake()
    {
        if (_aRenderer)
        {
            _isReadyToGetDamage = _aRenderer.isVisible;
            _aRenderer.gameObject.AddComponent<ObjectOnBecameVisible>().OnBecameVisibleAction += OnBecameVisible;
        }
        else
        {
            _isReadyToGetDamage = true;
        }
        
        _healthController = GetComponent<HealthController>();
        _emissionHitEffector = GetComponent<EmissionHitEffector>();
        
        ManualAwake();
    }
    protected virtual void ManualAwake() { }

    private void OnBecameVisible()
    {
        _isReadyToGetDamage = true;
    }

    private void Start()
    {
        _healthController.Initialize();
        ManualStart();
    }
    protected virtual void ManualStart() { }
    
    
    public virtual void GetHit(float damage)
    {
        if (!_isReadyToGetDamage || !enabled)
            return;
        
        if (_emissionHitEffector)
            _emissionHitEffector.DoEffect();

        _healthController.DecreaseHealth(damage);
        
        OnGotHit?.Invoke();
    }
    
    public virtual void Died()
    {
        var cols = GetComponents<Collider>().ToList();
        if (cols.Count > 0) cols.ForEach(c => c.enabled = false);
        var rb = GetComponent<Rigidbody>();
        if (rb) rb.isKinematic = true;
    }

    [ContextMenu(nameof(Editor_GetHit))]
    public void Editor_GetHit()
    {
        GetHit(Random.Range(0.2f, 0.5f) * MaxHealth);
    }

    public virtual Vector3 GetFollowPos(Character character)
    {
        Vector3 pos = transform.position;
        // NavmeshExtensions.RandomPoint(pos, 15f, out pos);
        return pos;
    }

    public void KillForcefully()
    {
        if (!IsAlive)
            return;
        GetHit(MaxHealth * 10);
    }
}
