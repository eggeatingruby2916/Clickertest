using UnityEngine;

public class UpgradeData : ScriptableObject
{
    [Header("# Upgrade Data")]
    public int level;
    public float riseMultiplier = 1.1f;
    public int cost;
}
