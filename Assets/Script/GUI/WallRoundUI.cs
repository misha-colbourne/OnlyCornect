using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyCornect
{
    public class WallRoundUI : MonoBehaviour
    {
        [SerializeField] private List<ClueUI> Clues;

        private List<WallQuestion> wallQuestions;

        private void Awake()
        {
            foreach (var clue in Clues)
            {
                clue.SelectableButton.onClick.AddListener(delegate { OnClueClicked(clue); });
            }
        }

        public void Init(List<WallQuestion> wallQuestions)
        {
            this.wallQuestions = wallQuestions;

            int clueToSet = 0;
            foreach (WallQuestion wallQuestion in wallQuestions)
            {
                foreach (string clue in wallQuestion.Clues)
                {
                    Clues[clueToSet].Text.text = clue;
                    clueToSet++;
                }
            }

            foreach (ClueUI clue in Clues)
            {
                int randomIndex = UnityEngine.Random.Range(0, Clues.Count - 1);
                clue.transform.parent.SetSiblingIndex(randomIndex);
                clue.Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;
            }
        }

        private void OnClueClicked(ClueUI clue)
        {
            clue.SelectableButton.interactable = false;
            clue.Text.color = Color.white;

            ResetClues();
        }

        private void ResetClues()
        {
            if (Clues.Count(x => x.SelectableButton.interactable == false) >= 4)
            {
                foreach (ClueUI clue in Clues)
                {
                    clue.SelectableButton.interactable = true;
                    clue.Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;
                }
            }
        }
    }
}
