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
    private long _click;
    private long _autoclick;
    
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public long GetGold() { return _gold; }
    public long AddGold(long gold) { return _gold + gold; }
    public long MinusGold(long gold) { return _gold - gold; }
    
    public long GetClick() { return _click; }
    public long AddClick(long click) { return _click + click; }
    
    public long GetAutoclick() { return _autoclick; }
    public long AddAutoclick(long click) { return _autoclick + click; }

    public void Save()
    {
        PlayerPrefs.SetString("Gold", _gold.ToString());
        PlayerPrefs.SetString("Click", _click.ToString());
        PlayerPrefs.SetString("Autoclick", _autoclick.ToString());
    }

    public void Load()
    {
        _gold = long.Parse(PlayerPrefs.GetString("Gold"));
        _click = long.Parse(PlayerPrefs.GetString("Click"));
        _autoclick = long.Parse(PlayerPrefs.GetString("Autoclick"));
    }
}
