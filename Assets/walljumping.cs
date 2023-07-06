using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class walljumping : MonoBehaviour
{
    [Header("References")]
    public playermovement pm;
    public Transform orientation;
    public Transform cam;
    public Rigidbody rb;

    [Header("Ledge Detection")]
    public float ledgeDetectionLenght;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;
    public float maxLedgeGrabDistance;
    bool readyToJump;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLenght, whatIsLedge);

        if (!ledgeDetected) return;
        
        float distanceToLedge = Vector3.Distance(transform.position,ledgeHit.transform.position);

        if (distanceToLedge > maxLedgeGrabDistance) readyToJump = true;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        LedgeDetection();
    }
}
