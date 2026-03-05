using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.Linq;

[RequireComponent(typeof(TextMeshProUGUI))]
[RequireComponent(typeof(Image))]
public class AchievementPrefab : MonoBehaviour
{
    public TextMeshProUGUI _name;
    public TextMeshProUGUI _description;
    public Image _icon;
    public Slider _slider;
    public TextMeshProUGUI _unlocked;

    public void SetAchievement(string name, string text, string spriteName, string statCode, int targetAmmount, int value)
    {
        _name.text = name;
        _description.text = text;
        _icon.sprite = Resources.Load<Sprite>("AchievementIcons/" + spriteName);
        _slider.maxValue = targetAmmount;
        _slider.value = value;
        if (value >= targetAmmount)
        {
            value = targetAmmount;
            _unlocked.text = value + "/" + value;
        }
        else
        {
            _unlocked.text = value + "/" + targetAmmount;
        }

    }
}
