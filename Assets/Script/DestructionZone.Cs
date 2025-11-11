using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class DestructionZone : MonoBehaviour
{
    [Header("Event to destroy a spawn object")]
    public UnityEvent<GameObject> OnObjectDestroyed;

    private void Awake()
    {
        // Check collider mode Trigger
        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check object is spawned
        if (other.TryGetComponent(out SpawnedObjectTag spawnedTag))
        {
            // Activation of the event to destroy
            OnObjectDestroyed?.Invoke(other.gameObject);

            // Destroy the object
            Destroy(other.gameObject);
        }
    }
}
