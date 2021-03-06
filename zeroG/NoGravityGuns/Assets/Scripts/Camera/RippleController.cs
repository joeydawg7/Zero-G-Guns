﻿//using System.Collections;

//using UnityEngine;
//using UnityEngine.Rendering.PostProcessing;

//public class RippleController : MonoBehaviour
//{
//    [SerializeField] private float maxAmount = 25f;
//    [SerializeField] private float friction = .95f;

//    private Coroutine rippleRoutine;
//    //private Ripple ripple;
//    private PostProcessVolume rippleVolume;

//    private void Start()
//    {
//        ripple = ScriptableObject.CreateInstance<Ripple>();
//        ripple.enabled.Override(false);
//        ripple.Amount.Override(0f);
//        ripple.WaveAmount.Override(10f);
//        ripple.WaveSpeed.Override(15f);
//        rippleVolume = PostProcessManager.instance.QuickVolume(gameObject.layer, 100f, ripple);
//    }

//    private void OnDestroy()
//    {
//        StopAllCoroutines();
//        //RuntimeUtilities.DestroyVolume(rippleVolume, true, true);
//    }

//    private void Update()
//    {
//        //on mouse click ripples
//        //if (Input.GetMouseButtonDown(0))
//        //{
//        //    Vector2 position = Input.mousePosition;

//        //    if (rippleRoutine != null)
//        //        StopCoroutine(rippleRoutine);

//        //    ripple.CenterX.Override(position.x / Screen.width);
//        //    ripple.CenterY.Override(position.y / Screen.height);

//        //    //rippleRoutine = StartCoroutine(DoRipple());
//        //    Ripple(position);
//        //}
//    }
//    //25, 0.95
//    public void Ripple(Vector3 pos, float maxAmount = 120, float friction = 0.97f )
//    {
//        if (rippleRoutine != null)
//            StopCoroutine(rippleRoutine);

//        pos = Camera.main.WorldToScreenPoint(pos);


//        ripple.CenterX.Override(pos.x / Screen.width);
//        ripple.CenterY.Override(pos.y / Screen.height);

//        Debug.DrawLine(new Vector2(pos.x / Screen.width, pos.y / Screen.height), new Vector2(0, 0), Color.magenta, 10f);

//        this.maxAmount = maxAmount;
//        this.friction = friction;

//        rippleRoutine = StartCoroutine(DoRipple());
//    }

//    private IEnumerator DoRipple()
//    {
        

//        ripple.enabled.Override(true);

//        float amount = maxAmount;
//        Debug.Log(amount);
//        while (amount > .5f)
//        {
//            ripple.Amount.value = amount;
//            amount *= friction;
//            yield return null;
//        }

//        ripple.enabled.Override(false);
//    }
//}