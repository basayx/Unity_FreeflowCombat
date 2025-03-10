using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthController : MonoBehaviour
{
    public bool IsAlive => _currentHealth > 0;

    private Damageable _damageable;
    
    [SerializeField] private float maxHealth = 100;
    public float MaxHealth
    {
        get => maxHealth;
        set
        {
            bool isValueChanged = Mathf.Abs(maxHealth - value) > 0.01f;
            
            float percent = _currentHealth / maxHealth;
            maxHealth = value;
            _currentHealth = maxHealth * percent;
            
            if (isValueChanged && healthUIManager && IsAlive)
                healthUIManager.Refresh(maxHealth, _currentHealth);
        }
    }

    [SerializeField] private Vector2 healthRandomizeRange = new Vector2(1, 1);

    private float _currentHealth;
    public float CurrentHealth => _currentHealth;

    [SerializeField] private HealthUIManager healthUIManager;
    
    public void Initialize()
    {
        _damageable = GetComponent<Damageable>();

        maxHealth *= healthRandomizeRange.GetRandom();
        _currentHealth = maxHealth;
        
        if (healthUIManager && IsAlive)
            healthUIManager.Refresh(maxHealth, _currentHealth);
    }
    
    public void DecreaseHealth(float decreaseAmount)
    {
        if (!IsAlive)
            return;
        
        _currentHealth -= decreaseAmount;
        
        if (healthUIManager && IsAlive)
            healthUIManager.Refresh(maxHealth, _currentHealth);
        
        if (_currentHealth <= 0f)
        {
            Die();
            return;
        }
    }

    public void Die(bool isByCriticalDamage = false)
    {
        if (healthUIManager)
        {
            healthUIManager.gameObject.SetActive(false);
        }
        _damageable.Died();
    }

    public void IncreaseHealth(float increaseAmount)
    {
        if (_currentHealth < 0)
            _currentHealth = 0;
        
        _currentHealth += increaseAmount;
        if (_currentHealth > maxHealth)
            _currentHealth = maxHealth;

        if (healthUIManager)
        {
            healthUIManager.gameObject.SetActive(true);
            healthUIManager.Refresh(maxHealth, _currentHealth);
        }
    }

    public void CompletelyHideTheUI()
    {
        healthUIManager.gameObject.SetActive(false);
    }

    public void SetHealthPercent(float percent)
    {
        _currentHealth = MaxHealth * percent;
    }
}
