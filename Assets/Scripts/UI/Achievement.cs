using UnityEngine;
[System.Serializable]
public class Achievement
{
    public string _name;
    public string _statCode;
    public string _imageName;
    public string _description;
    public int _targetAmmount;
    public bool _unlocked;



}
[System.Serializable]
public class Stat
{
    public string _code;
    public int _value;
}
[System.Serializable]
public class Data
{
    public Stat[] _statistics;
    public Achievement[] _achievement;
}