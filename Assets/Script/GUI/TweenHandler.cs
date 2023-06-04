using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using YamlDotNet.Core.Tokens;

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
        Size = TweenAction.CANVAS_SIZEDELTA,
        PreferredSize = 50
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public string TweenDescription;
    public List<GameObject> ObjectsToTween;
    public ETweenProperty TweenProperty;
    public LeanTweenType EaseType;
    public AnimationCurve Curve;
    public float Duration;
    public float Delay;
    public float Stagger;

    public bool useSpecifiedFrom = true;
    public Vector3 From;
    public Vector3 To;
    public bool Loop;
    public int PingPong;

    private LTDescr _tweenObject;
    private bool reverse;

    // --------------------------------------------------------------------------------------------------------------------------------------
    private void Awake()
    {
        // Use own gameobject if tween targets unspecified
        if (ObjectsToTween == null || ObjectsToTween.Count == 0)
            ObjectsToTween = new List<GameObject>() { gameObject };
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Begin(bool reverse = false, Action onComplete = null)
    {
        if (reverse)
            SwapFromTo();

        for (int i = 0; i < ObjectsToTween.Count; i++)
        {
            var toTween = ObjectsToTween[i];

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
                case ETweenProperty.PreferredSize:
                    PreferredSize(toTween);
                    break;
            }

            if (_tweenObject != null)
            {
                _tweenObject.setDelay(Delay + i * Stagger);
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

            if (reverse)
                SwapFromTo();
        }
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    private void SwapFromTo()
    {
        var temp = From;
        From = To;
        To = temp;
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

        Vector2 from = From;
        if (useSpecifiedFrom)
        {
            from = new Vector2(
                (From.x == -1) ? rt.sizeDelta.x : From.x,
                (From.y == -1) ? rt.sizeDelta.y : From.y
            );
        }

        _tweenObject = LeanTween.size(rt, to, Duration).setFrom(from);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void PreferredSize(GameObject toTween)
    {
        var le = toTween.GetComponent<LayoutElement>();

        Vector3 from = From;
        if (useSpecifiedFrom)
        {
            from = new Vector3(
                (From.x == -1) ? le.preferredWidth : From.x,
                (From.y == -1) ? le.preferredHeight : From.y
            );
        }

        Vector3 to = new Vector3(
            (To.x == -1) ? le.preferredWidth : To.x,
            (To.y == -1) ? le.preferredHeight : To.y
        );

        void callback(Vector3 value)
        {
            var le = toTween.GetComponent<LayoutElement>();
            le.preferredWidth = value.x;
            le.preferredHeight = value.y;
        }

        _tweenObject = LeanTween.value(toTween, callback, from, to, Duration);
    }
}

