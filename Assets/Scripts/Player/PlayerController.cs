using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable<int>
{
    public float speed = 10.0f;
    public float acceleration = 1.0f;
    public float maxSpeed = 10.0f;
    public float deceleration = 1.0f;
    private Rigidbody rb;
    public static PlayerController instance;
    public int hp = 5;
    public int maxHp = 5;
    public bool IsDead => hp <= 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        instance = this;
    }

    void FixedUpdate()
    {
        Movement();
    }

    public void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;
        speed = PlayerShootyManager.instance.handlingStamina / maxSpeed;
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead) return;
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        HealthBar.instance.UpdateHealthBar();
    }

    public void Die()
    {
        print("Player died");
        PlayerController.instance.enabled = false;
        PlayerShootyManager.instance.enabled = false;
        WeaponHolder.instance.enabled = false;
    }
}
