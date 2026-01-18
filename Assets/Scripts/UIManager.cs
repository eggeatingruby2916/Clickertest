using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public ItemManager itemManager;
    
    public GameObject gameStartPanel;
    
    public List<TMP_Text> clickUpgradeButtonTexts = new();
    public List<TMP_Text> autoClickUpgradeButtonTexts = new();
    
    public TMP_Text currentGoldText;
    public TMP_Text currentClickText;
    public TMP_Text autoClickText;
    
    public TMP_Text comebackText;
    public TMP_Text addGoldText;

    public void Start()
    {
        GameManager.Instance.CalculateOfflineReward(out int hours, out int minutes, out int seconds, out long reward);

        if (reward > 0)
        {
            gameStartPanel.SetActive(true);
            comebackText.text = $"{hours}시간 {minutes}분 {seconds}초 만에 다시 오셨네요.";
            addGoldText.text = $"Add Gold : {NumberFormatter.Format(reward)}";
        }
    }

    void Update()
    {
        UpdateStatusTexts();
        UpdateUpgradeButtonTexts();
    }

    public void CancelButtonClick()
    {
        gameStartPanel.SetActive(false);
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
            var item = itemManager.clickUpgrades[i];
            clickUpgradeButtonTexts[i].text = $"Level : {item.level} \nAdd Click : {NumberFormatter.Format(item.addClick)} \nCost : {NumberFormatter.Format(item.cost)}";
        }

        for (int i = 0; i < autoClickUpgradeButtonTexts.Count; i++)
        {
            var item = itemManager.autoClickUpgrades[i];
            autoClickUpgradeButtonTexts[i].text = $"Level : {item.level} \nAdd Auto Click : {NumberFormatter.Format(item.addAutoClick)} \nCost : {NumberFormatter.Format(item.cost)}";
        }
    }

    public void MainButtonClick()
    {
        GameManager.Instance.AddGold(GameManager.Instance.GetClick());
    }

    public void ClickUpgradeButtonClick(int index)
    {
        var item = itemManager.clickUpgrades[index];
        if (GameManager.Instance.GetGold() < item.cost)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        GameManager.Instance.AddClick(item.addClick);
        GameManager.Instance.MinusGold(item.cost);
        itemManager.NextClickUpgrade(index);
    }

    public void AutoClickUpgradeButtonClick(int index)
    {
        var item = itemManager.autoClickUpgrades[index];
        if (GameManager.Instance.GetGold() < item.cost)
        {
            Debug.Log("돈이 부족합니다.");
            return;
        }

        GameManager.Instance.AddAutoClick(item.addAutoClick);
        GameManager.Instance.MinusGold(item.cost);
        itemManager.NextAutoClickUpgrade(index);
    }
    
}
