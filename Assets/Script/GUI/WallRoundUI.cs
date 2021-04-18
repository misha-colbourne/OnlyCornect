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
        public float INCORRECT_GROUP_CLEAR_DELAY;

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
                    Clues[clueToSet].Connection = wallQuestion.Connection;
                    Clues[clueToSet].SelectableButton.interactable = true;
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
            clue.Text.color = Color.white;
            clue.GetComponent<Image>().sprite = SelectedSprites[currentGroupIndex];
            clue.Overlay.color = SelectedOverlays[currentGroupIndex];

            SpriteState ss = clue.SelectableButton.spriteState;
            ss.highlightedSprite = SelectedSprites[currentGroupIndex];
            clue.SelectableButton.spriteState = ss;

            clue.GetComponents<TweenHandler>()[0].Begin();

            List<WallClueUI> selectedClues = Clues.Where(x => x.Selected).ToList();
            if (selectedClues.Count >= 4)
            {
                if (selectedClues.All(x => x.Connection == clue.Connection))
                {
                    GroupFound(selectedClues);
                }
                else
                {
                    StartCoroutine(ResetClues());
                }
            }
        }

        private void GroupFound(List<WallClueUI> selectedClues)
        {
            foreach (var selectedClue in selectedClues)
            {
                selectedClue.GroupFound = true;
                selectedClue.Selected = false;
                selectedClue.SelectableButton.interactable = false;

                SpriteState ss = selectedClue.SelectableButton.spriteState;
                ss.disabledSprite = SelectedSprites[currentGroupIndex];
                selectedClue.SelectableButton.spriteState = ss;
            }

            currentGroupIndex++;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private IEnumerator ResetClues()
        {
            yield return new WaitForSeconds(INCORRECT_GROUP_CLEAR_DELAY);

            foreach (WallClueUI clue in Clues.Where(x => x.Selected))
            {
                clue.Selected = false;
                clue.Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;

                clue.GetComponent<Image>().sprite = UtilitiesForUI.Instance.BOX_LIGHT;
                clue.Overlay.color = UtilitiesForUI.Instance.OVERLAY_LIGHT;

                SpriteState ss = clue.SelectableButton.spriteState; 
                ss.highlightedSprite = UtilitiesForUI.Instance.BOX_SELECTED;
                clue.SelectableButton.spriteState = ss;

                clue.GetComponents<TweenHandler>()[1].Begin();
            }
            
            yield return null;
        }
    }
}
