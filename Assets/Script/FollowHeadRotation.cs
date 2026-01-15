using UnityEngine;

public class FollowHeadRotation : MonoBehaviour
{
    // test text follow the head rotation on Y axis only
    // (it was a test, no finish)
    [SerializeField] Transform head;

    Vector3 fixedPosition;

    void Start()
    {
        fixedPosition = transform.position;
    }

    void LateUpdate()
    {
        transform.position = fixedPosition;

        float yaw = head.eulerAngles.y;
        transform.rotation = Quaternion.AngleAxis(yaw, Vector3.up);
    }
}
