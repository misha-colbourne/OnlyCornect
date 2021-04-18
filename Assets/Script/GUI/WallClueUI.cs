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
