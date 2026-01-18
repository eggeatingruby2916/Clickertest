using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class UIManager : MonoBehaviour
{
    public List<Button> upgradeButtons = new List<Button>();
    
    public List<ClickUpgradeData> clickUpgradeDatas = new List<ClickUpgradeData>();
    public List<AutoClickUpgradeData> autoClickUpgradeDatas = new List<AutoClickUpgradeData>();
    
    public List<TMP_Text> clickUpgradeButtonTexts = new List<TMP_Text>();
    public List<TMP_Text> autoClickUpgradeButtonTexts = new List<TMP_Text>();
    
    public TMP_Text currentGoldText;
    public TMP_Text currentClickText;
    public TMP_Text autoClickText;

    void Update()
    {
        currentGoldText.text = $"Current Gold : {GameManager.Instance.GetGold()}";
        currentClickText.text = $"Current Click : {GameManager.Instance.GetClick()}";
        autoClickText.text = $"Auto Click : {GameManager.Instance.GetAutoClick()}";

        for (int i = 0; i < clickUpgradeButtonTexts.Count; i++)
        {
            clickUpgradeButtonTexts[i].text = $"Level : {clickUpgradeDatas[i].level} \n Add Click : {clickUpgradeDatas[i].addClick} \n Cost : {clickUpgradeDatas[i].cost}";
        }
        
        for (int i = 0; i < autoClickUpgradeButtonTexts.Count; i++)
        {
            autoClickUpgradeButtonTexts[i].text = $"Level : {autoClickUpgradeDatas[i].level} \n Add Auto Click : {autoClickUpgradeDatas[i].addAutoClick} \n Cost : {autoClickUpgradeDatas[i].cost}";
        }
    }

    public void MainButtonClick()
    {
        GameManager.Instance.AddGold(GameManager.Instance.GetClick());
    }

    public void ClickUpgradeButtonClick(int index)
    {
        if (GameManager.Instance.GetGold() >= clickUpgradeDatas[index].cost)
        {
            GameManager.Instance.AddClick(clickUpgradeDatas[index].addClick);
            GameManager.Instance.MinusGold(clickUpgradeDatas[index].cost);
        }

        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }

    public void UpgradeButtonClick(int index)
    {
        if (GameManager.Instance.GetGold() >= autoClickUpgradeDatas[index].cost)
        {
            GameManager.Instance.AddClick(autoClickUpgradeDatas[index].addAutoClick);
            GameManager.Instance.MinusGold(autoClickUpgradeDatas[index].cost);
        }

        else
        {
            Debug.Log("돈이 부족합니다.");
        }
    }
}
