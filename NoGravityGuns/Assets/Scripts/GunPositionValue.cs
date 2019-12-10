using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPositionValue : MonoBehaviour
{
    public Vector3 position;
    public Transform gunConnectionPoint;
    public Vector3 handConnectionPoint;

    //[Header("Leave blank if none required")]
    //public string AnimationTrigger;

    //public void SetHandPositions(ArmsScript arms)
    //{
    //    Animator animator = arms.GetComponent<Animator>();

    //    if(animator !=null && AnimationTrigger!="")
    //    {
    //        animator.SetTrigger(AnimationTrigger);
    //    }
    //}

    public void SetHandPositions(ArmsScript arms)
    {
        this.gameObject.transform.localPosition = gunConnectionPoint.transform.localPosition + new Vector3(Mathf.Sqrt(Mathf.Pow((gunConnectionPoint.localPosition.x - this.gameObject.transform.localPosition.x), 2.0f)), Mathf.Sqrt(Mathf.Pow((gunConnectionPoint.localPosition.y - this.gameObject.transform.localPosition.y), 2.0f)), 0.0f);
        //this.gameObject.transform.localPosition = arms.connectionPoint.transform.localPosition - new Vector3(connectionPoint.position.x, 0.0f, 0.0f);
        
    }

}
