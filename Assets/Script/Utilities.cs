using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public static class Utilities
{
    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void Show(this MonoBehaviour mb)
    {
        mb.gameObject.SetActive(true);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static void Hide(this MonoBehaviour mb)
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
    public static IEnumerator WaitAFrameThenRun(Action action)
    {
        yield return null;
        action.Invoke();
    }

}
