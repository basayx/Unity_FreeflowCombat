using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class UpgradeableData : ScriptableObject
{
    [SerializeField] private string id;
    public int Level
    {
        get => PlayerPrefs.GetInt(id + "_Level", 0);
        set => PlayerPrefs.SetInt(id + "_Level", value);
    }
    public bool IsReachedMaxLevel => pricesByLevels.Count < Level;

    public List<PriceInLevel> pricesByLevels = new List<PriceInLevel>();
    public List<PriceStruct> GetPricesInCurrentLevel()
    {
        var level = Level;
        if (pricesByLevels.Count <= level) return null; 
        return pricesByLevels[level].prices;
    }
    public int GetPayedPriceAmount(int level, CurrencyData currencyData)
    {
        var prices = GetPricesInCurrentLevel();
        if (prices == null) return 0; 
        var priceIndex = prices.FindIndex(x => x.currencyData == currencyData);
        if (priceIndex < 0) return 0;
        return PlayerPrefs.GetInt(id + "_Level_Price_" + priceIndex, 0);
    }
    public int GetPayedPriceAmount(CurrencyData currencyData) => GetPayedPriceAmount(Level, currencyData);
    public void SetPayedPriceAmount(int level, CurrencyData currencyData, int value = 1)
    {
        var prices = GetPricesInCurrentLevel();
        if (prices == null) return; 
        var priceIndex = prices.FindIndex(x => x.currencyData == currencyData);
        if (priceIndex < 0) return;
        PlayerPrefs.SetInt(id + "_Level_Price_" + priceIndex, value);
    }
    public void SetPayedPriceAmount(CurrencyData currencyData, int value = 1) => SetPayedPriceAmount(Level, currencyData, value);
    public void IncreasePayedPriceAmount(int level, CurrencyData currencyData, int value = 1)
    {
        var prices = GetPricesInCurrentLevel();
        if (prices == null) return; 
        var priceIndex = prices.FindIndex(x => x.currencyData == currencyData);
        if (priceIndex < 0) return;
        PlayerPrefs.SetInt(id + "_Level_Price_" + priceIndex, PlayerPrefs.GetInt(id + "_Level_Price_" + priceIndex) + value);
    }
    public void IncreasePayedPriceAmount(CurrencyData currencyData, int value = 1) => IncreasePayedPriceAmount(Level, currencyData, value);

    public bool IsStillNeedsToSpendForCurrentLevel()
    {
        var prices = GetPricesInCurrentLevel();
        if (prices == null) return false; 
        foreach (var priceStruct in prices)
        {
            if (GetPayedPriceAmount(priceStruct.currencyData) < priceStruct.priceAmount)
                return true;
        }
        return false;
    }
}

[System.Serializable]
public struct PriceInLevel
{
    public List<PriceStruct> prices;
}
[System.Serializable]
public struct PriceStruct
{
    public CurrencyData currencyData;
    public int priceAmount;
}