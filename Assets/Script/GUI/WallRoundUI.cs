using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class WallRoundUI : MonoBehaviour
    {
        [SerializeField] private List<WallClueUI> Clues;
        [SerializeField] private List<Sprite> SelectedSprites;
        [SerializeField] private List<Color> SelectedOverlays;

        private List<WallQuestion> wallQuestions;
        private int currentGroupIndex;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            foreach (var clue in Clues)
            {
                clue.SelectableButton.onClick.AddListener(delegate { OnClueClicked(clue); });
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init(List<WallQuestion> wallQuestions)
        {
            this.wallQuestions = wallQuestions;
            currentGroupIndex = 0;

            int clueToSet = 0;
            foreach (WallQuestion wallQuestion in wallQuestions)
            {
                foreach (string clue in wallQuestion.Clues)
                {
                    Clues[clueToSet].Text.text = clue;
                    clueToSet++;
                }
            }

            foreach (WallClueUI clue in Clues)
            {
                int randomIndex = UnityEngine.Random.Range(0, Clues.Count - 1);
                clue.transform.parent.SetSiblingIndex(randomIndex);
                clue.Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnClueClicked(WallClueUI clue)
        {
            clue.GetComponent<Image>().sprite = SelectedSprites[currentGroupIndex];
            clue.Overlay.color = SelectedOverlays[currentGroupIndex];

            SpriteState ss = clue.SelectableButton.spriteState;
            ss.highlightedSprite = SelectedSprites[currentGroupIndex];
            ss.pressedSprite = SelectedSprites[currentGroupIndex];
            clue.SelectableButton.spriteState = ss;

            if (Clues.Count(x => x.Selected) >= 4)
                ResetClues();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void ResetClues()
        {
            foreach (WallClueUI clue in Clues)
            {
                clue.Selected = false;
                clue.Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;

                clue.GetComponent<Image>().sprite = UtilitiesForUI.Instance.BOX_LIGHT;
                clue.Overlay.color = UtilitiesForUI.Instance.OVERLAY_LIGHT;

                SpriteState ss = clue.SelectableButton.spriteState; 
                ss.highlightedSprite = UtilitiesForUI.Instance.BOX_SELECTED;
                clue.SelectableButton.spriteState = ss;
            }

            currentGroupIndex++;
            currentGroupIndex %= 4;
        }
    }
}
