using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyHoldersUIGroup : MonoBehaviour
{
    [SerializeField] private CurrencyHolderUIElement currencyHolderUIElementPrefab;
    [SerializeField] private Transform layoutParent;
    private List<CurrencyHolderUIElement> _uiElements = new List<CurrencyHolderUIElement>();
    [SerializeField] private bool isInGame;
    [SerializeField] private Transform titleTransform;
    [SerializeField] private bool isSubToChanges = true;
    [SerializeField] private bool onlyShowDefaultCurrencies = true;
    private void Start()
    {
        foreach (CurrencyData currencyData in CurrencyManager.Instance.CurrencyDatas)
        {
            if (onlyShowDefaultCurrencies && !currencyData.IsDefault) continue;
            GetUIElement(currencyData);
        }
    }

    CurrencyHolderUIElement GetUIElement(CurrencyData data)
    {
        var uiElement = _uiElements.Find(x => x.Data == data);
        if (!uiElement)
        {
            uiElement = Instantiate(currencyHolderUIElementPrefab, layoutParent);
            uiElement.Initialize(data, isInGame, isSubToChanges);
            _uiElements.Add(uiElement);
            titleTransform.SetAsLastSibling();
        }
        return uiElement;
    }

    public void HideAllButMain()
    {
        foreach (var currencyData in CurrencyManager.Instance.CurrencyDatas)
        {
            if (onlyShowDefaultCurrencies && !currencyData.IsDefault) continue;
            var uiElement = GetUIElement(currencyData);
            uiElement.gameObject.SetActive(currencyData.IsMain);
        }
    }

    public void ShowAll()
    {
        foreach (var currencyData in CurrencyManager.Instance.CurrencyDatas)
        {
            if (onlyShowDefaultCurrencies && !currencyData.IsDefault) continue;
            var uiElement = GetUIElement(currencyData);
            uiElement.gameObject.SetActive(currencyData.IsMain || currencyData.Value > 0);
        }
    }
}
