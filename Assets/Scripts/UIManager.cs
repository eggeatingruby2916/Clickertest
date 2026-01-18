using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public GameObject Game_Start_Panel;

    public List<ClickUpgradeData> clickUpgradeDatas = new List<ClickUpgradeData>();
    public List<AutoClickUpgradeData> autoClickUpgradeDatas = new List<AutoClickUpgradeData>();
    
    public List<TMP_Text> clickUpgradeButtonTexts = new List<TMP_Text>();
    public List<TMP_Text> autoClickUpgradeButtonTexts = new List<TMP_Text>();
    
    public TMP_Text currentGoldText;
    public TMP_Text currentClickText;
    public TMP_Text autoClickText;
    
    public TMP_Text Comeback_Text;
    public TMP_Text AddGold_Text;

    public void Start()
    {
        GameManager.Instance.CalculateOfflineReward(out int hours, out int minutes, out int seconds, out long reward);

        if (reward > 0)
        {
            Game_Start_Panel.SetActive(true);
            Comeback_Text.text = $"{hours}시간 {minutes}분 {seconds}초 만에 다시 오셨네요.";
            AddGold_Text.text = $"Add Gold : {NumberFormatter.Format(reward)}";
        }
    }

    void Update()
    {
        UpdateStatusTexts();
        UpdateUpgradeButtonTexts();
    }

    public void CanelButtonClick()
    {
        Game_Start_Panel.SetActive(false);
    }

    private void UpdateStatusTexts()
    {
        currentGoldText.text = $"Current Gold : {NumberFormatter.Format(GameManager.Instance.GetGold())}";
        currentClickText.text = $"Current Click : {NumberFormatter.Format(GameManager.Instance.GetClick())}";
        autoClickText.text = $"Auto Click : {NumberFormatter.Format(GameManager.Instance.GetAutoClick())}";
    }

    private void UpdateUpgradeButtonTexts()
    {
        for (int i = 0; i < clickUpgradeButtonTexts.Count; i++)
        {
            var data = clickUpgradeDatas[i];
            clickUpgradeButtonTexts[i].text = $"Level : {data.level} \n Add Click : {NumberFormatter.Format(data.addClick)} \n Cost : {NumberFormatter.Format(data.cost)}";
        }

        for (int i = 0; i < autoClickUpgradeButtonTexts.Count; i++)
        {
            var data = autoClickUpgradeDatas[i];
            autoClickUpgradeButtonTexts[i].text = $"Level : {data.level} \n Add Auto Click : {NumberFormatter.Format(data.addAutoClick)} \n Cost : {NumberFormatter.Format(data.cost)}";
        }
    }

    public void MainButtonClick()
    {
        GameManager.Instance.AddGold(GameManager.Instance.GetClick());
    }

    public void ClickUpgradeButtonClick(int index)
    {
        if (GameManager.Instance.GetGold() < clickUpgradeDatas[index].cost)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        GameManager.Instance.AddClick(clickUpgradeDatas[index].addClick);
        GameManager.Instance.MinusGold(clickUpgradeDatas[index].cost);
        NextClickUpgrade(index);
    }

    public void AutoClickUpgradeButtonClick(int index)
    {
        if (GameManager.Instance.GetGold() < autoClickUpgradeDatas[index].cost)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        GameManager.Instance.AddAutoClick(autoClickUpgradeDatas[index].addAutoClick);
        GameManager.Instance.MinusGold(autoClickUpgradeDatas[index].cost);
        NextAutoClickUpgrade(index);
    }

    private void NextClickUpgrade(int index)
    {
        var data = clickUpgradeDatas[index];
        data.addClickMultiplier *= data.effectRiseMultiplier;
        data.addClick = (long)Math.Round(data.addClickMultiplier);
        NextUpgrade(data);
    }

    private void NextAutoClickUpgrade(int index)
    {
        var data = autoClickUpgradeDatas[index];
        data.addAutoClickMultiplier *= data.effectRiseMultiplier;
        data.addAutoClick = (long)Math.Round(data.addAutoClickMultiplier);
        NextUpgrade(data);
    }

    private void NextUpgrade(UpgradeData data)
    {
        data.level++;
        data.costMultiplier *= data.costRiseMultiplier;
        data.cost = (long)Math.Round(data.costMultiplier);
    }
}
