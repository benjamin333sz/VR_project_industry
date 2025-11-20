using UnityEngine;
using UnityEngine.Rendering;

public class TreadmillForce : MonoBehaviour
{
    public float currentSpeed { get; private set; }

    public float GetSpeed() {return currentSpeed;}
    private void OnTriggerStay(Collider other)
    {
        Rigidbody body = other.attachedRigidbody;
        if (body != null)
        {
            Vector3 velocity = body.linearVelocity;
            velocity = transform.InverseTransformVector(velocity);
            velocity.z = currentSpeed;
            velocity = transform.TransformVector(velocity);
            body.linearVelocity = velocity;
        }
    }

    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
}
