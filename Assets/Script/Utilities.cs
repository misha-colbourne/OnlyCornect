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
    public static void SetVisible(this GameObject go, bool visible)
    {
        CanvasGroup cg = go.GetComponent<CanvasGroup>();
        cg.alpha = visible ? 1 : 0;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public static IEnumerator WaitAFrameThenRun(Action action)
    {
        yield return null;
        action.Invoke();
    }
}
