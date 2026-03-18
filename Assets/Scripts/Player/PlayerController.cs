using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerController : MonoBehaviour, IDamageable<int>, IObservable<IDamageableObserver>, IDamageableObserver
{
    public float speed = 10.0f;
    public float acceleration = 1.0f;
    public float maxSpeed = 10.0f;
    public float deceleration = 1.0f;
    [HideInInspector]
    public Rigidbody rb;
    public static PlayerController instance;
    public int hp = 5;
    public int maxHp = 5;
    public bool IsDead => hp <= 0;
    public bool isInvincible = false;

    public UnityEvent OnInitialize;
    public UnityEvent OnDeactivate;
    public UnityEvent OnActivate;

    private Animator animator;

    private void Awake()
    {
        instance = this;
        observers = new();
        animator = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        OnInitialize.Invoke();
    }

    private void OnEnable()
    {
        OnActivate.Invoke();
    }

    private void OnDisable()
    {
        if (IsDead)
        {
            OnDeactivate.Invoke();
        }
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
        speed = maxSpeed * (PlayerShootyManager.instance.handlingStamina / PlayerShootyManager.instance.maxHandlingStamina);
        rb.linearVelocity = new Vector3(direction.x * speed, rb.linearVelocity.y, direction.z * speed);

        if (direction.sqrMagnitude > 0.01f)
        {
            if (Tutoriel.instance != null)
            {
                Tutoriel.CompleteStep("move");
            }
        }
        // Convert the world movement direction into local space depending on where we are looking
        Vector3 localDir = transform.InverseTransformDirection(direction);

        // Pass the local velocities to the animator 
        // localDir.x gives us our left/right strafe relative to aim
        // localDir.z gives us our forward/backward relative to aim
        animator.SetFloat("Xspeed", localDir.x);
        animator.SetFloat("Yspeed", localDir.z); // Using Z here because in 3D, Z is forward
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;
        if (IsDead) return;
        hp -= damage;
        if (hp <= 0)
        {
            Die();
        }
        HealthBar.instance.UpdateHealthBar();
    }
    public void Heal(int amount)
    {
        if (IsDead) return;
        hp += amount;
        if (hp > maxHp)
        {
            hp = maxHp;
        }
        HealthBar.instance.UpdateHealthBar();
    }

    public void Die()
    {
        print("Player died");
        instance.enabled = false;
        PlayerShootyManager.instance.enabled = false;
        WeaponHolder.instance.enabled = false;

        if (GameOverUI.instance != null)
        {
            GameOverUI.instance.Show();
        }
    }

    private List<IDamageableObserver> observers = new();
    public void OnHealthUpdate(int damageAmount)
    {
        foreach (IDamageableObserver observer in observers)
        {
            observer.OnHealthUpdate(damageAmount);
        }
    }

    public void OnDead()
    {
        foreach (IDamageableObserver observer in observers)
        {
            observer.OnDead();
        }
    }

    public void AddObserver(IDamageableObserver observer)
    {
        observers ??= new List<IDamageableObserver>();
        if (!observers.Contains(observer))
        {
            observers.Add(observer);
        }
    }

    public void RemoveObserver(IDamageableObserver observer)
    {
        if (observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }
}
