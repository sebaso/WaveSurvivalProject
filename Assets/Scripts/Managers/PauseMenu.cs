using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu;
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
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0 : 1;
            PlayerController.instance.enabled = !pauseMenu.activeSelf;
            PlayerShootyManager.instance.enabled = !pauseMenu.activeSelf;
        }
    }
    public void Resume()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        PlayerController.instance.enabled = true;
        PlayerController.instance.GetComponent<Rigidbody>().isKinematic = false; // se vuelve kinematic por razones que dios no sabe. solo unity.
        PlayerShootyManager.instance.enabled = true;
    }
    public void Quit()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        PlayerController.instance.enabled = true; // por si acaso?????
        PlayerShootyManager.instance.enabled = true;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
