using System;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerPresenter : MonoBehaviour
{
    private Rigidbody _rb;
    private PlayerModel _model;
    private PlayerInput _pInput;

    [SerializeField]
    private GameObject _mesh;

    public Action<bool> OnPlayerMoving { get; set; }
    public Action<int> OnCoinsCollected { get; set; }
    public Action<int, int> OnPlayerHealthChangedUI { get; set; } // Cambio de vida en IU
    public Action OnPlayerDiedUI { get; set; } // Pantalla de perder

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _model = GetComponent<PlayerModel>();
        _pInput = GetComponent<PlayerInput>();

    }

    private void OnEnable()
    {
        if (_model != null)
        {
            _model.OnCoinsChanged += PresenterCoinsChanged;
            _model.OnHealthChanged += HandleModelHealthChanged;
            _model.OnPlayerDied += HandleModelPlayerDied;
        }
    }

    private void OnDisable()
    {
        if (_model != null)
        {
            _model.OnCoinsChanged -= PresenterCoinsChanged;
            _model.OnHealthChanged -= HandleModelHealthChanged;
            _model.OnPlayerDied -= HandleModelPlayerDied;
        }
    }

    void FixedUpdate()
    {
        if (_pInput != null && !_pInput.enabled)
        {
            return;
        }

        Vector3 input = _pInput.Axis;
        ApplyMovement(input);
        UpdateTilt(input.x);
    }

    public void ApplyMovement(Vector3 direction)
    {
        if (_model == null) return;
        _rb.velocity = _model.CalculateMove(direction);

        bool isMoving = direction.magnitude > 0.1f;
        OnPlayerMoving?.Invoke(isMoving);
    }

    private void UpdateTilt(float inputX)
    {
        if (_model == null || _mesh == null) return;
        Quaternion targetRotation = _model.CalculateTargetRotation(inputX);
        Quaternion currentRotation = _mesh.transform.localRotation;
        _mesh.transform.localRotation = Quaternion.Slerp(currentRotation, targetRotation, _model.TiltSpeed * Time.fixedDeltaTime);
    }

    public void ProcessDamage(int damageAmount)
    {
        if (_model != null)
        {
            _model.TakeDamage(damageAmount);
        }
    }

    private void HandleModelHealthChanged(int currentHealth, int maxHealth)
    {
        OnPlayerHealthChangedUI?.Invoke(currentHealth, maxHealth);
    }

    private void HandleModelPlayerDied()
    {
        if (_pInput != null)
        {
            _pInput.enabled = false; // Apagar inputs
        }
        OnPlayerDiedUI?.Invoke(); // Notificar pantalla de perder
    }

    private void OnTriggerEnter(Collider other)
    {
        if (_model == null) return;

        if (other.CompareTag("Coin"))
        {
            _model.AddCoin();
            Destroy(other.gameObject);
        }
    }

    private void PresenterCoinsChanged(int newCoinCount)
    {
        OnCoinsCollected?.Invoke(newCoinCount);
    }
}