using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

    private const string PlayerDataKey = "PlayerData";
    public Data _data;
    private string _fileName = "data.dat";
    private string _dataPath;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(this.gameObject);

        _dataPath = Application.persistentDataPath + "/" + _fileName;

        // Cleanup legacy file-based data if it exists
        if (File.Exists(_dataPath))
        {
            Debug.Log("[DataManager] Legacy data file found. Deleting after migration/switch.");
            File.Delete(_dataPath);
        }

        Load();
    }

    public void Save()
    {
        string json = JsonUtility.ToJson(_data);
        PlayerPrefs.SetString(PlayerDataKey, json);
        PlayerPrefs.Save();
    }

    private void Load()
    {
        if (PlayerPrefs.HasKey(PlayerDataKey))
        {
            string json = PlayerPrefs.GetString(PlayerDataKey);
            JsonUtility.FromJsonOverwrite(json, _data);
        }
        else
        {
            Debug.Log("[DataManager] No PlayerPrefs data found. Starting fresh.");
        }
    }

    public void DeleteData()
    {
        PlayerPrefs.DeleteKey(PlayerDataKey);
        Debug.Log("[DataManager] Data deleted from PlayerPrefs.");
    }
    public Stat GetStateWithCode(string code)
    {
        for (int i = 0; i < _data._statistics.Length; i++)
        {
            if (_data._statistics[i]._code == code)
            {
                return _data._statistics[i];
            }
        }
        return null;
    }
    public Achievement[] CheckAchievements(string statCode)
    {
        return _data._achievement.Where(a => a._statCode == statCode).ToArray();
    }
}
