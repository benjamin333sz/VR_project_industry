using UnityEngine;

public class levierRotation : MonoBehaviour
{
    public Transform leverHandle;
    [Range(-70,70)]public float debugHandAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Mes calculs
        // Aboutissent a un angle
        float handAngle = ComputeHandAngle(leverHandle);
        SetLeverAngle(handAngle);
        // avec des quaternions
        leverHandle.eulerAngles = new Vector3(handAngle, 0, 0); // euler angles
        float handAngleDebug = debugHandAngle;
    }

    float ComputeHandAngle(Transform hand)
    {
        // FALSE IL FAUT RETROUVER LA BONNE FORMULE
        return Vector3.SignedAngle(hand.position,transform.up,transform.forward);
    }

    void SetLeverAngle(float angle)
    {
        leverHandle.rotation = Quaternion.Euler(angle, 0, 0);
    }
}
