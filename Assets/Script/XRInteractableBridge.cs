using UnityEngine;
#if UNITY_XR_INTERACTION_TOOLKIT
using UnityEngine.XR.Interaction.Toolkit;
#endif

[DisallowMultipleComponent]
public class XRInteractableBridge : MonoBehaviour
{ 
    [Tooltip("Référence au HandleZGrab à contrôler")]
    public HandleZGrab handle;

#if UNITY_XR_INTERACTION_TOOLKIT
    XRBaseInteractable interactable;

    void Awake()
    {
        if (handle == null)
            Debug.LogWarning("XRInteractableBridge : HandleZGrab non référencé.", this);

        interactable = GetComponent<XRBaseInteractable>();
        if (interactable == null)
        {
            Debug.LogWarning("XRInteractableBridge : aucun XRBaseInteractable trouvé sur ce GameObject.", this);
            return;
        }

        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    void OnDestroy()
    {
        if (interactable != null)
        {
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }
    }

    void OnSelectEntered(SelectEnterEventArgs args)
    {
        // Récupère l'interactor en tant que XRBaseInteractor pour obtenir sa Transform
        if (args.interactorObject is XRBaseInteractor xrInteractor)
            handle?.StartGrab(xrInteractor.transform);
        else
            Debug.LogWarning("XRInteractableBridge : interactor non convertible en XRBaseInteractor.", this);
    }

    void OnSelectExited(SelectExitEventArgs args)
    {
        handle?.EndGrab();
    }
#else
    void Awake()
    {
        Debug.LogWarning("XRInteractableBridge : UNITY_XR_INTERACTION_TOOLKIT non défini. Utiliser un bridge manuel ou définir le symbole de compilation.", this);
    }
#endif
}