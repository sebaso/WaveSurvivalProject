using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Pool;

[RequireComponent(typeof(WeaponHolder))]
[RequireComponent(typeof(CinemachineImpulseSource))]
public class PlayerShootyManager : MonoBehaviour
{
    public Transform bulletSpawn;
    private WeaponHolder weaponHolder;

    public static PlayerShootyManager instance;
    public CinemachineImpulseSource impulseSource;
    public ObjectPool<Bullet> bulletPool;
    public GameObject bulletPrefab;


    private float nextFire = 0f;
    private Camera playerCamera;
    private Vector3 lookTarget;
    public bool canRegenerate = true;
    public float handlingStaminaRegenRate = 10f;
    public float handlingStaminaRegenTimer = 0f;
    public float handlingStamina = 100f;
    public float handlingStaminaRegenDelay = 1f;
    private float handlingStaminaDegenRate = 50f;
    public int maxHandlingStamina = 100;
    public int minHandlingStamina = 70;
    void Start()
    {
        instance = this;
        playerCamera = Camera.main;
        weaponHolder = GetComponent<WeaponHolder>();
        impulseSource = GetComponent<CinemachineImpulseSource>();
        bulletPool = new ObjectPool<Bullet>(CreateBullet, OnGetBullet, OnReleaseBullet, OnDestroyBullet);
    }
    Bullet CreateBullet()
    {
        Bullet bullet = Instantiate(bulletPrefab, bulletSpawn.position, bulletSpawn.rotation).GetComponent<Bullet>();
        bullet.SetPool(bulletPool);
        return bullet;
    }
    void OnGetBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(true);
    }
    void OnReleaseBullet(Bullet bullet)
    {
        bullet.gameObject.SetActive(false);
    }
    void OnDestroyBullet(Bullet bullet)
    {
        Destroy(bullet.gameObject);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Reload();
        }
        Aim();

        if (weaponHolder == null || weaponHolder.CurrentWeapon == null) return;

        if (Input.GetMouseButton(0) && Time.time > nextFire && weaponHolder.CurrentWeapon.currentAmmoInClip > 0)
        {
            nextFire = Time.time + weaponHolder.CurrentWeapon.fireRate;
            Shoot();
            weaponHolder.UpdateAmmo();
        }
        HandlingStaminaRegen();
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

    void Shoot()
    {
        impulseSource.GenerateImpulseWithVelocity(Vector3.up * weaponHolder.CurrentWeapon.screenShakeAmount);
        canRegenerate = false;
        handlingStaminaRegenTimer = 0;
        var currentWeapon = weaponHolder.CurrentWeapon;
        if (currentWeapon == null || bulletSpawn == null) return;

        Bullet bulletScript = bulletPool.Get();
        GameObject bulletInstance = bulletScript.gameObject;

        // Reset position and rotation
        bulletInstance.transform.position = bulletSpawn.position;
        if (bulletInstance.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        bulletScript.damage = currentWeapon.damage;
        bulletScript.punchThrough = currentWeapon.punchThrough;
        currentWeapon.currentAmmoInClip -= 1;

        // Update handling stamina
        handlingStamina = Mathf.Lerp(handlingStamina, handlingStamina - currentWeapon.weaponHandling, handlingStaminaDegenRate * Time.deltaTime);
        handlingStamina = Mathf.Clamp(handlingStamina, minHandlingStamina, maxHandlingStamina);

        if (rb != null)
        {
            // Calculate accuracy spread
            float staminaFactor = (handlingStamina - minHandlingStamina) / (maxHandlingStamina - minHandlingStamina);
            float staminaMultiplier = Mathf.Lerp(30f, 1f, staminaFactor);
            float maxSpreadAngle = (100f - currentWeapon.baseAccuracy) * staminaMultiplier;

            float spreadAngle = Random.Range(0f, maxSpreadAngle);
            float spreadRotation = Random.Range(0f, 360f);

            Vector3 spreadDirection = Quaternion.Euler(
                Mathf.Sin(spreadRotation * Mathf.Deg2Rad) * spreadAngle,
                Mathf.Cos(spreadRotation * Mathf.Deg2Rad) * spreadAngle,
                0f
            ) * bulletSpawn.forward;

            rb.linearVelocity = spreadDirection.normalized * currentWeapon.bulletSpeed;
        }
    }

}
