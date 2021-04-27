using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class ClueUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public Image Overlay;
        public TMP_Text Text;
        public Button SelectableButton;
        public Image FlashLayer;
        public Image Picture;

        [SerializeField] private bool IsGlyph = false;
        [HideInInspector] public bool Selected = false;

        private bool flashing = false;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnButtonClick()
        {
            if (SelectableButton.interactable)
                Selected = true;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SetFlash(bool flash)
        {
            if (flash)
            {
                FlashLayer.SetActive();
                foreach (var tween in FlashLayer.GetComponents<TweenHandler>())
                    tween.Begin();
            }
            else if (flashing)
            {
                FlashLayer.SetInactive();
                foreach (var tween in FlashLayer.GetComponents<TweenHandler>())
                    tween.Cancel();
            }

            flashing = flash;
            Overlay.color = UtilitiesForUI.Instance.OVERLAY_DARK;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnPointerEnter(PointerEventData eventData)
        {
            if (!Selected && IsGlyph && SelectableButton.interactable)
                Overlay.color = UtilitiesForUI.Instance.OVERLAY_TIME;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public virtual void OnPointerExit(PointerEventData eventData)
        {
            if (!Selected && IsGlyph && SelectableButton.interactable)
                Overlay.color = UtilitiesForUI.Instance.OVERLAY_DARK;
        }
    }
}
