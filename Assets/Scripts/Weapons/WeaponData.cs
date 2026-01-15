using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Wave System/Weapon")]
public class WeaponData : ScriptableObject
{
    
    public string weaponName;
    public GameObject bulletPrefab;
    public int damage;
    public float fireRate;
    public float baseAccuracy = 95f;
    public int punchThrough;
    public float bulletSpeed;
    public AudioClip shootSound;
    public AudioClip reloadSound;
    public Sprite weaponIcon;
    public int clipSize;
    public int currentAmmoInClip;
    public float reloadTime;
    public int ammoCapacity;
    public int ammo;
    public int weaponHandling;
    public float screenShakeAmount = 1;

}
