using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeableSpendArea : SpendArea
{
    private Upgradeable _upgradeable;
    public override List<PriceStruct> GetPricesInCurrentLevel() => _upgradeable.Data.GetPricesInCurrentLevel();

    public override void Initialize()
    {
        base.Initialize();
        _upgradeable = GetComponentInParent<Upgradeable>(true);
    }

    public override void Spent(CurrencyData currencyData, int amount)
    {
        if (!gameObject.activeSelf) return;
        base.Spent(currencyData, amount);
            
        if (_upgradeable.Data.IsStillNeedsToSpendForCurrentLevel())
        {
            _upgradeable.Data.IncreasePayedPriceAmount(currencyData, amount);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    public override void Refresh()
    {
        base.Refresh();

        var level = _upgradeable.Data.Level;
        if (_upgradeable.Data.pricesByLevels.Count < level) return; 
        var priceInLevel = _upgradeable.Data.pricesByLevels[level];
        var currencyUIElementNo = 0;
        foreach (var price in priceInLevel.prices)
        {
            if (currencyUIElementNo >= currencyPriceUIElements.Count)
                break;
            var payed = _upgradeable.Data.GetPayedPriceAmount(price.currencyData);
            if (payed >= price.priceAmount)
                continue;
            var uiElement = currencyPriceUIElements[currencyUIElementNo];
            uiElement.Initialize(price.currencyData, price.priceAmount - payed);
            currencyUIElementNo++;
        }
    }
}
