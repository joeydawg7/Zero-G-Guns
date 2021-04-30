using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOnParticleSystemEnd : MonoBehaviour
{
    // Start is called before the first frame update
    private void OnParticleSystemStopped()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
