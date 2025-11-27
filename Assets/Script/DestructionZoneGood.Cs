using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(BoxCollider))]
public class DestructionZoneGood : MonoBehaviour
{
    [Header("Event Bonus to destroy a spawn object")]
    public UnityEvent<GameObject> OnBonusObjectBinDestroyed;


    [Header("Event Malus to destroy a spawn object")]
    public UnityEvent<GameObject> OnMalusObjectBinDestroyed;

    private void Awake()
    {
        // Check collider mode Trigger
        var box = GetComponent<BoxCollider>();
        box.isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check object is spawned
        if (other.gameObject.CompareTag("Inappropriate"))
        {
        
            // Activation of the event to destroy
            OnBonusObjectBinDestroyed?.Invoke(other.gameObject);

            // Destroy the object
            Destroy(other.gameObject);
        }

        if (other.gameObject.CompareTag("Respawn")) 
        {

            // Activation of the event to destroy
            OnMalusObjectBinDestroyed?.Invoke(other.gameObject);

            // Destroy the object
            Destroy(other.gameObject);
        }
    }
}
