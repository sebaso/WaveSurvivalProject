using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(WeaponHolder))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlayerShootyManager : MonoBehaviour
{
    public Transform bulletSpawn;
    public Transform weaponMountPoint; // Parent transform for weapon models
    private GameObject currentWeaponModel;

    private WeaponHolder weaponHolder;
    public static PlayerShootyManager instance;
    public CinemachineImpulseSource impulseSource;
    public static ObjectPool<Bullet> bulletPool;
    public GameObject bulletPrefab;
    public ParticleSystem muzzleFlash;
    public enum ItemType { Weapon, Consumable }
    public ItemType itemType;
    private Animator animator;
    public float reloadTimeModifier;

    private float nextFire = 0f;
    private Camera playerCamera;
    private Vector3 lookTarget;
    public bool canRegenerate = true;
    public float handlingStaminaRegenRate = 10f;
    public float handlingStaminaRegenTimer = 0f;
    public float handlingStamina = 100f;
    public float handlingStaminaRegenDelay = 1f;
    private readonly float handlingStaminaDegenRate = 50f;
    public int maxHandlingStamina = 100;
    public int minHandlingStamina = 70;
    public int maxGrenadeCount = 3;
    public int grenadeCount = 0;
    public float grenadeThrowForce = 10f;

    public delegate void GrenadeCountChanged(int current, int max);
    public event GrenadeCountChanged OnGrenadeCountChanged;
    public GameObject grenadePrefab;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        muzzleFlash = GetComponentInChildren<ParticleSystem>();
        playerCamera = Camera.main;
        weaponHolder = GetComponent<WeaponHolder>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet, true, 10, 10);
        animator = GetComponentInChildren<Animator>();

        weaponHolder.OnWeaponChanged += UpdateWeaponModel;
        UpdateWeaponModel();
    }

    void OnDestroy()
    {
        if (weaponHolder != null)
        {
            weaponHolder.OnWeaponChanged -= UpdateWeaponModel;
        }
    }

    void UpdateWeaponModel()
    {
        if (currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }

        if (weaponHolder == null || weaponHolder.CurrentWeapon == null) return;

        if (weaponHolder.CurrentWeapon.weaponMesh != null && weaponMountPoint != null)
        {
            currentWeaponModel = Instantiate(weaponHolder.CurrentWeapon.weaponMesh, weaponMountPoint);
            currentWeaponModel.transform.localPosition = weaponHolder.CurrentWeapon.modelOffsetPosition;
            currentWeaponModel.transform.localEulerAngles = weaponHolder.CurrentWeapon.modelOffsetRotation;

            Transform newSpawn = currentWeaponModel.transform.Find("BulletSpawn");
            if (newSpawn != null)
            {
                bulletSpawn = newSpawn;
                muzzleFlash.transform.position = newSpawn.transform.position;
            }
        }
    }
    void ThrowGrenade()
    {
        if (grenadeCount > 0)
        {
            grenadeCount--;
            OnGrenadeCountChanged?.Invoke(grenadeCount, maxGrenadeCount);
            GameObject grenade = Instantiate(grenadePrefab, bulletSpawn.position, bulletSpawn.rotation);
            grenade.GetComponent<Rigidbody>().AddForce(bulletSpawn.forward * grenadeThrowForce);
            grenade.GetComponent<Rigidbody>().AddTorque(new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)) * grenadeThrowForce);
        }
    }

    Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation).GetComponent<Bullet>();
        bullet.SetPool(bulletPool);
        return bullet;
    }
    void OnGetBullet(Bullet bullet)
    {
        if (bullet == null)
        {
            bullet = CreateBullet();
        }
        bullet.gameObject.SetActive(true);
        bullet.transform.SetPositionAndRotation(bulletSpawn.position, bulletSpawn.rotation);
        bullet.SetPool(bulletPool);
    }
    void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
        bullet.SetPool(null);
    }
    void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    void Update()
    {
        switch (itemType)
        {
            case ItemType.Weapon:
                Aim();
                if (Input.GetKeyDown(KeyCode.R))
                {
                    Reload();
                }
                if (weaponHolder == null || weaponHolder.CurrentWeapon == null || weaponHolder.availableWeapons.Count == 0) return;

                if (Input.GetMouseButton(0) && Time.time > nextFire && weaponHolder.CurrentWeapon.currentAmmoInClip > 0)
                {
                    nextFire = Time.time + weaponHolder.CurrentWeapon.fireRate;
                    Shoot();
                    weaponHolder.UpdateAmmo();
                }
                break;
            case ItemType.Consumable:
                if (Input.GetKeyDown(KeyCode.E))
                {
                    UseConsumable();
                    print("Used consumable");
                }
                break;
        }
        HandlingStaminaRegen();
    }

    public void AddGrenades(int amount)
    {
        print("Adding grenades");
        grenadeCount = Mathf.Min(grenadeCount + amount, maxGrenadeCount);
        OnGrenadeCountChanged?.Invoke(grenadeCount, maxGrenadeCount);
    }

    public bool UseGrenade()
    {
        if (grenadeCount > 0)
        {
            grenadeCount--;
            OnGrenadeCountChanged?.Invoke(grenadeCount, maxGrenadeCount);
            return true;
        }
        return false;
    }

    void HandlingStaminaRegen()
    {
        if (!canRegenerate)
        {
            handlingStaminaRegenTimer += Time.deltaTime;
            if (handlingStaminaRegenTimer >= handlingStaminaRegenDelay)
            {
                canRegenerate = true;
                handlingStaminaRegenTimer = 0;
            }
        }
        if (handlingStamina < 100 && canRegenerate)
        {

            handlingStamina += handlingStaminaRegenRate * Time.deltaTime;
        }

    }
    public void Reload()
    {
        weaponHolder.Reload();
        animator.SetTrigger("Reload");
    }


    void Aim()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Floor")))
        {
            lookTarget = hit.point;
        }

        Vector3 playerLookDir = lookTarget - transform.position;
        playerLookDir.y = 0;
        if (playerLookDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(playerLookDir);
        }

        if (bulletSpawn != null)
        {
            Vector3 aimDir = lookTarget - bulletSpawn.position;
            if (aimDir.sqrMagnitude > 0.001f)
            {
                bulletSpawn.rotation = Quaternion.LookRotation(aimDir);
            }
        }
    }
    void UseConsumable()
    {
        PlayerController.instance.Heal(1);
        weaponHolder.RemoveWeapon(weaponHolder.CurrentWeapon);
        WeaponHUD.instance.RedrawHUD();
    }

    void Shoot()
    {
        if (muzzleFlash != null)
        {
            muzzleFlash.Play();
        }
        weaponHolder.CancelReload();
        impulseSource.GenerateImpulseWithVelocity(Vector3.up * weaponHolder.CurrentWeapon.screenShakeAmount);
        canRegenerate = false;
        handlingStaminaRegenTimer = 0;
        var currentWeapon = weaponHolder.CurrentWeapon;
        if (currentWeapon == null || bulletSpawn == null) return;

        Bullet bulletScript = bulletPool.Get();
        if (bulletScript == null)
        {
            animator.SetTrigger("Shoot");
            bulletScript = CreateBullet();
        }

        bulletScript.transform.position = bulletSpawn.position;

        currentWeapon.currentAmmoInClip -= 1;


        handlingStamina = Mathf.Lerp(handlingStamina, handlingStamina - currentWeapon.weaponHandling, handlingStaminaDegenRate * Time.deltaTime);
        handlingStamina = Mathf.Clamp(handlingStamina, minHandlingStamina, maxHandlingStamina);


        float staminaFactor = (handlingStamina - minHandlingStamina) / (maxHandlingStamina - minHandlingStamina);
        float staminaMultiplier = Mathf.Lerp(30f, 1f, staminaFactor);
        float maxSpreadAngle = (100f - currentWeapon.baseAccuracy) * staminaMultiplier;

        float currentSpread = Random.Range(-maxSpreadAngle * 0.5f, maxSpreadAngle * 0.5f);

        Vector3 flatForward = bulletSpawn.forward;
        flatForward.y = 0;
        flatForward.Normalize();

        Vector3 spreadDirection = Quaternion.Euler(0, currentSpread, 0) * flatForward;

        bulletScript.Initialize(spreadDirection, currentWeapon.bulletSpeed, currentWeapon.damage, currentWeapon.punchThrough);
        if (currentWeapon.isShotgun)
        {
            for (int i = 0; i < currentWeapon.shotgunPellets - 1; i++)
            {
                float pelletSpread = Random.Range(-maxSpreadAngle * 0.5f, maxSpreadAngle * 0.5f);
                Bullet bulletScript1 = bulletPool.Get();
                bulletScript1.transform.position = bulletSpawn.position;
                Vector3 newSpreadDirection = Quaternion.Euler(0, pelletSpread, 0) * flatForward;
                bulletScript1.Initialize(newSpreadDirection, currentWeapon.bulletSpeed, currentWeapon.damage, currentWeapon.punchThrough);
            }
        }
    }


}
