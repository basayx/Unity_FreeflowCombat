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
    

    [Header("RevenueMultiplier")] 
    [SerializeField] private GameObject revenueMultiplierBonusGroup;
    [SerializeField] private CanvasGroup revenueMultiplierBonusCanvasGroup;
    [SerializeField] private Slider revenueMultiplierBonusDurationSlider;
    [SerializeField] private TextMeshProUGUI revenueMultiplierBonusDurationText;
    [SerializeField] private TextMeshProUGUI revenueMultiplierValueText;
    public float CurrentRevenueMultiplier { get; set; } = 1f;
    private Coroutine _revenueMultiplierBonusCoroutine;

    private void Start()
    {
        _playerControl = FindObjectOfType<PlayerControl>();
        
        revenueMultiplierBonusCanvasGroup.alpha = 0;
        revenueMultiplierBonusGroup.SetActive(false);
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
        coinTransform.rotation = Quaternion.Euler(Random.Range(-360, 360), Random.Range(-360, 360), Random.Range(-360, 360));
        coinTransform.DOLocalJump(Vector3.forward, 4, 1, 0.5f).SetEase(Ease.InSine)
            .OnStart(() =>
            {
                coinTransform.SetParent(_playerControl.transform);
            })
            .OnComplete(() =>
            {
                GainInGameCurrency(gainValue, currencyData);
                
                coinTransform.DOKill();
                ObjectPoolingManager.Instance.ReturnObjectToPool(collectablePoolObject);
            });
    }

    public void GiveRevenueMultiplierBonus(float multiplier, float duration)
    {
        if (_revenueMultiplierBonusCoroutine != null)
            StopCoroutine(_revenueMultiplierBonusCoroutine);

        _revenueMultiplierBonusCoroutine = StartCoroutine(RevenueMultiplierBonusCoroutine());
        IEnumerator RevenueMultiplierBonusCoroutine()
        {
            revenueMultiplierBonusCanvasGroup.DOKill();
            revenueMultiplierBonusGroup.SetActive(true);
            revenueMultiplierBonusCanvasGroup.alpha = 0;
            revenueMultiplierBonusCanvasGroup.DOFade(1f, 0.25f);

            CurrentRevenueMultiplier = multiplier;
            revenueMultiplierValueText.text = "x" + Mathf.RoundToInt(CurrentRevenueMultiplier).LargeIntToString();

            var timeLeft = duration;
            RefreshDurationUI(timeLeft, duration);
            
            while (timeLeft > 0f)
            {
                timeLeft -= Time.deltaTime;
                RefreshDurationUI(timeLeft, duration);
                yield return null;
            }

            yield return null;
            
            CurrentRevenueMultiplier = 1;
            
            revenueMultiplierBonusCanvasGroup.DOKill();
            revenueMultiplierBonusCanvasGroup.DOFade(0f, 0.25f)
                .OnComplete(() =>
                {
                    revenueMultiplierBonusGroup.SetActive(false);
                });
        }
    }

    void RefreshDurationUI(float timeLeft, float maxDuration)
    {
        revenueMultiplierBonusDurationSlider.value = timeLeft / maxDuration;
        revenueMultiplierBonusDurationText.text = Mathf.CeilToInt(timeLeft).LargeIntToString() + "s";
    }
}
