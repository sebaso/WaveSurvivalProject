using UnityEngine;

public class GameManager : MonoBehaviour
{
    private PlayerController playerController;
    private PlayerShootyManager playerShootyManager;
    private WeaponHolder weaponHolder;
    
    void Start()
    {
        playerController = PlayerController.instance;
        playerShootyManager = PlayerShootyManager.instance;
        weaponHolder = WeaponHolder.instance;
        //playerController.AddObserver(playerShootyManager);
        //playerController.AddObserver(weaponHolder);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
