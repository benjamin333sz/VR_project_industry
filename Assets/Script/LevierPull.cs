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
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        currentHand = args.interactorObject.transform;

    }

    private void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        currentHand = null;

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
        Vector3 localPos = transform.InverseTransformPoint(hand.position);
        return localPos;
    }

    void MoveHandle(float handPosY)
    {
        float clampedY = Mathf.Clamp(handPosY, 0, maxPullDistance);
        Vector3 localPos = leverHandle.localPosition;
        localPos.y = clampedY;
        leverHandle.localPosition = localPos;
    }

    void UpdateTreadmillSpeed(bool pause)
    {
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
        if (isGrabbed && currentHand != null)
        {
            Vector3 handLocalPos = ComputeHandPos(currentHand);
            MoveHandle(handLocalPos.y);
        }

        float currentY = leverHandle.localPosition.y;
        if (Mathf.Abs(currentY - InitialPosition) >= maxPullDistance*0.9f)
        {
            paused = true;
            UpdateTreadmillSpeed(paused);

        }
        if (Mathf.Abs(currentY - InitialPosition) <= maxPullDistance * 0.1f)
        {
            paused = false;
            UpdateTreadmillSpeed(paused);

        }
    }
}
