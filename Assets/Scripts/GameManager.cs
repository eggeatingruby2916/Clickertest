using System;
using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<GameManager>();
            }
            return instance;
        }
    }

    public ItemManager itemManager;

    public const long MAX_VALUE = 999_999_999_999_999_999L;  // 약 100경

    private long _gold;
    private long _click = 1;
    private long _autoClick;
    private DateTime _lastSaveTime;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            Load();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        StartCoroutine(AddAutoGold());
    }

    public long GetGold() { return _gold; }
    public long AddGold(long gold)
    {
        if (gold <= 0) return _gold;
        if (_gold >= MAX_VALUE - gold)
            _gold = MAX_VALUE;
        else
            _gold += gold;
        return _gold;
    }
    public long MinusGold(long gold) { _gold -= gold; return _gold; }

    public long GetClick() { return _click; }
    public long AddClick(long click)
    {
        if (click <= 0) return _click;
        if (_click >= MAX_VALUE - click)
            _click = MAX_VALUE;
        else
            _click += click;
        return _click;
    }

    public long GetAutoClick() { return _autoClick; }
    public long AddAutoClick(long click)
    {
        if (click <= 0) return _autoClick;
        if (_autoClick >= MAX_VALUE - click)
            _autoClick = MAX_VALUE;
        else
            _autoClick += click;
        return _autoClick;
    }

    public void CalculateOfflineReward(out int hours, out int minutes, out int seconds, out long rewardGold)
    {
        hours = 0;
        minutes = 0;
        seconds = 0;
        rewardGold = 0;

        if (_autoClick <= 0)
            return;

        TimeSpan elapsed = DateTime.Now - _lastSaveTime;
        long offlineSeconds = (long)elapsed.TotalSeconds;

        if (offlineSeconds <= 0)
            return;

        long reward = _autoClick * offlineSeconds;
        AddGold(reward);

        hours = (int)elapsed.TotalHours;
        minutes = elapsed.Minutes;
        seconds = elapsed.Seconds;
        rewardGold = reward;
    }

    private IEnumerator AddAutoGold()
    {
        while (true)
        {
            AddGold(_autoClick);
            Save();
            yield return new WaitForSeconds(1f);
        }
    }

    private void Save()
    {
        PlayerPrefs.SetString("Gold", _gold.ToString());
        PlayerPrefs.SetString("Click", _click.ToString());
        PlayerPrefs.SetString("AutoClick", _autoClick.ToString());
        PlayerPrefs.SetString("LastSaveTime", DateTime.Now.ToBinary().ToString());
        
        if(itemManager != null) itemManager.Save();
    }

    private void Load()
    {
        _gold = long.Parse(PlayerPrefs.GetString("Gold", "0"));
        _click = long.Parse(PlayerPrefs.GetString("Click", "1"));
        _autoClick = long.Parse(PlayerPrefs.GetString("AutoClick", "0"));

        string savedTime = PlayerPrefs.GetString("LastSaveTime", "");
        _lastSaveTime = string.IsNullOrEmpty(savedTime)
            ? DateTime.Now
            : DateTime.FromBinary(long.Parse(savedTime));

        if (itemManager != null) itemManager.Load();
    }
}
