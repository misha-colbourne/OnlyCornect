using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class WallClueUI : ClueUI, IPointerDownHandler
    {
        // --------------------------------------------------------------------------------------------------------------------------------------
        public override void OnPointerEnter(PointerEventData eventData)
        {
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public override void OnPointerExit(PointerEventData eventData)
        {
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnPointerDown(PointerEventData eventData)
        {
            Selected = true;
            Text.color = Color.white;
            //SelectableButton.onClick.Invoke();
        }
    }
}
