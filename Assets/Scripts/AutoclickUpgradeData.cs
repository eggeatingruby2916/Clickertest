using UnityEngine;

[CreateAssetMenu(fileName = "AutoClickUpgradeData", menuName = "ScriptableObject/AutoClickUpgradeData")]
public class AutoClickUpgradeData : UpgradeData
{
    [Header("# Auto Click Upgrade Data")]
    public long addAutoClick;
    public double addAutoClickMultiplier;
}
