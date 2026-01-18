using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<int> level = new();
    public List<float> effectRiseMultiplier = new();
    public List<long> basicCost = new();
    public List<long> cost = new();
    public List<double> costMultiplier = new();
    public List<float> costRiseMultiplier = new();
    
    public List<long> basicAddClick = new();
    public List<long> addClick = new();
    public List<double> addClickMultiplier = new();
    
    public List<long> basicAddAutoClick = new();
    public List<long> addAutoClick = new();
    public List<double> addAutoClickMultiplier = new();

    private void Awake()
    {
        Load();
    }

    public void NextClickUpgrade(int index)
    {
        addClickMultiplier[index] *= effectRiseMultiplier[index];
        addClick[index] = (long)Math.Round(addClickMultiplier[index]);
        NextUpgrade(index);
    }

    public void NextAutoClickUpgrade(int index)
    {
        addAutoClickMultiplier[index] *= effectRiseMultiplier[index];
        addAutoClick[index] = (long)Math.Round(addAutoClickMultiplier[index]);
        NextUpgrade(index);
    }

    private void NextUpgrade(int index)
    {
        level[index]++;
        costMultiplier[index] *= costRiseMultiplier[index];
        cost[index] = (long)Math.Round(costMultiplier[index]);
    }

    public void Save()
    {
        for (int i = 0; i < level.Count; i++)
        {
            PlayerPrefs.SetInt("Level" + i, level[i]);
            PlayerPrefs.SetString("Cost" + i, cost[i].ToString());
            PlayerPrefs.SetString("CostMultiplier" + i, costMultiplier[i].ToString(CultureInfo.InvariantCulture));
        }

        for (int i = 0; i < addClick.Count; i++)
        {
            PlayerPrefs.SetString("AddClick" + i, addClick[i].ToString());
            PlayerPrefs.SetString("AddClickMultiplier" + i, addClickMultiplier[i].ToString(CultureInfo.InvariantCulture));
        }

        for (int i = 0; i < addAutoClick.Count; i++)
        {
            PlayerPrefs.SetString("AddAutoClick" + i, addAutoClick[i].ToString());
            PlayerPrefs.SetString("AddAutoClickMultiplier" + i, addAutoClickMultiplier[i].ToString(CultureInfo.InvariantCulture));
        }
    }

    public void Load()
    {
        for (int i = 0; i < level.Count; i++)
        {
            level[i] = PlayerPrefs.GetInt("Level" + i, 1);
            cost[i] = long.Parse(PlayerPrefs.GetString("Cost" + i, basicCost[i].ToString()));
            costMultiplier[i] = double.Parse(PlayerPrefs.GetString("CostMultiplier" + i, basicCost[i].ToString()), CultureInfo.InvariantCulture);
        }

        for (int i = 0; i < addClick.Count; i++)
        {
            addClick[i] = long.Parse(PlayerPrefs.GetString("AddClick" + i, basicAddClick[i].ToString()));
            addClickMultiplier[i] = double.Parse(PlayerPrefs.GetString("AddClickMultiplier" + i, basicAddClick[i].ToString()), CultureInfo.InvariantCulture);
        }

        for (int i = 0; i < addAutoClick.Count; i++)
        {
            addAutoClick[i] = long.Parse(PlayerPrefs.GetString("AddAutoClick" + i, basicAddAutoClick[i].ToString()));
            addAutoClickMultiplier[i] = double.Parse(PlayerPrefs.GetString("AddAutoClickMultiplier" + i, basicAddAutoClick[i].ToString()), CultureInfo.InvariantCulture);
        }
    }
}
