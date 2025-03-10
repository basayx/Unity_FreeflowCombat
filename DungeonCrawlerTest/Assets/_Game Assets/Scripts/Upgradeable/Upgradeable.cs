using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Upgradeable : MonoBehaviour
{
    [SerializeField] private UpgradeableData data;
    public UpgradeableData Data => data;

    private UpgradeableSpendArea _spendArea;

    [SerializeField] private List<GameObject> viewsByLevel = new List<GameObject>();

    private void Start()
    {
        _spendArea = GetComponentInChildren<UpgradeableSpendArea>(true);
        if (_spendArea)
        {
            _spendArea.Initialize();
            if (!data.IsReachedMaxLevel)
            {
                CheckUpgrade();
                _spendArea.OnAllSpentCompleted += CheckUpgrade;
                if (data.IsStillNeedsToSpendForCurrentLevel())
                {
                    _spendArea.Refresh();
                    _spendArea.gameObject.SetActive(true);
                }
            }
            else
            {
                _spendArea.gameObject.SetActive(false);
            }
        }
        RefreshView();
    }

    void CheckUpgrade()
    {
        if (!data.IsReachedMaxLevel && !data.IsStillNeedsToSpendForCurrentLevel())
        {
            data.Level++;
            RefreshView(true);
            if (!data.IsReachedMaxLevel && data.IsStillNeedsToSpendForCurrentLevel())
            {
                _spendArea.Refresh();
                _spendArea.gameObject.SetActive(true);
                return;
            }
        }
        _spendArea.gameObject.SetActive(false);
    }

    private void RefreshView(bool wTweenEffect = false)
    {
        viewsByLevel.ForEach(x => x.gameObject.SetActive(false));
        var view = viewsByLevel[Mathf.Clamp(data.Level, 0, viewsByLevel.Count - 1)];
        view.SetActive(true);
        if (wTweenEffect)
        {
            var t = view.transform;
            t.DOKill();
            t.localScale = Vector3.one * 0.1f;
            t.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        }
    }
}
