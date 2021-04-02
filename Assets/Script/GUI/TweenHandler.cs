using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TweenHandler : MonoBehaviour
{
    public enum ETweenProperty
    {
        Move = 35,
        Rotate = 38,
        Scale = 39,
        Fade = 23
    }

    [SerializeField]
    private ETweenProperty _tweenProperty;
    [HideInInspector]
    public TweenAction TweenProperty;

    public LeanTweenType EaseType;
    public AnimationCurve Curve;
    public float Duration;
    public float Delay;

    public bool useSpecifiedFrom = true;
    public Vector3 From;
    public Vector3 To;

    private LTDescr _tweenObject;

    // --------------------------------------------------------------------------------------------------------------------------------------
    // Start is called before the first frame update
    void Start()
    {
        TweenProperty = (TweenAction)_tweenProperty;
    }

    // Update is called once per frame
    void Update()
    {

    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Begin()
    {
        switch (TweenProperty)
        {
            case TweenAction.MOVE_LOCAL:
                Move();
                break;
            case TweenAction.ROTATE_LOCAL:
                Rotate();
                break;
            case TweenAction.SCALE:
                Scale();
                break;
            case TweenAction.CANVAS_ALPHA:
                Fade();
                break;
        }

        if (_tweenObject != null)
        {
            _tweenObject.setDelay(Delay);
            _tweenObject.setEase(EaseType);
            if (EaseType == LeanTweenType.animationCurve)
                _tweenObject.setEase(Curve);
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Move()
    {
        if (useSpecifiedFrom)
            gameObject.transform.localPosition = From;

        _tweenObject = LeanTween.moveLocal(gameObject, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Rotate()
    {
        if (useSpecifiedFrom)
            gameObject.transform.localEulerAngles = From;

        _tweenObject = LeanTween.rotateLocal(gameObject, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Scale()
    {
        if (useSpecifiedFrom)
            gameObject.transform.localScale = From;

        _tweenObject = LeanTween.scale(gameObject, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Fade()
    {
        if (gameObject.GetComponent<CanvasGroup>() == null)
            gameObject.AddComponent<CanvasGroup>();

        var cg = gameObject.GetComponent<CanvasGroup>();

        if (useSpecifiedFrom)
            cg.alpha = From.x;

        _tweenObject = LeanTween.alphaCanvas(gameObject.GetComponent<CanvasGroup>(), To.x, Duration);
    }



    //[HideInInspector]
    //public float Elapsed = 0;

    //private IEnumerator coroutine;
    //private Quaternion startQ;
    //private Quaternion endQ;
    //private CanvasGroup canvasGroup;

    //// --------------------------------------------------------------------------------------------------------------------------------------
    //// Start is called before the first frame update
    //void Start()
    //{
    //    if (TweenProperty == ETweenProperty.Position)
    //    {
    //        if (UseOwnPosAsFrom)
    //            from = transform.localPosition;
    //    }

    //    if (TweenProperty == ETweenProperty.Rotation)
    //    {
    //        startQ = Quaternion.Euler(from);
    //        endQ = Quaternion.Euler(to);
    //    }

    //    if (TweenProperty == ETweenProperty.Visibility)
    //    {
    //        canvasGroup = GetComponent<CanvasGroup>();
    //    }

    //    coroutine = Lerp();
    //}

    //// --------------------------------------------------------------------------------------------------------------------------------------
    //public void BeginLerp()
    //{
    //    StopCoroutine(coroutine);

    //    Elapsed = 0;
    //    coroutine = Lerp();

    //    StartCoroutine(coroutine);
    //}

    //// --------------------------------------------------------------------------------------------------------------------------------------
    //IEnumerator Lerp()
    //{
    //    yield return new WaitForSeconds(Delay);

    //    while (Elapsed < Duration)
    //    {
    //        float t = Elapsed / Duration;

    //        switch (TweenProperty)
    //        {
    //            case (ETweenProperty.Position):
    //                transform.localPosition = Vector3.Lerp(from, to, t);
    //                break;
    //            case (ETweenProperty.Rotation):
    //                transform.localRotation = Quaternion.Lerp(startQ, endQ, t);
    //                break;
    //            case (ETweenProperty.Visibility):
    //                canvasGroup.alpha = Mathf.Lerp(from.x, to.x, t);
    //                break;
    //        }

    //        Elapsed += Time.deltaTime;
    //        yield return null;
    //    }

    //    // Final pass 
    //    switch (TweenProperty)
    //    {
    //        case (ETweenProperty.Position):
    //            transform.localPosition = to;
    //            break;
    //        case (ETweenProperty.Rotation):
    //            transform.localRotation = endQ;
    //            break;
    //        case (ETweenProperty.Visibility):
    //            canvasGroup.alpha = to.x;
    //            break;
    //    }
    //}

}
