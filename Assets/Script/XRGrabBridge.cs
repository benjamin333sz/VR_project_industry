using UnityEngine;
#if UNITY_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

// Pont optionnel pour XR Interaction Toolkit.
// Si vous utilisez XRGrabInteractable, attachez ce script au même GameObject et référencez HandleZGrab.
[DisallowMultipleComponent]
public class XRGrabBridge : MonoBehaviour
{
    public HandleZGrab handle;

#if UNITY_XR_INTERACTION_TOOLKIT
    XRGrabInteractable grab;

    void Awake()
    {
        if (handle == null) Debug.LogWarning("HandleZGrab non référencé sur XRGrabBridge.", this);
        grab = GetComponent<XRGrabInteractable>();
        if (grab != null)
        {
            grab.selectEntered.AddListener(OnSelectEntered);
            grab.selectExited.AddListener(OnSelectExited);
        }
    }

    void OnDestroy()
    {
        if (grab != null)
        {
            grab.selectEntered.RemoveListener(OnSelectEntered);
            grab.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        // tenter de récupérer l'interactor comme XRBaseInteractor pour obtenir sa Transform
        var interactor = args.interactorObject as XRBaseInteractor;
        if (interactor != null)
        {
            handle?.StartGrab(interactor.transform);
        }
        else
        {
            Debug.LogWarning("Impossible de caster l'interactor en XRBaseInteractor.", this);
        }
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        handle?.EndGrab();
    }
#else
    void Awake()
    {
        Debug.LogWarning("XRGrabBridge : UNITY_XR_INTERACTION_TOOLKIT non défini. Si vous utilisez XR Interaction Toolkit, définissez le symbole de compilation UNITY_XR_INTERACTION_TOOLKIT ou utilisez le bridge manuellement.", this);
    }
#endif
}