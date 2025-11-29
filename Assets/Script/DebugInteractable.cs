
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class DebugInteract : MonoBehaviour
{
    public XRSimpleInteractable interactable;

    void OnEnable()
    {
        interactable.hoverEntered.AddListener((args) => Debug.Log("Hover"));
        interactable.selectEntered.AddListener((args) => Debug.Log("SELECT !!!"));
        interactable.selectExited.AddListener((args) => Debug.Log("UN-SELECT"));
    }
}
