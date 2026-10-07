using UnityEngine;

// Un paquete físico del mundo. Se coge con el botón de interactuar.
// Suelto tiene física (cae, se puede empujar); en la mano se vuelve "fantasma":
// sin colisiones y sin física, pegado al punto donde lo lleva el jugador.
[RequireComponent(typeof(Rigidbody))]
public class Package : MonoBehaviour, IInteractable
{
    [SerializeField] private PackageData data;

    public PackageData Data => data;
    public bool IsHeld { get; private set; }
    public string GetInteractionPrompt(GameObject interactor) => $"Coger: {data.displayName}";

    private Rigidbody rb;
    private Collider[] colliders;

    private void Awake()
    {
        if (data == null)
        {
            Debug.LogError($"{name}: falta asignar un PackageData en el Package.", this);
        }
        rb = GetComponent<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();
    }

    public void Interact(GameObject interactor)
    {
        if (interactor.TryGetComponent(out PlayerCarry carry))
        {
            carry.TryPickUp(this);
        }
    }

    // Lo pega a un punto (la mano del jugador, la parrilla de la bici...)
    public void AttachTo(Transform holder)
    {
        IsHeld = true;
        rb.isKinematic = true;
        SetCollidersEnabled(false);
        transform.SetParent(holder, false);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    // Lo suelta en el mundo, con la física activada otra vez
    public void Detach(Vector3 position, Quaternion rotation)
    {
        transform.SetParent(null);
        transform.SetPositionAndRotation(position, rotation);
        SetCollidersEnabled(true);
        rb.isKinematic = false;
        IsHeld = false;
    }

    private void SetCollidersEnabled(bool enabled)
    {
        foreach (Collider c in colliders)
        {
            c.enabled = enabled;
        }
    }
}
