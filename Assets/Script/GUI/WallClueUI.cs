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
        [HideInInspector] public string Connection;
        [HideInInspector] public bool GroupFound;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnPointerDown(PointerEventData eventData)
        {
            Selected = true;
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
