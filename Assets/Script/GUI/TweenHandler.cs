using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
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
    public bool Disabled;
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

    private LTDescr tweenObject;
    private Vector3 to;
    private Vector3 from;

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
        if (Disabled)
            return;

        if (reverse)
            SwapFromTo();

        to = Vector3.zero;
        from = Vector3.zero;

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

            if (tweenObject != null)
            {
                //if (useSpecifiedFrom)
                //    tweenObject.setFrom(from);

                float delay = Delay + i * Stagger;
                tweenObject.setDelay(delay);

                tweenObject.setEase(EaseType);
                if (EaseType == LeanTweenType.animationCurve)
                    tweenObject.setEase(Curve);

                if (Loop)
                    tweenObject.setLoopClamp();

                if (PingPong > 0)
                    tweenObject.setLoopPingPong(PingPong);
                else if (PingPong == -1)
                    tweenObject.setLoopPingPong();

                if (onComplete != null)
                    tweenObject.setOnComplete(onComplete);
            }
        }

        if (reverse)
            SwapFromTo();
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
    public void SetToFromValues(Vector3 vec)
    {
        SetToFromValues(vec.x, vec.y, vec.z);
    }

    public void SetToFromValues(float x, float y = 0, float z = 0)
    {
        to = new Vector3(
            (To.x == -1) ? x : To.x,
            (To.y == -1) ? y : To.y,
            (To.z == -1) ? z : To.z
        );

        from = new Vector3(
            (From.x == -1) ? x : From.x,
            (From.y == -1) ? y : From.y,
            (From.z == -1) ? z : From.z
        );
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Move(GameObject toTween)
    {
        SetToFromValues(toTween.transform.localPosition);
        if (useSpecifiedFrom)
            toTween.transform.localPosition = from;

        if (TweenProperty == ETweenProperty.MoveX)
            tweenObject = LeanTween.moveLocalX(toTween, to.x, Duration);
        else if (TweenProperty == ETweenProperty.MoveY)
            tweenObject = LeanTween.moveLocalY(toTween, to.y, Duration);
        else
            tweenObject = LeanTween.moveLocal(toTween, to, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void MoveWorld(GameObject toTween)
    {
        SetToFromValues(toTween.transform.position);
        if (useSpecifiedFrom)
            toTween.transform.position = from;
        tweenObject = LeanTween.move(toTween, to, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Rotate(GameObject toTween)
    {
        SetToFromValues(toTween.transform.localEulerAngles);
        if (useSpecifiedFrom)
            toTween.transform.localEulerAngles = from;
        tweenObject = LeanTween.rotateLocal(toTween, to, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Scale(GameObject toTween)
    {
        SetToFromValues(toTween.transform.localScale);
        if (useSpecifiedFrom)
            toTween.transform.localScale = from;
        tweenObject = LeanTween.scale(toTween, to, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Fade(GameObject toTween)
    {
        CanvasGroup cg = toTween.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = toTween.AddComponent<CanvasGroup>();

        SetToFromValues(cg.alpha);
        if (useSpecifiedFrom)
            cg.alpha = from.x;
        tweenObject = LeanTween.alphaCanvas(toTween.GetComponent<CanvasGroup>(), to.x, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void Size(GameObject toTween)
    {
        var rt = toTween.GetComponent<RectTransform>();
        SetToFromValues(rt.sizeDelta);
        if (useSpecifiedFrom)
            rt.sizeDelta = from;
        tweenObject = LeanTween.size(rt, to, Duration);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void PreferredSize(GameObject toTween)
    {
        var le = toTween.GetComponent<LayoutElement>();
        SetToFromValues(le.preferredWidth, le.preferredHeight);
        if (useSpecifiedFrom)
        {
            le.preferredWidth = from.x;
            le.preferredHeight= from.y;
        }

        void callback(Vector3 value)
        {
            var le = toTween.GetComponent<LayoutElement>();
            le.preferredWidth = value.x;
            le.preferredHeight = value.y;
        }

        tweenObject = LeanTween.value(toTween, callback, from, to, Duration);
    }
}

