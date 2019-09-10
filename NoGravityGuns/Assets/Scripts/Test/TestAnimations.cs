using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAnimations : MonoBehaviour
{

    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    bool thing=false;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (!thing)
            {
                anim.SetBool("isHPCritical", true);
                thing = true;
            }
            else
            {
                anim.SetBool("isHPCritical", false);
                thing = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            anim.SetTrigger("takeDamage");
        }
    }
}
