using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu]
public class CurrencyData : ScriptableObject
{
    [SerializeField] private string id;
    [SerializeField] private bool isDefault;
    public bool IsDefault => isDefault;
    [SerializeField] private bool isMain;
    public bool IsMain => isMain;

    [SerializeField] private int defaultValue;
    public UnityAction<CurrencyData, int> OnValueChanged;
    public int Value
    {
        get => PlayerPrefs.GetInt("Currency" + id, defaultValue);
        set
        {
            PlayerPrefs.SetInt("Currency" + id, value);
            OnValueChanged?.Invoke(this, value);
        }
    }
    public UnityAction<CurrencyData, int> OnInGameValueChanged;
    public int inGameValue;
    public int InGameValue
    {
        get => inGameValue;
        set
        {
            inGameValue = value;
            OnInGameValueChanged?.Invoke(this, value);
        }
    }
    
    [Header("Display")]
    [SerializeField] private Sprite icon;
    public Sprite Icon => icon;

    [Header("Collectable")]
    [SerializeField] private PoolObjectBase collectablePrefab;
    public PoolObjectBase CollectablePrefab => collectablePrefab;
}
