using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class RagdollDissolve : MonoBehaviour
{
    public Renderer[] _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    public float dissolveTime = 5f;
    [SerializeField] private float _dissolveMaxHeight = 2f;
    [SerializeField] private float _dissolveMinHeight = -2f;
    private float timer;
    private float _currentDissolveHeight;
    public bool _isDissolving;
    public float timeToWaitBeforeDissolve = 5f;
    public Vector3 forceDirection = Vector3.back;
    public float forceMagnitude = 1f;
    private readonly Action _dissolveCallback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Awake()
    {
        _renderer = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        Vector3 dir = forceDirection;
        if (PlayerController.instance != null)
        {
            dir = (transform.position - PlayerController.instance.transform.position).normalized;
            dir.y = Mathf.Max(0.2f, dir.y);
            dir.Normalize();
        }
        if (PlayerController.instance == null)
        {
            dir = Vector3.back;
            print("PlayerController not found. TF is happening bruv?");
        }

        ApplyForce(dir * UnityEngine.Random.Range(forceMagnitude / 2, forceMagnitude * 2));
        if (UnityEngine.Random.Range(1, 300) == 1)
        {
            ApplyForce(dir * 50);
        }
        RagdollManager.instance.AddRagdoll(gameObject);
        for (int i = 0; i < _renderer.Length; i++)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            _renderer[i].GetPropertyBlock(_materialPropertyBlock);
            _renderer[i].SetPropertyBlock(_materialPropertyBlock);
            _currentDissolveHeight = _dissolveMaxHeight;
        }
        StartCoroutine(WaitBeforeDissolve());
        Destroy(gameObject, 6f);

    }
    void OnDestroy()
    {
        RagdollManager.instance.RemoveRagdoll(gameObject);
    }
    void OnEnable()
    {
        _renderer = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in _renderer)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_materialPropertyBlock);
            renderer.SetPropertyBlock(_materialPropertyBlock);
            _currentDissolveHeight = _dissolveMaxHeight;
        }
        ResetDissolve();
    }
    public IEnumerator WaitBeforeDissolve()
    {
        yield return new WaitForSeconds(timeToWaitBeforeDissolve);
        StartDissolve();
    }
    // Update is called once per frame
    void Update()
    {
        if (!_isDissolving) return;

        if (_isDissolving)
        {
            foreach (Renderer renderer in _renderer)
            {
                renderer.GetPropertyBlock(_materialPropertyBlock);
                timer -= Time.deltaTime;
                _currentDissolveHeight = Mathf.Lerp(_dissolveMinHeight, _dissolveMaxHeight, timer / dissolveTime);
                _materialPropertyBlock.SetFloat("_CutoffHeight", _currentDissolveHeight);
                renderer.SetPropertyBlock(_materialPropertyBlock);
            }
            if (timer <= 0)
            {
                _isDissolving = false;
                _dissolveCallback?.Invoke();
            }
        }
    }
    public void ApplyForce(Vector3 force)
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.AddForce(force, ForceMode.Impulse);
        }
    }
    public void StartDissolve()
    {
        _isDissolving = true;
        timer = dissolveTime;
        foreach (Renderer renderer in _renderer)
        {
            renderer.GetPropertyBlock(_materialPropertyBlock);
            _materialPropertyBlock.SetFloat("_CutoffHeight", _dissolveMaxHeight);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }
        Destroy(gameObject, dissolveTime);
    }
    public void StopDissolve()
    {
        _isDissolving = false;
    }
    public void ResetDissolve()
    {
        _renderer = GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in _renderer)
        {
            _materialPropertyBlock = new MaterialPropertyBlock();
            renderer.GetPropertyBlock(_materialPropertyBlock);
            _currentDissolveHeight = _dissolveMaxHeight;
            _materialPropertyBlock.SetFloat("_CutoffHeight", _dissolveMaxHeight);
            renderer.SetPropertyBlock(_materialPropertyBlock);
        }

    }
}
