using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using StarterAssets;
using UnityEngine;

public class PlayerSpendManager : MonoBehaviour
{
    [SerializeField] private StarterAssetsInputs starterAssetsInputs;
    [SerializeField] private float spendSpan = 0.1f;
    private float _lastSpendTime;
    
    private void OnTriggerStay(Collider col)
    {
        var spendArea = col.GetComponent<SpendArea>();
        if (spendArea && _lastSpendTime + spendSpan <= Time.time && starterAssetsInputs.move == Vector2.zero)
        {
            var prices = spendArea.GetPricesInCurrentLevel();
            if (prices != null)
            {
                foreach (PriceStruct priceStruct in prices)
                {
                    if (CurrencyManager.Instance.IsHaveEnoughCurrency(1, priceStruct.currencyData))
                    {
                        _lastSpendTime = Time.time;
                        DoSpend(spendArea, transform.position + Vector3.up, priceStruct.currencyData, 1);
                        CurrencyManager.Instance.DecreaseCurrency(1, priceStruct.currencyData);
                        break;
                    }
                }
            }
        }
    }
    
    public virtual void DoSpend(SpendArea spendArea, Vector3 currencySpawnPos, CurrencyData currencyData, int amount)
    {
        spendArea.Spent(currencyData, amount);
        var currency = ObjectPoolingManager.Instance.GetObjectFromPool(currencyData.CollectablePrefab);
        var t = currency.GetGameObject().transform;
        t.DOKill();
        t.position = currencySpawnPos;
        t.DOJump(spendArea.transform.position, 1.5f, 1, 0.5f).
            OnComplete(() =>
            {
                spendArea.Refresh();
                ObjectPoolingManager.Instance.ReturnObjectToPool(currency);
            });
    }
}
