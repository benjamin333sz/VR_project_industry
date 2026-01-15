using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class LeverRotationXR : MonoBehaviour
{
    [Header("References")]
    public Transform leverHandle;
    public XRSimpleInteractable interactable;
    public TreadmillsController treadmillController;

    [Header("Limits")]
    public float minAngle = -60f;
    public float maxAngle = 60f;

    [Header("Interaction Settings")]
    public float sensitivity = 1.0f;

    private Transform currentHand;
    private bool isGrabbed = false;

    private float initialHandAngle;
    private float initialLeverAngle;

    private float lastNormalizedSpeed = 0f;

    private void OnEnable()
    {
        // Register event listeners
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    private void Start()
    {
        // Ensure treadmillController is assigned
        if (treadmillController == null)
            treadmillController = FindFirstObjectByType<TreadmillsController>();
    }


    private void OnDisable()
    {
        // Unregister event listeners
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        // Start grabbing the lever handle
        isGrabbed = true;
        currentHand = args.interactorObject.transform;

        initialHandAngle = ComputeHandAngle(currentHand);

        // Store the initial lever angle
        initialLeverAngle = leverHandle.localEulerAngles.x;
        // Adjust for angles greater than 180 degrees
        if (initialLeverAngle > 180) initialLeverAngle -= 360;
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        // Stop grabbing the lever handle
        isGrabbed = false;
        currentHand = null;
        if (treadmillController != null)
            treadmillController.SetTargetSpeed01(lastNormalizedSpeed);

    }

    private void Update()
    {
        // If the lever is being grabbed, update its rotation based on hand movement
        if (isGrabbed && currentHand != null)
        {
            float angle = ComputeHandAngle(currentHand);

            float delta = (angle - initialHandAngle) * sensitivity;
            float newAngle = Mathf.Clamp(initialLeverAngle + delta, minAngle, maxAngle);
            float newSpeed =Mathf.InverseLerp(minAngle, maxAngle, newAngle);
            lastNormalizedSpeed = newSpeed;
            SetLeverAngle(newAngle);
            UpdateTreadmillSpeed(newSpeed);
            
        }
    }

    float ComputeHandAngle(Transform hand)
    {
        // Convert hand position to local space of the lever
        Vector3 localPos = transform.InverseTransformPoint(hand.position);
        return Mathf.Atan2(localPos.y, localPos.z) * Mathf.Rad2Deg;
    }

    void SetLeverAngle(float angle)
    {
        // Set the lever handle's local rotation based on the computed angle
        leverHandle.localRotation = Quaternion.Euler(angle, 0f, 0f);
    }

    void UpdateTreadmillSpeed(float newSpeed)
    {
        // Safety check
        if (treadmillController == null) { 
            Debug.LogWarning("TreadmillsController reference is missing.");
            return; 
        }

        // Update the treadmill speed based on lever position
        treadmillController.SetSpeed(newSpeed);
    }
}
