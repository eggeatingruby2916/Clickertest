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
    public long AddGold(long gold) { _gold += gold; return _gold; }
    public long MinusGold(long gold) { _gold -= gold; return _gold; }
    
    public long GetClick() { return _click; }
    public long AddClick(long click) { return _click + click; }
    
    public long GetAutoClick() { return _autoClick; }
    public long AddAutoClick(long click) { return _autoClick + click; }

    public void Save()
    {
        PlayerPrefs.SetString("Gold", _gold.ToString());
        PlayerPrefs.SetString("Click", _click.ToString());
        PlayerPrefs.SetString("Autoclick", _autoClick.ToString());
    }

    public void Load()
    {
        _gold = long.Parse(PlayerPrefs.GetString("Gold", "0"));
        _click = long.Parse(PlayerPrefs.GetString("Click", "1"));
        _autoClick = long.Parse(PlayerPrefs.GetString("AutoClick", "0"));
    }
}
