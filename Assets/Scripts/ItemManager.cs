using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

// 공통 부분
[System.Serializable]
public class UpgradeItem
{
    public int level = 1;
    public float effectRiseMultiplier = 1.1f;
    public long basicCost;
    public long cost;
    public double costMultiplier;
    public float costRiseMultiplier = 1.1f;
}

// Click 전용
[System.Serializable]
public class ClickUpgradeItem : UpgradeItem
{
    public long basicAddClick;
    public long addClick;
    public double addClickMultiplier;
}

// AutoClick 전용
[System.Serializable]
public class AutoClickUpgradeItem : UpgradeItem
{
    public long basicAddAutoClick;
    public long addAutoClick;
    public double addAutoClickMultiplier;
}

public class ItemManager : MonoBehaviour
{
    public List<ClickUpgradeItem> clickUpgrades = new();
    public List<AutoClickUpgradeItem> autoClickUpgrades = new();

    public void NextClickUpgrade(int index)
    {
        var item = clickUpgrades[index];
        item.addClickMultiplier = ClampMultiplier(item.addClickMultiplier * item.effectRiseMultiplier);
        item.addClick = ClampToMax(item.addClickMultiplier);
        NextUpgrade(item);
    }

    public void NextAutoClickUpgrade(int index)
    {
        var item = autoClickUpgrades[index];
        item.addAutoClickMultiplier = ClampMultiplier(item.addAutoClickMultiplier * item.effectRiseMultiplier);
        item.addAutoClick = ClampToMax(item.addAutoClickMultiplier);
        NextUpgrade(item);
    }

    private void NextUpgrade(UpgradeItem item)
    {
        item.level++;
        item.costMultiplier = ClampMultiplier(item.costMultiplier * item.costRiseMultiplier);
        item.cost = ClampToMax(item.costMultiplier);
    }

    private double ClampMultiplier(double value)
    {
        return Math.Min(value, GameManager.MAX_VALUE);
    }

    private long ClampToMax(double value)
    {
        if (value >= GameManager.MAX_VALUE)
            return GameManager.MAX_VALUE;
        return (long)Math.Round(value);
    }

    public void Save()
    {
        for (int i = 0; i < clickUpgrades.Count; i++)
        {
            var item = clickUpgrades[i];
            PlayerPrefs.SetInt("Click_Level" + i, item.level);
            PlayerPrefs.SetString("Click_Cost" + i, item.cost.ToString());
            PlayerPrefs.SetString("Click_CostMultiplier" + i, item.costMultiplier.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString("Click_AddClick" + i, item.addClick.ToString());
            PlayerPrefs.SetString("Click_AddClickMultiplier" + i, item.addClickMultiplier.ToString(CultureInfo.InvariantCulture));
        }

        for (int i = 0; i < autoClickUpgrades.Count; i++)
        {
            var item = autoClickUpgrades[i];
            PlayerPrefs.SetInt("AutoClick_Level" + i, item.level);
            PlayerPrefs.SetString("AutoClick_Cost" + i, item.cost.ToString());
            PlayerPrefs.SetString("AutoClick_CostMultiplier" + i, item.costMultiplier.ToString(CultureInfo.InvariantCulture));
            PlayerPrefs.SetString("AutoClick_AddAutoClick" + i, item.addAutoClick.ToString());
            PlayerPrefs.SetString("AutoClick_AddAutoClickMultiplier" + i, item.addAutoClickMultiplier.ToString(CultureInfo.InvariantCulture));
        }
    }

    public void Load()
    {
        for (int i = 0; i < clickUpgrades.Count; i++)
        {
            var item = clickUpgrades[i];
            item.level = PlayerPrefs.GetInt("Click_Level" + i, 1);
            item.cost = long.Parse(PlayerPrefs.GetString("Click_Cost" + i, item.basicCost.ToString()));
            item.costMultiplier = double.Parse(PlayerPrefs.GetString("Click_CostMultiplier" + i, item.basicCost.ToString()), CultureInfo.InvariantCulture);
            item.addClick = long.Parse(PlayerPrefs.GetString("Click_AddClick" + i, item.basicAddClick.ToString()));
            item.addClickMultiplier = double.Parse(PlayerPrefs.GetString("Click_AddClickMultiplier" + i, item.basicAddClick.ToString()), CultureInfo.InvariantCulture);
        }

        for (int i = 0; i < autoClickUpgrades.Count; i++)
        {
            var item = autoClickUpgrades[i];
            item.level = PlayerPrefs.GetInt("AutoClick_Level" + i, 1);
            item.cost = long.Parse(PlayerPrefs.GetString("AutoClick_Cost" + i, item.basicCost.ToString()));
            item.costMultiplier = double.Parse(PlayerPrefs.GetString("AutoClick_CostMultiplier" + i, item.basicCost.ToString()), CultureInfo.InvariantCulture);
            item.addAutoClick = long.Parse(PlayerPrefs.GetString("AutoClick_AddAutoClick" + i, item.basicAddAutoClick.ToString()));
            item.addAutoClickMultiplier = double.Parse(PlayerPrefs.GetString("AutoClick_AddAutoClickMultiplier" + i, item.basicAddAutoClick.ToString()), CultureInfo.InvariantCulture);
        }
    }
}
