using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeverPullXR : MonoBehaviour
{
    [Header("References")]
    public Transform leverHandle;
    public XRSimpleInteractable interactable;
    public TreadmillsController treadmillController;

    [Header("Limits")]
    public float maxPullDistance = 0.1f;

    private Transform currentHand;
    private bool isGrabbed = false;
    private float InitialPosition;
    private Coroutine returnCoroutine;
    private bool paused = false;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InitialPosition = leverHandle.localPosition.y;
    }
    private void OnEnable()
    {
        // Register event listeners
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Start grabbing the lever handle
        isGrabbed = true;
        currentHand = args.interactorObject.transform;

    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        currentHand = null;

        // Start returning the lever handle to its initial position
        if (returnCoroutine != null)
            StopCoroutine(returnCoroutine);
        returnCoroutine = StartCoroutine(ReturnHandleToInitialPosition());
    }

    private IEnumerator ReturnHandleToInitialPosition()
    {
        float duration = 5f;
        float elapsed = 0f;
        Vector3 startPos = leverHandle.localPosition;
        Vector3 targetPos = startPos;
        targetPos.y = InitialPosition;

        // Smoothly move the lever handle back to initial position
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, t);
            leverHandle.localPosition = newPos;
            yield return null;
        }
        leverHandle.localPosition = targetPos;
        returnCoroutine = null;
    }

    Vector3 ComputeHandPos(Transform hand)
    {
        // Convert hand position to local space of the lever
        Vector3 localPos = transform.InverseTransformPoint(hand.position);
        return localPos;
    }

    void MoveHandle(float handPosY)
    {
        // Clamp the lever handle position within limits
        float clampedY = Mathf.Clamp(handPosY, 0, maxPullDistance);
        Vector3 localPos = leverHandle.localPosition;
        localPos.y = clampedY;
        leverHandle.localPosition = localPos;
    }

    void UpdateTreadmillSpeed(bool pause)
    {
        // Safety check
        if (treadmillController == null)
        {
            Debug.LogWarning("TreadmillsController reference is missing.");
            return;
        }

        treadmillController.SetPaused(pause);
    }

    // Update is called once per frame
    void Update()
    {
        // Handle lever movement when grabbed
        if (isGrabbed && currentHand != null)
        {
            Vector3 handLocalPos = ComputeHandPos(currentHand);
            MoveHandle(handLocalPos.y);
        }

        // Check lever position to update treadmill speed
        float currentY = leverHandle.localPosition.y;
        if (Mathf.Abs(currentY - InitialPosition) >= maxPullDistance*0.9f)
        {
            paused = true;
            UpdateTreadmillSpeed(paused);

        }
        // Reset when lever is near initial position
        if (Mathf.Abs(currentY - InitialPosition) <= maxPullDistance * 0.1f)
        {
            paused = false;
            UpdateTreadmillSpeed(paused);

        }
    }
}
