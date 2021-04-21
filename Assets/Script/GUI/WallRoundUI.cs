using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class WallRoundUI : MonoBehaviour
    {
        public float INCORRECT_GROUP_CLEAR_DELAY = 0.33f;
        public float LIVES_DISABLED_ALPHA = 0.2f;
        public Vector3 GROUP_FOUND_SCALE_DELTA = new Vector3(0.1f, 0.1f, 0.1f);

        [Space]

        [HideInInspector] public int GROUP_SIZE = 4;

        public GridLayoutGroup ClueGrid;
        public TimeBoxUI TimeBox;
        public List<Image> Lives;

        public List<WallClueUI> Clues;
        public List<Sprite> SelectedSprites;
        public List<Color> SelectedOverlays;

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
                    Clues[clueToSet].GroupFound = false;
                    clueToSet++;
                }
            }

            List<int> indexList = Enumerable.Range(0, Clues.Count).OrderBy(x => UnityEngine.Random.value).ToList();
            Clues = indexList.Select(x => Clues[x]).ToList();
            for (int i = 0; i < Clues.Count; i++)
            {
                Clues[i].transform.parent.SetSiblingIndex(i);
                Clues[i].Text.color = UtilitiesForUI.Instance.TEXT_NORMAL_COLOUR;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnClueClicked(WallClueUI clue)
        {
            clue.tweenShrinkOnClick.Begin();

            if (clue.ToggleButton.isOn)
            {
                // Toggle on
                clue.Text.color = Color.white;
                clue.GetComponent<Image>().sprite = SelectedSprites[currentGroupIndex];
                clue.Overlay.color = SelectedOverlays[currentGroupIndex];

                SpriteState clickedClueSS = clue.ToggleButton.spriteState;
                clickedClueSS.highlightedSprite = SelectedSprites[currentGroupIndex];
                clickedClueSS.pressedSprite = SelectedSprites[currentGroupIndex];
                clue.ToggleButton.spriteState = clickedClueSS;

                // Selected group of four
                List<WallClueUI> selectedClues = Clues.Where(x => x.ToggleButton.isOn).ToList();
                if (selectedClues.Count >= GROUP_SIZE)
                {
                    // If all clues have the same connection, thus a valid group
                    if (selectedClues.All(x => x.Connection == clue.Connection))
                    {
                        // Mark as found and lock in group colours
                        foreach (var selectedClue in selectedClues)
                        {
                            selectedClue.GroupFound = true;
                            selectedClue.ToggleButton.SetIsOnWithoutNotify(false);
                            selectedClue.ToggleButton.interactable = false;

                            SpriteState ss = selectedClue.ToggleButton.spriteState;
                            ss.disabledSprite = SelectedSprites[currentGroupIndex];
                            selectedClue.ToggleButton.spriteState = ss;
                        }

                        // Ensure ordering of selected and remaining clues matches hierarchy
                        selectedClues.OrderBy(x => x.transform.GetSiblingIndex());
                        List<WallClueUI> remainingClues = Clues.Where(x => !x.GroupFound)
                                                                 .OrderBy(x => x.transform.GetSiblingIndex())
                                                                 .ToList();

                        // Set new clue container sibling indices to have group move to the top, store From position for move tween
                        int switchoverIndex = selectedClues.Count + remainingClues.Count - GROUP_SIZE;
                        for (int i = 0; i < selectedClues.Count + remainingClues.Count; i++)
                        {
                            if (i < switchoverIndex)
                            {
                                remainingClues[i].tweenMoveOnCorrectGroupFound.From = remainingClues[i].transform.position;
                                remainingClues[i].transform.parent.SetSiblingIndex(i);
                                //remainingClues[i].tweenScaleOnCorrectGroupFound.To = Vector3.one - GROUP_FOUND_SCALE_DELTA;
                            }
                            else
                            {
                                selectedClues[i - switchoverIndex].tweenMoveOnCorrectGroupFound.From = selectedClues[i - switchoverIndex].transform.position;
                                selectedClues[i - switchoverIndex].transform.parent.SetSiblingIndex(i);
                                //selectedClues[i - switchoverIndex].tweenScaleOnCorrectGroupFound.To = Vector3.one;
                            }
                        }

                        // Force clue grid layout to update new positions pre-tween
                        ClueGrid.CalculateLayoutInputHorizontal();
                        ClueGrid.CalculateLayoutInputVertical();
                        ClueGrid.SetLayoutHorizontal();
                        ClueGrid.SetLayoutVertical();

                        // Assignment of To position for move tween and start tween anim
                        foreach (var clueToMove in selectedClues.Concat(remainingClues))
                        {
                            clueToMove.tweenMoveOnCorrectGroupFound.To = clueToMove.transform.position;
                            clueToMove.transform.position = clueToMove.tweenMoveOnCorrectGroupFound.From;
                            clueToMove.tweenMoveOnCorrectGroupFound.Begin();
                            //clueToMove.tweenScaleOnCorrectGroupFound.Begin();
                        }

                        currentGroupIndex++;
                    }
                    else
                    {
                        // Clear invalid selection group
                        StartCoroutine(ResetClues());
                    }
                }
            }
            else
            {
                // Deselecting a clue so clear selected colours
                ResetClueColours(clue);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private IEnumerator ResetClues()
        {
            yield return new WaitForSeconds(INCORRECT_GROUP_CLEAR_DELAY);

            foreach (WallClueUI clue in Clues.Where(x => x.ToggleButton.isOn))
            {
                clue.ToggleButton.SetIsOnWithoutNotify(false);
                ResetClueColours(clue);
                clue.tweenShakeOnIncorrectGroup.Begin();
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
