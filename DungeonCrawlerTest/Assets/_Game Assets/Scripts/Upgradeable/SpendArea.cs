using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class SpendArea : MonoBehaviour
{
    protected List<CurrencyPriceUIElement> currencyPriceUIElements = new List<CurrencyPriceUIElement>();
    public virtual List<PriceStruct> GetPricesInCurrentLevel() => null;
    
    public virtual void Initialize()
    {
        currencyPriceUIElements = new List<CurrencyPriceUIElement>(GetComponentsInChildren<CurrencyPriceUIElement>(true));
        currencyPriceUIElements.ForEach(x => x.gameObject.SetActive(false));
    }

    public virtual void Spent(CurrencyData currencyData, int amount)
    {
    }

    public UnityAction OnAllSpentCompleted;
    public virtual void AllSpentCompleted()
    {
        OnAllSpentCompleted?.Invoke();
    }
    
    public virtual void Refresh()
    {
        currencyPriceUIElements.ForEach(x => x.gameObject.SetActive(false));
    }
}
