using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunPositionValue : MonoBehaviour
{
    public Vector3 position;
    [Header("Leave blank if none required")]
    public string AnimationTrigger;

    public void SetHandPositions(ArmsScript arms)
    {
        Animator animator = arms.GetComponent<Animator>();

        if(animator !=null && AnimationTrigger!="")
        {
            animator.SetTrigger(AnimationTrigger);
        }

    }

}
