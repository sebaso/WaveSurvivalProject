using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[RequireComponent(typeof(MeshRenderer))]
public class DissolveBehaviour : MonoBehaviour
{
    public Renderer _renderer;
    private MaterialPropertyBlock _materialPropertyBlock;
    [SerializeField] public float dissolveTime = 5f;
    [SerializeField] private float _dissolveMaxHeight = 2f;
    [SerializeField] private float _dissolveMinHeight = -2f;
    private float timer;
    private float _currentDissolveHeight;
    public bool _isDissolving;
    private Action _dissolveCallback;

    // Start is called once before the first execution of Update after the MonoBehaviour is created


    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _materialPropertyBlock = new MaterialPropertyBlock();
        _currentDissolveHeight = _dissolveMaxHeight;

    }
    void OnEnable()
    {
        _renderer = GetComponent<MeshRenderer>();
        ResetDissolve();
    }
    // Update is called once per frame
    void Update()
    {
        if (!_isDissolving) return;

        if (_isDissolving)
        {
            _renderer.GetPropertyBlock(_materialPropertyBlock);
            timer -= Time.deltaTime;
            _currentDissolveHeight = Mathf.Lerp(_dissolveMinHeight, _dissolveMaxHeight, timer / dissolveTime);
            _materialPropertyBlock.SetFloat("_CutoffHeight", _currentDissolveHeight);
            _renderer.SetPropertyBlock(_materialPropertyBlock);
            if (timer <= 0)
            {
                _isDissolving = false;
                _dissolveCallback?.Invoke();
            }
        }
    }
    public void StartDissolve()
    {
        _isDissolving = true;
        timer = dissolveTime;
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _materialPropertyBlock.SetFloat("_CutoffHeight", _dissolveMaxHeight);
        _renderer.SetPropertyBlock(_materialPropertyBlock);
    }
    public void StopDissolve()
    {
        _isDissolving = false;
    }
    public void ResetDissolve()
    {
        _renderer.GetPropertyBlock(_materialPropertyBlock);
        _currentDissolveHeight = _dissolveMaxHeight;
        _materialPropertyBlock.SetFloat("_CutoffHeight", _dissolveMaxHeight);
        _renderer.SetPropertyBlock(_materialPropertyBlock);

    }
}
