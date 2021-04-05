using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class ClueUI : MonoBehaviour
    {
        public Image Overlay;
        public TMP_Text Text;
        public Button SelectableButton;
        public Image FlashLayer;
        public Image Picture;

        [HideInInspector]
        public bool Selected = false;

        private bool flashing = false;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            if (SelectableButton != null)
                SelectableButton.onClick.AddListener(OnButtonClick);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Update is called once per frame
        void Update()
        {

        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void OnButtonClick()
        {
            Selected = true;
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
    }

}
