using UnityEngine;
using UnityEngine.UI;

public class AchievementMenu : MonoBehaviour
{
    public GameObject achievementMenu;
    public GameObject achievementPrefab;
    public Transform achievementParent;


    public void ToggleAchievementMenu()
    {
        achievementMenu.SetActive(!achievementMenu.activeSelf);
        PopulateAchievementMenu();
    }
    public void PopulateAchievementMenu()
    {
        foreach (Transform child in achievementParent)
        {
            Destroy(child.gameObject);
        }
        foreach (Achievement achievement in DataManager.Instance._data._achievement)
        {
            GameObject achievementObj = Instantiate(achievementPrefab, achievementParent);
            achievementObj.GetComponent<AchievementPrefab>().SetAchievement(achievement._name, achievement._description, achievement._imageName, achievement._statCode, achievement._targetAmmount, DataManager.Instance.GetStateWithCode(achievement._statCode)._value);
        }

    }

}
