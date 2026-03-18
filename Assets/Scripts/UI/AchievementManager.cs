using System;
using UnityEngine;

public class AchievementManager : MonoBehaviour
{
    public static Action<string, string> OnAchievementUnlocked;
    private static AchievementManager _instance;
    public static AchievementManager Instance { get { return _instance; } }
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
    }
    public void IncreaseStat(string statCode, int amount)
    {
        Stat stat = DataManager.Instance.GetStateWithCode(statCode);
        if (stat == null) return;
        stat._value += amount;
        Achievement[] unlockedAchievements = DataManager.Instance.CheckAchievements(statCode);
        foreach (Achievement achievement in unlockedAchievements)
        {
            if (!achievement._unlocked && achievement._targetAmmount <= stat._value)
            {
                achievement._unlocked = true;
                OnAchievementUnlocked?.Invoke(achievement._name, achievement._imageName);
            }
        }
        DataManager.Instance.Save();
    }
}
