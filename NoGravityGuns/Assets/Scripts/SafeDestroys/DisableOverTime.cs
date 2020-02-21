using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableOverTime : MonoBehaviour
{
    public void DisableOverT(float t)
    {
        if (gameObject.activeInHierarchy)
            StartCoroutine(DisableOverTimeCoroutine(t));
    }

    IEnumerator DisableOverTimeCoroutine(float t)
    {
        yield return new WaitForSeconds(t);
        gameObject.SetActive(false);
    }

}
