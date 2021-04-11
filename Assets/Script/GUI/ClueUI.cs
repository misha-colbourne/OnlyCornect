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
        // Start is called before the first frame update
        void Start()
        {
            if (SelectableButton != null)
            {
                SelectableButton.onClick.AddListener(OnButtonClick);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnButtonClick()
        {
            Selected = true;
            Overlay.color = UtilitiesForUI.Instance.OVERLAY_DARK;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SetFlash(bool flash)
        {
            if (flash)
            {
                FlashLayer.gameObject.SetActive(true);
                foreach (var tween in FlashLayer.GetComponents<TweenHandler>())
                    tween.Begin();
            }
            else if (flashing)
            {
                FlashLayer.gameObject.SetActive(false);
                foreach (var tween in FlashLayer.GetComponents<TweenHandler>())
                    tween.Cancel();
            }

            flashing = flash;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!Selected && IsGlyph)
                Overlay.color = UtilitiesForUI.Instance.OVERLAY_TIME;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnPointerExit(PointerEventData eventData)
        {
            if (!Selected && IsGlyph)
                Overlay.color = UtilitiesForUI.Instance.OVERLAY_DARK;
        }
    }
}
