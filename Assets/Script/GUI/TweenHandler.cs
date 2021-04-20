using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TweenHandler : MonoBehaviour
{
    public enum ETweenProperty
    {
        Move = TweenAction.MOVE_LOCAL,
        MoveX = TweenAction.MOVE_LOCAL_X,
        MoveY = TweenAction.MOVE_LOCAL_Y,
        MoveWorld = TweenAction.MOVE,
        Rotate = TweenAction.ROTATE_LOCAL,
        Scale = TweenAction.SCALE,
        Fade = TweenAction.CANVAS_ALPHA,
        Size = TweenAction.CANVAS_SIZEDELTA
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public string TweenDescription;
    public List<GameObject> ObjectsToTween;
    public ETweenProperty TweenProperty;
    public LeanTweenType EaseType;
    public AnimationCurve Curve;
    public float Duration;
    public float Delay;

    public bool useSpecifiedFrom = true;
    public Vector3 From;
    public Vector3 To;
    public bool Loop;
    public int PingPong;

    private LTDescr _tweenObject;

    // --------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        // Use own gameobject if tween targets unspecified
        if (ObjectsToTween == null || ObjectsToTween.Count == 0)
            ObjectsToTween = new List<GameObject>() { gameObject };
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Begin(Action onComplete = null)
    {
        foreach (var toTween in ObjectsToTween)
        {
            switch (TweenProperty)
            {
                case ETweenProperty.Move:
                    Move(toTween);
                    break;
                case ETweenProperty.MoveWorld:
                    MoveWorld(toTween);
                    break;
                case ETweenProperty.MoveX:
                    Move(toTween);
                    break;
                case ETweenProperty.MoveY:
                    Move(toTween);
                    break;
                case ETweenProperty.Rotate:
                    Rotate(toTween);
                    break;
                case ETweenProperty.Scale:
                    Scale(toTween);
                    break;
                case ETweenProperty.Fade:
                    Fade(toTween);
                    break;
                case ETweenProperty.Size:
                    Size(toTween);
                    break;
            }

            if (_tweenObject != null)
            {
                _tweenObject.setDelay(Delay);
                _tweenObject.setEase(EaseType);
                if (EaseType == LeanTweenType.animationCurve)
                    _tweenObject.setEase(Curve);

                if (Loop)
                    _tweenObject.setLoopClamp();

                if (PingPong > 0)
                    _tweenObject.setLoopPingPong(PingPong);
                else if (PingPong == -1)
                    _tweenObject.setLoopPingPong();

                if (onComplete != null)
                    _tweenObject.setOnComplete(onComplete);
            }
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Cancel()
    {
        foreach (var toTween in ObjectsToTween)
            LeanTween.cancel(toTween);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Move(GameObject toTween)
    {
        if (useSpecifiedFrom)
            toTween.transform.localPosition = From;

        if (TweenProperty == ETweenProperty.MoveX)
            _tweenObject = LeanTween.moveLocalX(toTween, To.x, Duration);
        else if (TweenProperty == ETweenProperty.MoveY)
            _tweenObject = LeanTween.moveLocalY(toTween, To.y, Duration);
        else
            _tweenObject = LeanTween.moveLocal(toTween, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void MoveWorld(GameObject toTween)
    {
        if (useSpecifiedFrom)
            toTween.transform.position = From;

        _tweenObject = LeanTween.move(toTween, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Rotate(GameObject toTween)
    {
        if (useSpecifiedFrom)
            toTween.transform.localEulerAngles = From;

        _tweenObject = LeanTween.rotateLocal(toTween, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Scale(GameObject toTween)
    {
        if (useSpecifiedFrom)
            toTween.transform.localScale = From;

        _tweenObject = LeanTween.scale(toTween, To, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Fade(GameObject toTween)
    {
        CanvasGroup cg = toTween.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = toTween.AddComponent<CanvasGroup>();

        if (useSpecifiedFrom)
            cg.alpha = From.x;

        _tweenObject = LeanTween.alphaCanvas(toTween.GetComponent<CanvasGroup>(), To.x, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Size(GameObject toTween)
    {
        var rt = toTween.transform as RectTransform;

        Vector2 to = new Vector2(
            (To.x == -1) ? rt.sizeDelta.x : To.x,
            (To.y == -1) ? rt.sizeDelta.y : To.y
        );

        if (useSpecifiedFrom)
        {
            Vector2 from = new Vector2(
                (From.x == -1) ? rt.sizeDelta.x : From.x,
                (From.y == -1) ? rt.sizeDelta.y : From.y
            );

            rt.sizeDelta = from;
        }

        _tweenObject = LeanTween.size(rt, To, Duration);
    }
}
