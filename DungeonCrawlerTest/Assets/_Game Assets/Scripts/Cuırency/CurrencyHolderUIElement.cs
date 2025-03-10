using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyHolderUIElement : MonoBehaviour
{
    private CurrencyData _data;
    public CurrencyData Data => _data;

    private bool _isInGame; 

    [SerializeField] private TextMeshProUGUI valueText;
    private int _value;
    [SerializeField] private Image iconImage;
    [SerializeField] private DamageNumber damageNumber;
    private float _lastDamageNumberSpawnTime;

    private Canvas _canvas;
    
    public void Initialize(CurrencyData data, bool isInGame, bool isSubToChanges)
    {
        if (!_canvas) _canvas = GetComponentInParent<Canvas>(true);
        _data = data;
        _isInGame = isInGame;
        iconImage.sprite = _data.Icon;
        if (isInGame)
        {
            _value = data.InGameValue;
            OnValueChanged(_data, _value);
            if (isSubToChanges)
                _data.OnInGameValueChanged += OnValueChanged;
        }
        else
        {
            _value = data.Value;
            OnValueChanged(_data, _value);
            if (isSubToChanges)
                _data.OnValueChanged += OnValueChanged;
        }
    }

    private void OnValueChanged(CurrencyData currencyData, int value)
    {
        if (!_canvas) _canvas = GetComponentInParent<Canvas>(true);
        
        int diff = value - _value;
        if (diff > 0 && damageNumber && Time.time > _lastDamageNumberSpawnTime + 0.075f)
        {
            _lastDamageNumberSpawnTime = Time.time;
            var dmgNumber = damageNumber.Spawn(valueText.transform.position, diff);
            dmgNumber.transform.SetParent(_canvas.transform);
        }
        
        _value = value;
        valueText.text = _value.LargeIntToString();
        gameObject.SetActive((_data.IsMain && !_isInGame) || _value > 0);
    }

    private void OnDestroy()
    {
        if (_data)
        {
            if (_isInGame)
                _data.OnInGameValueChanged -= OnValueChanged;
            else
                _data.OnValueChanged -= OnValueChanged;
        }
    }
}
