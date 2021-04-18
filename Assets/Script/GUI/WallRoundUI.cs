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
                clue.ToggleButton.onValueChanged.AddListener(delegate { OnClueClicked(clue); });
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
                    Clues[clueToSet].ToggleButton.SetIsOnWithoutNotify(false);
                    Clues[clueToSet].ToggleButton.interactable = true;
                    Clues[clueToSet].Connection = wallQuestion.Connection;
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
            clue.GetComponents<TweenHandler>()[0].Begin();

            if (clue.ToggleButton.isOn)
            {
                // Toggle on
                clue.Text.color = Color.white;
                clue.GetComponent<Image>().sprite = SelectedSprites[currentGroupIndex];
                clue.Overlay.color = SelectedOverlays[currentGroupIndex];

                SpriteState ss = clue.ToggleButton.spriteState;
                ss.highlightedSprite = SelectedSprites[currentGroupIndex];
                ss.pressedSprite = SelectedSprites[currentGroupIndex];
                clue.ToggleButton.spriteState = ss;

                List<WallClueUI> selectedClues = Clues.Where(x => x.ToggleButton.isOn).ToList();
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
            else
            {
                // Toggle off
                ResetClueColours(clue);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void GroupFound(List<WallClueUI> selectedClues)
        {
            foreach (var selectedClue in selectedClues)
            {
                selectedClue.GroupFound = true;
                selectedClue.ToggleButton.SetIsOnWithoutNotify(false);
                selectedClue.ToggleButton.interactable = false;

                SpriteState ss = selectedClue.ToggleButton.spriteState;
                ss.disabledSprite = SelectedSprites[currentGroupIndex];
                selectedClue.ToggleButton.spriteState = ss;
            }

            currentGroupIndex++;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private IEnumerator ResetClues()
        {
            yield return new WaitForSeconds(INCORRECT_GROUP_CLEAR_DELAY);

            foreach (WallClueUI clue in Clues.Where(x => x.ToggleButton.isOn))
            {
                clue.ToggleButton.SetIsOnWithoutNotify(false);
                ResetClueColours(clue);
                clue.GetComponents<TweenHandler>()[1].Begin();
            }
            
            yield return null;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void ResetClueColours(WallClueUI clue)
        {
            clue.GetComponent<Image>().sprite = UtilitiesForUI.Instance.BOX_LIGHT;
            clue.Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;
            clue.Overlay.color = UtilitiesForUI.Instance.OVERLAY_LIGHT;

            SpriteState ss = clue.ToggleButton.spriteState;
            ss.highlightedSprite = UtilitiesForUI.Instance.BOX_SELECTED;
            ss.pressedSprite = UtilitiesForUI.Instance.BOX_SELECTED;
            clue.ToggleButton.spriteState = ss;
        }
    }
}
