using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponHUDItem : MonoBehaviour
{
    public Image iconImage;
    public TextMeshProUGUI keyText;
    public Image backgroundImage;
    public Color selectedColor = Color.green;
    public Color normalColor = Color.white;

    public void Setup(WeaponData weapon, int index, bool isSelected)
    {
        if (weapon.weaponIcon != null)
        {
            iconImage.sprite = weapon.weaponIcon;
            iconImage.enabled = true;
        }
        else
        {
            iconImage.enabled = false;
        }
        keyText.text = (index + 1).ToString() + ")" + weapon.weaponName;

        if (isSelected)
        {
            backgroundImage.color = selectedColor;
        }
        else
        {
            backgroundImage.color = normalColor;
        }
    }
}
