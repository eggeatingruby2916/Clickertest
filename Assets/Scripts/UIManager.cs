using UnityEngine;
using TMPro;

public class UIManager : MonoBehaviour
{
    public TMP_Text currentGoldText;
    public TMP_Text currentClickText;
    public TMP_Text autoClickText;

    void Update()
    {
        currentGoldText.text = $"Current Gold : {GameManager.Instance.GetGold()}";
        currentClickText.text = $"Current Click : {GameManager.Instance.GetClick()}";
        autoClickText.text = $"Auto Click : {GameManager.Instance.GetAutoClick()}";
    }

    public void UpgradeButtonClick()
    {
        
    }
}
