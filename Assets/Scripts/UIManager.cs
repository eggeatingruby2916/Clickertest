using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public ItemManager itemManager;
    
    public GameObject Game_Start_Panel;
    
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
            clickUpgradeButtonTexts[i].text = $"Level : {itemManager.level[i]} \nAdd Click : {NumberFormatter.Format(itemManager.addClick[i])} \nCost : {NumberFormatter.Format(itemManager.cost[i])}";
        }

        for (int i = 0; i < autoClickUpgradeButtonTexts.Count; i++)
        {
            autoClickUpgradeButtonTexts[i].text = $"Level : {itemManager.level[i]} \nAdd Auto Click : {NumberFormatter.Format(itemManager.addAutoClick[i])} \nCost : {NumberFormatter.Format(itemManager.cost[i])}";
        }
    }

    public void MainButtonClick()
    {
        GameManager.Instance.AddGold(GameManager.Instance.GetClick());
    }

    public void ClickUpgradeButtonClick(int index)
    {
        if (GameManager.Instance.GetGold() < itemManager.cost[index])
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        GameManager.Instance.AddClick(itemManager.addClick[index]);
        GameManager.Instance.MinusGold(itemManager.cost[index]);
        itemManager.NextClickUpgrade(index);
    }

    public void AutoClickUpgradeButtonClick(int index)
    {
        if (GameManager.Instance.GetGold() < itemManager.cost[index])
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        GameManager.Instance.AddAutoClick(itemManager.addAutoClick[index]);
        GameManager.Instance.MinusGold(itemManager.cost[index]);
        itemManager.NextAutoClickUpgrade(index);
    }
    
}
