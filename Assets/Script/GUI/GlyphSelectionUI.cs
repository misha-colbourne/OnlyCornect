using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    public class GlyphSelectionUI : MonoBehaviour
    {
        public List<ClueUI> GlyphBoxes;

        [HideInInspector]
        public bool SelectionMade = false;

        private 

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Update is called once per frame
        void Update()
        {

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
        public void Show()
        {
            gameObject.SetActive(true);

            foreach (var gbox in GlyphBoxes)
            {
                gbox.SelectableButton.interactable = !gbox.Selected;
                gbox.Overlay.color = gbox.Selected ? UtilitiesForUI.Instance.OVERLAY_LIGHT : UtilitiesForUI.Instance.OVERLAY_DARK;
            }

            SelectionMade = false;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Hide()
        {
            gameObject.SetActive(false);

            foreach (var gbox in GlyphBoxes)
            {
                gbox.SetFlash(false);
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
