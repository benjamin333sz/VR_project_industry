using System.Collections;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class HandleZGrab : MonoBehaviour
{
    [Header("Référence de l'axe (utiliser un GameObject dont forward = axe Z voulu)")]
    public Transform referenceOrigin;

    [Header("Distance max (positive) depuis la position initiale le long de referenceOrigin.forward")]
    public float maxDistance = 0.2f;

    [Header("Distance (depuis la position initiale) pour déclencher l'évènement")]
    public float threshold = 0.15f;

    [Header("Vitesse de retour lors du relâchement (1 = immédiat, >0 = plus rapide)")]
    public float returnSpeed = 5f;

    [Header("Damping du suivi pendant la préhension")]
    public float followSmooth = 20f;

    [Header("Evènement quand le seuil est atteint (invoked une fois par prise)")]
    public UnityEvent OnReachedThreshold;

    [Header("Gizmo (éditeur)")]
    public bool showGizmo = true;
    public Color gizmoAxisColor = Color.cyan;
    public Color gizmoThresholdColor = Color.yellow;
    public Color gizmoMaxColor = Color.red;
    public float gizmoSphereSize = 0.01f;

    Vector3 initialWorldPos;
    float initialProjZ; // projection initiale le long de referenceOrigin.forward
    Transform grabbedHand;
    bool thresholdFired;
    Coroutine returnCoroutine;

    void Awake()
    {
        if (referenceOrigin == null)
        {
            // fallback : prendre le parent ou soi-même
            referenceOrigin = transform.parent != null ? transform.parent : transform;
        }

        // --- Ensure Rigidbody present and configured so the handle doesn't fall ---
        var rb = GetComponent<Rigidbody>();
        if (rb == null)
            rb = gameObject.AddComponent<Rigidbody>();

        // We move the handle by transform; prevent physics from applying gravity / forces.
        rb.isKinematic = true;
        rb.useGravity = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        // ----------------------------------------------------------------------

        initialWorldPos = transform.position;
        initialProjZ = Vector3.Dot(referenceOrigin.forward, initialWorldPos - referenceOrigin.position);
    }

    void Update()
    {
        if (grabbedHand != null)
        {
            FollowHand();
        }
    }

    void FollowHand()
    {
        // projection de la main sur l'axe forward de referenceOrigin (valeur scalaire Z)
        float handProjZ = Vector3.Dot(referenceOrigin.forward, grabbedHand.position - referenceOrigin.position);

        // clamp entre initialProjZ et initialProjZ + maxDistance (force mouvement positif seulement)
        float desiredProjZ = Mathf.Clamp(handProjZ, initialProjZ, initialProjZ + Mathf.Abs(maxDistance));

        float delta = desiredProjZ - initialProjZ;

        // calcul de la position cible en conservant l'offset latéral initial
        Vector3 targetPos = initialWorldPos + referenceOrigin.forward * delta;

        // smoothing pour éviter saccades
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * followSmooth);

        // déclencher l'évènement si le seuil est dépassé (une seule fois par prise)
        if (!thresholdFired && delta >= threshold)
        {
            thresholdFired = true;
            OnReachedThreshold?.Invoke();
        }
    }

    // Appelé pour démarrer la préhension — passer la transform de la main / interactor
    public void StartGrab(Transform hand)
    {
        if (hand == null) return;

        if (returnCoroutine != null)
        {
            StopCoroutine(returnCoroutine);
            returnCoroutine = null;
        }

        grabbedHand = hand;
        thresholdFired = false;
    }

    // Appelé pour relâcher
    public void EndGrab()
    {
        if (grabbedHand == null) return;

        grabbedHand = null;

        // lancer coroutine de retour
        if (returnCoroutine != null) StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(ReturnToInitial());
    }

    IEnumerator ReturnToInitial()
    {
        // remet également le flag de seuil
        thresholdFired = false;

        while ((transform.position - initialWorldPos).sqrMagnitude > 0.0001f)
        {
            transform.position = Vector3.Lerp(transform.position, initialWorldPos, Time.deltaTime * returnSpeed);
            yield return null;
        }

        transform.position = initialWorldPos;
        returnCoroutine = null;
    }

    // Utilitaires optionnels : exposition d'un état
    public bool IsGrabbed => grabbedHand != null;

    // Gizmo pour visualiser l'axe, le seuil et la limite max dans la scène (visible quand l'objet est sélectionné)
    void OnDrawGizmosSelected()
    {
        if (!showGizmo) return;

        var origin = referenceOrigin != null ? referenceOrigin : (transform.parent != null ? transform.parent : transform);
        Vector3 start = transform.position;
        Vector3 dir = origin.forward.normalized;

        // Axe
        Gizmos.color = gizmoAxisColor;
        Gizmos.DrawLine(start, start + dir * maxDistance);

        // Point de départ
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(start, gizmoSphereSize * 0.8f);

        // Seuil
        Gizmos.color = gizmoThresholdColor;
        Gizmos.DrawSphere(start + dir * Mathf.Clamp(threshold, 0f, maxDistance), gizmoSphereSize);

        // Max
        Gizmos.color = gizmoMaxColor;
        Gizmos.DrawSphere(start + dir * maxDistance, gizmoSphereSize * 1.2f);

        // Petite flèche directionnelle
        Vector3 tip = start + dir * maxDistance;
        Vector3 right = Vector3.Cross(dir, Vector3.up).normalized * gizmoSphereSize * 1.5f;
        Gizmos.DrawLine(tip, tip - dir * gizmoSphereSize * 1.5f + right);
        Gizmos.DrawLine(tip, tip - dir * gizmoSphereSize * 1.5f - right);
    }
}