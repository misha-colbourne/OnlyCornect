using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class WallClueUI : ClueUI, IPointerDownHandler
    {
        public Toggle ToggleButton;

        [HideInInspector] public string Connection;
        [HideInInspector] public bool GroupFound;

        [HideInInspector] public TweenHandler tweenShrinkOnClick            { get { return GetComponents<TweenHandler>()[0]; } }
        [HideInInspector] public TweenHandler tweenShakeOnIncorrectGroup    { get { return GetComponents<TweenHandler>()[1]; } }
        [HideInInspector] public TweenHandler tweenMoveOnCorrectGroupFound  { get { return GetComponents<TweenHandler>()[2]; } }
        //[HideInInspector] public TweenHandler tweenScaleOnCorrectGroupFound { get { return GetComponents<TweenHandler>()[3]; } }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnPointerDown(PointerEventData eventData)
        {
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public override void OnPointerEnter(PointerEventData eventData)
        {
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public override void OnPointerExit(PointerEventData eventData)
        {
        }

    }
}
