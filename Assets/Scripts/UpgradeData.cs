using UnityEngine;

public class UpgradeData : ScriptableObject
{
    [Header("# Upgrade Data")]
    public int level;
    public float effectRiseMultiplier = 1.1f;
    public long cost;
    public double costMultiplier;
    public float costRiseMultiplier = 1.15f;
}
