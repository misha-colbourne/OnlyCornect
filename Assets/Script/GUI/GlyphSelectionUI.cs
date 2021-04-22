using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    public class GlyphSelectionUI : MonoBehaviour
    {
        [SerializeField] private float DISABLED_ALPHA = 1.0f;
        
        public List<ClueUI> GlyphBoxes;

        [HideInInspector] public bool SelectionMade = false;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            foreach (var gbox in GlyphBoxes)
            {
                if (gbox.SelectableButton != null)
                    gbox.SelectableButton.onClick.AddListener(delegate { GlyphSelected(gbox); });
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnEnable()
        {
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
        public void Init(bool isWallRound)
        {
            for (int i = 0; i < GlyphBoxes.Count; i++)
            {
                var gbox = GlyphBoxes[i];

                if (!isWallRound || i < WallRoundUI.GLYPH_COUNT)
                {
                    gbox.transform.parent.gameObject.SetActive(true);
                    gbox.Selected = false;
                    gbox.SelectableButton.interactable = true;
                    gbox.Overlay.color = gbox.Selected ? UtilitiesForUI.Instance.OVERLAY_LIGHT : UtilitiesForUI.Instance.OVERLAY_DARK;

                    CanvasGroup cg = gbox.GetComponent<CanvasGroup>();
                    cg.alpha = gbox.Selected ? DISABLED_ALPHA : 1;
                }
                else
                {
                    gbox.transform.parent.gameObject.SetActive(false);
                    gbox.Selected = true;
                }
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
