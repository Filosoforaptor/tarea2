using System;
using UnityEngine;

public class PlayerModel : MonoBehaviour
{
    [SerializeField] private Quaternion initTiltRotation;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float tiltAngle = 30f; // cuanto inclina la nave
    [SerializeField] private float tiltSpeed = 5f;  // que tan rapido se inclina

    public int CurrentCoins { get; private set; }

    [Header("Estadisticas de vida")]
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    public event Action<int> OnCoinsChanged;
    public event Action<int, int> OnHealthChanged;
    public event Action OnPlayerDied;

    public float TiltSpeed { get => tiltSpeed; private set => tiltSpeed = value; }
    public int CurrentHealth => currentHealth;
    public int MaxHealth => maxHealth;

    void Awake()
    {
        currentHealth = maxHealth;
    }

    public Vector3 CalculateMove(Vector3 direction)
    {
        return direction.normalized * moveSpeed;
    }

    public Quaternion CalculateTargetRotation(float inputX)
    {
        float tiltZ = 0f;
        if (Mathf.Abs(inputX) > 0.01f)
            tiltZ = -inputX * tiltAngle;
        return initTiltRotation * Quaternion.Euler(0f, 0f, tiltZ);
    }

    public void AddCoin()
    {
        CurrentCoins++;
        Debug.Log("PlayerModel: Moneda agregada, total de monedas: " + CurrentCoins);
        OnCoinsChanged?.Invoke(CurrentCoins);
    }

    public void TakeDamage(int amount)
    {
        if (currentHealth <= 0) return;

        currentHealth -= amount;
        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            OnPlayerDied?.Invoke();
        }
    }
}