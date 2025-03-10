using System;
using System.Collections;
using System.Collections.Generic;
using DamageNumbersPro;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class InGameCurrencyGainManager : MonoBehaviour
{
    public static InGameCurrencyGainManager Instance;
    private void Awake()
    {
        if (Instance && Instance != this)
            Destroy(Instance);
        Instance = this;
    }

    private PlayerControl _playerControl;
    
    private void Start()
    {
        _playerControl = FindObjectOfType<PlayerControl>();
    }

    public void GainInGameCurrency(int gainValue, CurrencyData currencyData = null)
    {
        CurrencyManager.Instance.IncreaseCurrency(gainValue, currencyData, true);
    }

    public void SpawnAndGainCurrency(Vector3 spawnPos, int gainValue, CurrencyData currencyData = null)
    {
        var data = currencyData ? currencyData : CurrencyManager.Instance.MainCurrencyData;
        var collectablePoolObject = ObjectPoolingManager.Instance.GetObjectFromPool(data.CollectablePrefab);
        var coinTransform = collectablePoolObject.GetGameObject().transform;
        coinTransform.position = spawnPos + Vector3.up;
        // coinTransform.rotation = Quaternion.Euler(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
        coinTransform.DOLocalJump(Vector3.forward, 4, 1, 0.5f).SetEase(Ease.InSine)
            .OnStart(() =>
            {
                coinTransform.SetParent(_playerControl.transform);
            })
            .OnComplete(() =>
            {
                CurrencyManager.Instance.IncreaseCurrency(gainValue, currencyData, false);
                
                coinTransform.DOKill();
                ObjectPoolingManager.Instance.ReturnObjectToPool(collectablePoolObject);
            });
    }
}
