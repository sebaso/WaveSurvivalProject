using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject achievementsMenu;
    public GameObject settingsMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingsMenu && settingsMenu.activeSelf)
            {
                CloseSettings();
            }
            else if (achievementsMenu && achievementsMenu.activeSelf)
            {
                CloseAchievements();
            }
            else if (VendingMachine.instance != null && VendingMachine.instance.vendingMachineUI.activeSelf)
            {
                return;
            }
            else if (DefenseObjective.instance != null && DefenseObjective.instance.generatorWarning.activeSelf)
            {
                DefenseObjective.instance.DeactivateGeneratorWarning();
            }
            else
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        if (pauseMenu.activeSelf) TimeManager.instance.RequestPause(this);
        else TimeManager.instance.RequestResume(this);

        if (PlayerController.instance) PlayerController.instance.enabled = !pauseMenu.activeSelf;
        if (PlayerShootyManager.instance) PlayerShootyManager.instance.enabled = !pauseMenu.activeSelf;
    }
    public void OpenAchievements()
    {
        pauseMenu.SetActive(false);
        achievementsMenu.SetActive(true);
    }
    public void OpenSettings()
    {
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(true);
    }
    public void CloseSettings()
    {
        settingsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void CloseAchievements()
    {
        achievementsMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        TimeManager.instance.RequestResume(this);
        PlayerController.instance.enabled = true;
        PlayerController.instance.rb.isKinematic = false; // se vuelve kinematic por razones que dios no sabe. solo unity.
        PlayerShootyManager.instance.enabled = true;
    }
    public void Quit()
    {
        pauseMenu.SetActive(false);
        TimeManager.instance.RequestResume(this);
        PlayerController.instance.enabled = true; // por si acaso?????
        PlayerShootyManager.instance.enabled = true;
        SceneManager.LoadScene("MainMenu");
    }
}
