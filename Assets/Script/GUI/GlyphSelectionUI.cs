using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    public class GlyphSelectionUI : MonoBehaviour
    {
        public List<ClueUI> GlyphBoxes;

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
            
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Show()
        {
            gameObject.SetActive(true);

            foreach (var gbox in GlyphBoxes)
                gbox.gameObject.SetVisible(true);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Hide()
        {
            gameObject.SetActive(false);

            foreach (var gbox in GlyphBoxes)
                gbox.gameObject.SetVisible(false);
        }
    }
}
