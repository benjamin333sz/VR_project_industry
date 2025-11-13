using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class DestructionZone : MonoBehaviour
{
    [Header("Event Bonus to destroy a spawn object")]
    public UnityEvent<GameObject> OnBonusObjectDestroyed;


    [Header("Event Malus to destroy a spawn object")]
    public UnityEvent<GameObject> OnMalusObjectDestroyed;

    private void Awake()
    {
        // Check collider mode Trigger
        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check object is spawned
        if (other.gameObject.CompareTag("Respawn"))
        {
        
            // Activation of the event to destroy
            OnBonusObjectDestroyed?.Invoke(other.gameObject);

            // Destroy the object
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Inappropriate")) 
        {

            // Activation of the event to destroy
            OnMalusObjectDestroyed?.Invoke(other.gameObject);

            // Destroy the object
            Destroy(other.gameObject);
        }
    }
}
