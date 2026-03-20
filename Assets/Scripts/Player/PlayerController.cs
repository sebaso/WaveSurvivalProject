using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
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
    public Image hurtImage;
    public float hurtFadeSpeed = 5f;

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

    void Update()
    {
        if (hurtImage != null && hurtImage.color.a > 0)
        {
            Color color = hurtImage.color;
            color.a = Mathf.MoveTowards(color.a, 0, hurtFadeSpeed * Time.deltaTime);
            hurtImage.color = color;
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
        Vector3 localDir = transform.InverseTransformDirection(direction);
        animator.SetFloat("Xspeed", localDir.x);
        animator.SetFloat("Yspeed", localDir.z);
    }

    public void TakeDamage(int damage)
    {
        TakeDamage(damage, transform.position + Vector3.up);
    }

    public void TakeDamage(int damage, Vector3 hitPoint)
    {
        if (isInvincible) return;
        if (IsDead) return;
        hp -= damage;
        if (hurtImage != null)
        {
            Color color = hurtImage.color;
            color.a = 1f;
            hurtImage.color = color;
        }
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
