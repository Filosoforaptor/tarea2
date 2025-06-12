using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))] // Asegura que este presente el collider
[RequireComponent(typeof(MeteoriteModelFinal))] // Asegura que este presente el modelo

public class MeteoritePresenterFinal : MonoBehaviour
{
    private MeteoriteModelFinal model;
    private MeteoriteViewFinal view;
    private Rigidbody rb;
    private bool isDestroyed = false;

    void Awake()
    {
        model = GetComponent<MeteoriteModelFinal>();
        view = GetComponent<MeteoriteViewFinal>();
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        if (model != null)
        {
            rb.velocity = Vector3.down * model.Speed;
        }
        else
        {
            Debug.LogError("MeteoritePresenter: MeteoriteModel No se encuentra modelo", this);
            rb.velocity = Vector3.down * 5f;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (isDestroyed) return;

        if (model == null)
        {
            Debug.LogError("MeteoritePresenter: MeteoriteModel OnTriggerEnter es null no puede colisionar.", this);
            return;
        }

        if (other.CompareTag("Player"))
        {
            Debug.Log("MeteoritePresenter: Colision con player.", this);
            PlayerPresenter playerPresenter = other.GetComponent<PlayerPresenter>();
            if (playerPresenter != null)
            {
                playerPresenter.ProcessDamage(model.DamageToPlayer);
            }
            else
            {
                Debug.LogError("MeteoritePresenter: PlayerPresenter No se encontro objeto con tag player.", other);
            }
            HandleDestruction();
        }
    }

    private void HandleDestruction()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        Debug.Log("MeteoritePresenter: Boom", this);

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
        }

        // Desabilitar el Rigidbody para evitar más físicas
        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            col.enabled = false;
        }

        // Desactivar el MeshRenderer para ocultar el meteorito
        if (view != null)
        {
            view.PlayDestructionEffects();
        }
        Destroy(gameObject); 
    }
}