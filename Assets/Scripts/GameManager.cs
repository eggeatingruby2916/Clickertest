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
    public long AddGold(long gold) { _gold += gold; return _gold; }
    public long MinusGold(long gold) { _gold -= gold; return _gold; }

    public long GetClick() { return _click; }
    public long AddClick(long click) { _click += click; return _click; }

    public long GetAutoClick() { return _autoClick; }
    public long AddAutoClick(long click) { _autoClick += click; return _autoClick; }

    public DateTime GetLastSaveTime() { return _lastSaveTime; }

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

        if (offlineSeconds <= 60)
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
    }
}
