using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrencyPriceUIElement : MonoBehaviour
{
    [SerializeField] private Image currencyIconImage;
    [SerializeField] private TextMeshProUGUI priceText;
    private Color? _defaultColor;
    [SerializeField] private Color errorColor;
    
    public void Initialize(CurrencyData currencyData, int price)
    {
        currencyIconImage.sprite = currencyData.Icon;
        priceText.text = price.LargeIntToString();
        _defaultColor ??= priceText.color;
        CheckStatus(currencyData, price);
        gameObject.SetActive(true);
    }

    public void CheckStatus(CurrencyData currencyData, int price)
    {
        priceText.color = (currencyData.Value >= price && _defaultColor != null) ? (Color)_defaultColor : errorColor;
    }
}
