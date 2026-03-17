using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private static DataManager _instance;
    public static DataManager Instance { get { return _instance; } }

    public Data _data;
    private string _fileName = "data.dat";
    private string _dataPath;
    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        _dataPath = Application.persistentDataPath + "/" + _fileName;
        Load();
    }

    private void Save()
    {
        BinaryFormatter bf = new();
        FileStream file = File.Create(_dataPath);
        bf.Serialize(file, _data);
        file.Close();

    }
    private void Load()
    {
        if (!File.Exists(_dataPath)) return;
        {
            BinaryFormatter bf = new();
            FileStream file = File.Open(_dataPath, FileMode.Open);
            _data = (Data)bf.Deserialize(file);
            file.Close();
        }

    }
    [ContextMenu("Delete Data")]
    private void DeleteData()
    {
        if (File.Exists(_dataPath))
        {
            File.Delete(_dataPath);
        }
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
