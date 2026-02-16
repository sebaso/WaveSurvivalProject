using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour, IDamageable<int>, IObservable<IDamageableObserver>, IDamageableObserver
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

    public UnityEvent OnInitialize;
    public UnityEvent OnDeactivate;
    public UnityEvent OnActivate;



    private void Awake()
    {
        observers = new();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        instance = this;
        OnInitialize.Invoke();
    }

    private void OnEnable()
    {
        OnActivate.Invoke();
    }

    private void OnDisable()
    {
        OnDeactivate.Invoke();
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
