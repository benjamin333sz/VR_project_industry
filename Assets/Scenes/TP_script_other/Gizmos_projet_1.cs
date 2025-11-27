using UnityEditor;
using UnityEngine;
using System.Collections;

public class Gizmos_projet_1 : MonoBehaviour

    

{
    [Header("Gizmo (éditeur)")]
    public bool showGizmo = true;
    public Color gizmoAxisColor = Color.cyan;
    public Color gizmoThresholdColor = Color.yellow;
    public Color gizmoMaxColor = Color.red;
    public float range = 0.1f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        OnDrawGizmos();
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
#endif
}
