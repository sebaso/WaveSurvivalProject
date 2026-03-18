using UnityEngine;
using System.Collections.Generic;

public class Tutoriel : MonoBehaviour
{
    public List<GameObject> popups;
    private int currentPopupIndex = 0;
    public void NextPopup()
    {
        popups[currentPopupIndex].SetActive(false);
        currentPopupIndex++;
        if (currentPopupIndex < popups.Count)
        {
            popups[currentPopupIndex].SetActive(true);
        }
    }
    public void PreviousPopup()
    {
        popups[currentPopupIndex].SetActive(false);
        currentPopupIndex--;
        if (currentPopupIndex >= 0)
        {
            popups[currentPopupIndex].SetActive(true);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
