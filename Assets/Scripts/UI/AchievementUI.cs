using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AchievementUI : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private TextMeshProUGUI achievementName;
    private Image achievementIcon;
    private Animator animator;
    private float animationDuration = 3f;
    private float lastAchievementTime;
    void Start()
    {
        animator = GetComponent<Animator>();
        achievementIcon = GetComponentInChildren<Image>();
        achievementName = GetComponentInChildren<TextMeshProUGUI>();
        animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnEnable()
    {
        AchievementManager.OnAchievementUnlocked += SetandShow;
    }
    void OnDisable()
    {
        AchievementManager.OnAchievementUnlocked -= SetandShow;
    }
    private void SetandShow(string name, string iconName)
    {
        achievementName.text = name;
        achievementIcon.sprite = Resources.Load<Sprite>("AchievementIcons/" + iconName);
        animator.SetTrigger("Play");
        lastAchievementTime = Time.time;
        if (lastAchievementTime > Time.time)
        {
            lastAchievementTime += animationDuration;
        }
        else
        {
            lastAchievementTime = Time.time + animationDuration;
        }
        StartCoroutine(WaitForNextAchievement(name, iconName));
    }
    private IEnumerator WaitForNextAchievement(string name, string iconName)
    {
        yield return new WaitForSeconds(lastAchievementTime - Time.time);
        animator.SetTrigger("Play");
        achievementName.text = name;
        achievementIcon.sprite = Resources.Load<Sprite>("AchievementIcons/" + iconName);
    }
}
