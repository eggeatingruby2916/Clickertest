using UnityEngine;

[CreateAssetMenu(fileName = "ClickUpgradeData", menuName = "ScriptableObject/ClickUpgradeData")]
public class ClickUpgradeData : UpgradeData
{
    [Header("# Click Upgrade Data")]
    public long addClick;
    public double addClickMultiplier;
}
