using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LegFixer : MonoBehaviour
{
    public Transform attachedTransform;

    Rigidbody2D rb;
    Vector3 startingPos;
    HingeJoint2D hingeJointBodyPart;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        startingPos = transform.position;
        hingeJointBodyPart = GetComponent<HingeJoint2D>();

    }

    public void ResetLeg()
    {
        rb.isKinematic = true;
        transform.localPosition = startingPos;
        rb.isKinematic = false;
    }

    private void Start()
    {
        //hingeJoint.
    }

    // Update is called once per frame
    void Update()
    {
        //if(Vector2.Distance(startingPos, transform.position) > 0.5f)
        //{
        //    Debug.Log("RESETTING RAGDOLL " + gameObject.name + "!");
        //    rb.isKinematic = true;
        //    transform.localPosition = startingPos;
        //    rb.isKinematic = false;
        //}
    }
}
