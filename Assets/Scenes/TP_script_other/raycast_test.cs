using UnityEngine;
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
using UnityEngine.InputSystem;
#endif

public class raycast_test : MonoBehaviour
{
    public LayerMask layerMask = ~0;
    public float range = 100f;
    public float gizmoRadius = 0.5f;
    public Color gizmoColor = Color.red;
    public bool includeTriggerColliders = false;
    public Transform formCreate; // prefab à instancier avec la touche G

    private bool hasHit = false;
    private Vector3 hitPoint = Vector3.zero;
    private Vector3 lastRayOrigin = Vector3.zero;
    private Vector3 lastRayDirection = Vector3.forward;

    void Update()
    {
        var cam = Camera.main;
        if (cam == null) return;

        Vector2 mousePos = GetMousePosition();
        Ray ray = cam.ScreenPointToRay(new Vector3(mousePos.x, mousePos.y, 0f));
        lastRayOrigin = ray.origin;
        lastRayDirection = ray.direction;

        // Trace le rayon pour debug
        Debug.DrawRay(ray.origin, ray.direction * range, Color.green);

        RaycastHit hit;
        var qti = includeTriggerColliders ? QueryTriggerInteraction.Collide : QueryTriggerInteraction.Ignore;
        if (Physics.Raycast(ray, out hit, range, layerMask.value, qti))
        {
            hasHit = true;
            hitPoint = hit.point;
        }
        else
        {
            hasHit = false;
        }

        // Instanciation avec la touche G (gère ancien et nouveau input)
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        if (Keyboard.current != null && Keyboard.current.gKey.wasPressedThisFrame)
        {
            SpawnAtCursor();
        }
#else
        if (Input.GetKeyDown(KeyCode.G))
        {
            SpawnAtCursor();
        }
#endif
    }

    Vector2 GetMousePosition()
    {
#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        return Mouse.current != null ? Mouse.current.position.ReadValue() : Vector2.zero;
#else
        return Input.mousePosition;
#endif
    }

    void OnDrawGizmos()
    {
        // Dessine une sphère remplie uniquement si on a un hit
        if (!hasHit) return;
        Gizmos.color = gizmoColor;
        Gizmos.DrawSphere(hitPoint, gizmoRadius);
    }

    private void SpawnAtCursor()
    {
        if (formCreate == null)
        {
            Debug.LogWarning("formCreate non assigné dans l'inspector.");
            return;
        }

        Vector3 spawnPos;
        if (hasHit)
        {
            spawnPos = hitPoint;
        }
        else
        {
            // fallback : on instancie à une position le long du rayon si aucun hit
            spawnPos = lastRayOrigin + lastRayDirection * (range * 0.5f);
        }

        Instantiate(formCreate, spawnPos, Quaternion.identity);
    }
}