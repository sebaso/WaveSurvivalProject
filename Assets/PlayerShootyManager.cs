using UnityEngine;

public class PlayerShootyManager : MonoBehaviour
{
    public GameObject bullet;
    public Transform bulletSpawn;
    public float bulletSpeed = 10f;
    public float fireRate = 0.5f;
    private float nextFire = 0f;
    private Camera playerCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        TopDownAim();
        if (Input.GetMouseButton(0) && Time.time > nextFire)
        {
            if (bullet != null && bulletSpawn != null)
            {
                nextFire = Time.time + fireRate;
                Shoot();
            }
        }
    }
    void TopDownAim()
    {
        Ray ray = playerCamera.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new(Vector3.up, transform.position);
        if (groundPlane.Raycast(ray, out float rayDistance))
        {
            Vector3 point = ray.GetPoint(rayDistance);

            Vector3 lookDir = point - transform.position;
            lookDir.y = 0;

            if (lookDir != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }
    }
    void Shoot()
    {
        GameObject bulletInstance = Instantiate(bullet, bulletSpawn.position, bulletSpawn.rotation);
        if (bulletInstance.TryGetComponent<Rigidbody>(out var bulletRb))
        {
            bulletRb.AddForce(bulletSpawn.forward * bulletSpeed, ForceMode.Impulse);
        }
        Destroy(bulletInstance, 3.0f);
    }
}
