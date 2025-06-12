using UnityEngine;

public class MeteoriteModelFinal : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private int damageToPlayer = 1;
    [SerializeField] private float speed = 5f;

    public int DamageToPlayer => damageToPlayer;
    public float Speed => speed;

}