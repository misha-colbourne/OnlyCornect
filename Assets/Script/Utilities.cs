using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void SetActive(this MonoBehaviour mb)
    {
        mb.gameObject.SetActive(true);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void SetInactive(this MonoBehaviour mb)
    {
        mb.gameObject.SetActive(false);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void SetVisible(this GameObject go, bool visible)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = go.AddComponent<CanvasGroup>();
        cg.alpha = visible ? 1 : 0;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void SetVisible(this GameObject go, float alpha)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        if (cg == null)
            cg = go.AddComponent<CanvasGroup>();
        cg.alpha = alpha;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static bool IsVisible(this GameObject go)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        return cg != null && cg.alpha > 0;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static RectTransform GetRectTransform(this GameObject go)
    {
        return go.transform as RectTransform;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static IEnumerator WaitAFrameThenRun(Action action)
    {
        yield return null;
        action.Invoke();
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void ForceGridUpdates(this GridLayoutGroup grid)
    {
        grid.CalculateLayoutInputHorizontal();
        grid.CalculateLayoutInputVertical();
        grid.SetLayoutHorizontal();
        grid.SetLayoutVertical();
    }

}
