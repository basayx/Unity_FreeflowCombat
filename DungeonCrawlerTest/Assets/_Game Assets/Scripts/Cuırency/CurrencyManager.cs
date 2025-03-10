
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CurrencyManager : MonoBehaviour
{
    private static CurrencyManager _instance;
    public static CurrencyManager Instance
    {
        get
        {
            if (!_instance)
                _instance = FindObjectOfType<CurrencyManager>();
            return _instance;
        }
    }
    private void Awake()
    {
        if (Instance && Instance != this)
        {
            Destroy(Instance);
            _instance = this;
        }
        
        foreach (CurrencyData currencyData in currencyDatas)
        {
            currencyData.OnValueChanged += ACurrencyChanged;
            currencyData.OnInGameValueChanged += AInGameCurrencyChanged;
        }
    }


    private void ACurrencyChanged(CurrencyData currencyData, int value)
    {
        OnACurrencyChanged?.Invoke(currencyData, value);
    }
    private void AInGameCurrencyChanged(CurrencyData currencyData, int value)
    {
        OnAInGameCurrencyChanged?.Invoke(currencyData, value);
    }

    [SerializeField] private List<CurrencyData> currencyDatas = new List<CurrencyData>();
    public List<CurrencyData> CurrencyDatas => currencyDatas;

    [SerializeField] private CurrencyData mainCurrencyData;
    public CurrencyData MainCurrencyData => mainCurrencyData;

    public UnityAction<CurrencyData, int> OnACurrencyChanged;
    public UnityAction<CurrencyData, int> OnAInGameCurrencyChanged;

    private void OnDestroy()
    {
        foreach (CurrencyData currencyData in currencyDatas)
        {
            currencyData.OnValueChanged -= ACurrencyChanged;
            currencyData.OnInGameValueChanged -= AInGameCurrencyChanged;
        }
    }
    

    public void IncreaseCurrency(int increaseValue, CurrencyData currencyData = null, bool isInGame = false)
    {
        // if (isInGame && increaseValue > 0)
        //     GameManager.Instance.audioManager.PlayCoinCollect();
        var data = currencyData ? currencyData : mainCurrencyData;
        if (isInGame)
            data.InGameValue += increaseValue;
        else
            data.Value += increaseValue;
    }
    public void DecreaseCurrency(int decreaseValue, CurrencyData currencyData = null, bool isInGame = false)
    {
        var data = currencyData ? currencyData : mainCurrencyData;
        if (isInGame)
            data.InGameValue -= decreaseValue;
        else
            data.Value -= decreaseValue;
    }
    public void DecreaseCurrency(Dictionary<CurrencyData, int> dict, bool isInGame = false)
    {
        foreach (KeyValuePair<CurrencyData,int> keyValuePair in dict)
        {
            if (isInGame)
                keyValuePair.Key.InGameValue -= keyValuePair.Value;
            else
                keyValuePair.Key.Value -= keyValuePair.Value;
        }
    }

    public bool IsHaveEnoughCurrency(int targetValue, CurrencyData currencyData = null)
    {
        var data = currencyData ? currencyData : mainCurrencyData;
        return data.Value >= targetValue;
    }
    public bool IsHaveEnoughCurrency(Dictionary<CurrencyData, int> dict)
    {
        foreach (KeyValuePair<CurrencyData,int> keyValuePair in dict)
        {
            if (keyValuePair.Key.Value < keyValuePair.Value)
            {
                return false;
            }
        }
        return true;
    }
    
    public void ResetCurrencies()
    {
        foreach (CurrencyData currencyData in currencyDatas)
        {
            currencyData.Value = 0;
            currencyData.InGameValue = 0;
        }
    }
    public void ResetInGameCurrencies()
    {
        foreach (CurrencyData currencyData in currencyDatas)
        {
            currencyData.InGameValue = 0;
        }
    }
}
