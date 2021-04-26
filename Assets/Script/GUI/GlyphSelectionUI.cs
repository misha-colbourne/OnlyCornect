using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class GlyphSelectionUI : MonoBehaviour
    {
        public const float ALREADY_CHOSEN_ALPHA = 0.8f;
        
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
            foreach (var gbox in GlyphBoxes)
            {
                if (gbox.Selected)
                {
                    gbox.SelectableButton.interactable = false;
                    gbox.Overlay.color = UtilitiesForUI.Instance.OVERLAY_LIGHT;
                    SpriteState ss = gbox.SelectableButton.spriteState;
                    ss.disabledSprite = UtilitiesForUI.Instance.BOX_LIGHT;
                    gbox.SelectableButton.spriteState = ss;

                    CanvasGroup cg = gbox.GetComponent<CanvasGroup>();
                    cg.alpha = gbox.Selected ? ALREADY_CHOSEN_ALPHA : 1;
                }
                else
                {
                    gbox.SelectableButton.interactable = true;
                }
            }
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
                    gbox.Overlay.color = UtilitiesForUI.Instance.OVERLAY_DARK;
                    SpriteState ss = gbox.SelectableButton.spriteState;
                    ss.disabledSprite = UtilitiesForUI.Instance.BOX_DARK;
                    gbox.SelectableButton.spriteState = ss;

                    CanvasGroup cg = gbox.GetComponent<CanvasGroup>();
                    cg.alpha = 1;
                }
                else
                {
                    gbox.transform.parent.gameObject.SetActive(false);
                    gbox.Selected = true;
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void GlyphSelected(ClueUI selectedGbox)
        {
            if (!SelectionMade)
            {
                SelectionMade = true;
                selectedGbox.SetFlash(true);

                foreach (var gbox in GlyphBoxes)
                    gbox.SelectableButton.interactable = false;
            }
        }
    }
}
