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
    public TextMeshProUGUI _unlocked;
    void Start()
    {
        _name = GetComponent<TextMeshProUGUI>();
        _icon = GetComponent<Image>();

    }
    public void SetAchievement(string name, string text, string spriteName)
    {
        _name.text = name;
        _description.text = text;
        _icon.sprite = Resources.Load<Sprite>("Sprites/AchievementIcons/" + spriteName);
    }
}
