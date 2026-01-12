using UnityEngine;

public class PlayerShootyManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletSpawn;
    public float bulletSpeed = 20f;
    public float fireRate = 0.2f;

    private float nextFire = 0f;
    private Camera playerCamera;
    private Vector3 lookTarget;

    void Start()
    {
        playerCamera = Camera.main;
    }

    void Update()
    {
        Aim();

        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            nextFire = Time.time + fireRate;
            Shoot();
        }
    }

    void Aim()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);

        // Raycast to floor layer only
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, LayerMask.GetMask("Floor")))
        {
            lookTarget = hit.point;
        }

        // Rotate player (body) horizontally
        Vector3 playerLookDir = lookTarget - transform.position;
        playerLookDir.y = 0;
        if (playerLookDir.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(playerLookDir);
        }

        // Point the gun directly at the floor target
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
        if (bullet == null || bulletSpawn == null) return;

        GameObject bulletInstance = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);

        // Ensure bullet ignores player collision
        if (bulletInstance.TryGetComponent<Collider>(out var bCol) && TryGetComponent<Collider>(out var pCol))
        {
            Physics.IgnoreCollision(bCol, pCol);
        }

        if (bulletInstance.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = bulletSpawn.forward * bulletSpeed;
        }

        Destroy(bulletInstance, 3f);
    }
}
