using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    public class GlyphSelectionUI : MonoBehaviour
    {
        [SerializeField] private float DISABLED_ALPHA = 0.6f;
        public List<ClueUI> GlyphBoxes;
        [HideInInspector] public bool SelectionMade = false;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnEnable()
        {
            foreach (var gbox in GlyphBoxes)
            {
                gbox.SelectableButton.interactable = !gbox.Selected;
                gbox.Overlay.color = gbox.Selected ? UtilitiesForUI.Instance.OVERLAY_LIGHT : UtilitiesForUI.Instance.OVERLAY_DARK;

                CanvasGroup cg = gbox.GetComponent<CanvasGroup>();
                cg.alpha = gbox.Selected ? DISABLED_ALPHA : 1;
            }

            SelectionMade = false;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnDisable()
        {
            foreach (var gbox in GlyphBoxes)
            {
                gbox.SetFlash(false);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init()
        {
            foreach (var gbox in GlyphBoxes)
            {
                if (gbox.SelectableButton != null)
                    gbox.SelectableButton.onClick.AddListener(delegate { GlyphSelected(gbox); });
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void GlyphSelected(ClueUI gbox)
        {
            SelectionMade = true;
            gbox.SetFlash(true);
        }
    }
}
