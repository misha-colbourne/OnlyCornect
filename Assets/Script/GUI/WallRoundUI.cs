using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class WallRoundUI : MonoBehaviour
    {
        public float INCORRECT_GROUP_CLEAR_DELAY;
        public float CLUE_DISABLED_ALPHA;
        public float LIVES_DISABLED_ALPHA;
        public Vector3 GROUP_FOUND_SCALE_DELTA = new Vector3(0.1f, 0.1f, 0.1f);

        [Space]

        public const int GLYPH_COUNT = 2;
        public const int GROUP_COUNT = 4;
        public const int CLUES_PER_GROUP = 4;
        public const int LIVES_COUNT = 3;
        public const int ALL_GROUPS_AND_CONNECTIONS_SCORE = 8;
        public const int ALL_GROUPS_AND_CONNECTIONS_BONUS = 2;

        public GridLayoutGroup ClueGrid;
        public GameObject TimeAndLivesContainer;
        public TimeBoxUI TimeBox;
        public GameObject LivesContainer;
        public List<Image> Lives;
        public GameObject AnswerContainer;
        public TMP_Text AnswerText;
        public GameObject TicksContainer;

        public List<WallClueUI> Clues;
        public List<Sprite> SelectedSprites;
        public List<Color> SelectedOverlays;

        [HideInInspector] public int Score;

        private List<WallQuestion> wallQuestions;
        private int currentGroupIndex;
        private bool timeBarRunning;
        private bool onFinalPair;
        private int livesRemaining;
        private bool ontoConnections;
        private bool awardedPointsForCurrentGroup;

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
            onFinalPair = false;
            ontoConnections = false;
            Score = 0;

            int clueToSet = 0;
            foreach (WallQuestion wallQuestion in wallQuestions)
            {
                foreach (string clue in wallQuestion.Clues)
                {
                    Clues[clueToSet].Text.text = clue;
                    Clues[clueToSet].Connection = wallQuestion.Connection;
                    Clues[clueToSet].ToggleButton.SetIsOnWithoutNotify(false);
                    Clues[clueToSet].ToggleButton.interactable = true;
                    Clues[clueToSet].GroupFound = false;
                    Clues[clueToSet].gameObject.SetVisible(true);
                    Clues[clueToSet].transform.parent.gameObject.SetVisible(true);
                    ResetClueColours(Clues[clueToSet]);
                    clueToSet++;
                }
            }

            List<int> indexList = Enumerable.Range(0, Clues.Count).OrderBy(x => UnityEngine.Random.value).ToList();
            Clues = indexList.Select(x => Clues[x]).ToList();
            for (int i = 0; i < Clues.Count; i++)
            {
                Clues[i].transform.parent.SetSiblingIndex(i);
            }

            TimeAndLivesContainer.SetVisible(true);

            livesRemaining = LIVES_COUNT;
            LivesContainer.SetVisible(LIVES_DISABLED_ALPHA);
            foreach (var life in Lives)
                life.gameObject.SetVisible(true);

            timeBarRunning = false;
            TimeBox.gameObject.SetVisible(true);
            TimeBox.FillBar.transform.localPosition = Vector3.zero;
            TimeBox.Text.Hide();

            AnswerContainer.SetActive(false);
            foreach (var tick in TicksContainer.GetComponentsInChildren<Image>())
                tick.gameObject.SetVisible(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void StartTimeBar()
        {
            timeBarRunning = true;
            var fillTween = TimeBox.FillBar.GetComponent<TweenHandler>();
            fillTween.From = new Vector3(-TimeBox.GetComponent<LayoutElement>().preferredWidth, 0, 0);
            fillTween.Begin(onComplete: delegate { StopTimeBar(); });
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void StopTimeBar(bool outOfLives = false)
        {
            if (!timeBarRunning)
                return;

            timeBarRunning = false;
            var fillTween = TimeBox.FillBar.GetComponent<TweenHandler>();

            if (outOfLives)
            {
                fillTween.Cancel();
                TimeBox.Text.text = "Out of lives!";
                TimeBox.Text.Show();
            }
            else if (TimeBox.FillBar.LeanIsTweening())
            {
                fillTween.Cancel();
            }
            else
            {
                TimeBox.Text.text = "Time is up!";
                TimeBox.Text.Show();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void OnClueClicked(WallClueUI clue)
        {
            if (!timeBarRunning)
                return;

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
                if (selectedClues.Count >= CLUES_PER_GROUP)
                {
                    // If all clues have the same connection, thus a valid group
                    if (selectedClues.All(x => x.Connection == clue.Connection))
                    {
                        MarkAsCorrectGroup(selectedClues);
                        Score++;

                        // Ensure ordering of selected and remaining clues matches hierarchy
                        selectedClues.OrderBy(x => x.transform.GetSiblingIndex());
                        List<WallClueUI> remainingClues = Clues.Where(x => !x.GroupFound)
                                                                 .OrderBy(x => x.transform.GetSiblingIndex())
                                                                 .ToList();

                        MoveCluesToNewPositions(selectedClues, remainingClues);

                        // Activate final pair's three lives remaining
                        if (remainingClues.Count == CLUES_PER_GROUP * 2)
                        {
                            onFinalPair = true;
                            foreach (var tween in LivesContainer.GetComponents<TweenHandler>())
                                tween.Begin();
                        }
                        // Also fill second group when correct first of final pair entered
                        else if (remainingClues.Count == CLUES_PER_GROUP)
                        {
                            StopTimeBar();
                            MarkAsCorrectGroup(remainingClues);
                            Score++;

                            ontoConnections = true;
                            currentGroupIndex = 0;
                        }
                    }
                    else
                    {
                        // Clear invalid selected group
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
        private void MarkAsCorrectGroup(IEnumerable<WallClueUI> clues)
        {
            // Mark as found and lock in group colours
            foreach (var clue in clues)
            {
                clue.Text.color = Color.white;
                clue.GetComponent<Image>().sprite = SelectedSprites[currentGroupIndex];
                clue.Overlay.color = SelectedOverlays[currentGroupIndex];

                clue.GroupFound = true;
                clue.ToggleButton.SetIsOnWithoutNotify(false);
                clue.ToggleButton.interactable = false;

                SpriteState ss = clue.ToggleButton.spriteState;
                ss.disabledSprite = SelectedSprites[currentGroupIndex];
                clue.ToggleButton.spriteState = ss;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void MoveCluesToNewPositions(List<WallClueUI> selectedClues, List<WallClueUI> remainingClues)
        {
            // Set new clue container sibling indices to have group move to the top, store From position for move tween
            int switchoverIndex = selectedClues.Count + remainingClues.Count - CLUES_PER_GROUP;
            for (int i = 0; i < selectedClues.Count + remainingClues.Count; i++)
            {
                if (i < switchoverIndex)
                {
                    remainingClues[i].tweenMoveOnCorrectGroupFound.From = remainingClues[i].transform.position;
                    remainingClues[i].transform.parent.SetSiblingIndex(i);
                }
                else
                {
                    selectedClues[i - switchoverIndex].tweenMoveOnCorrectGroupFound.From = selectedClues[i - switchoverIndex].transform.position;
                    selectedClues[i - switchoverIndex].transform.parent.SetSiblingIndex(i);
                }
            }

            // Force clue grid layout to update new positions pre-tween
            ClueGrid.ForceGridUpdates();

            // Assignment of To position for move tween and start tween anim
            foreach (var clueToMove in selectedClues.Concat(remainingClues))
            {
                clueToMove.tweenMoveOnCorrectGroupFound.To = clueToMove.transform.position;
                clueToMove.transform.position = clueToMove.tweenMoveOnCorrectGroupFound.From;
                clueToMove.tweenMoveOnCorrectGroupFound.Begin();
            }

            currentGroupIndex++;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void ResolveWall()
        {
            if (ontoConnections)
                return;

            if (timeBarRunning)
                StopTimeBar();

            foreach (var tween in TimeAndLivesContainer.GetComponents<TweenHandler>())
                tween.Begin();

            List<WallClueUI> ungroupedClues = Clues.Where(x => !x.GroupFound).ToList();

            if (ungroupedClues.Count > 0)
            {
                List<WallClueUI> remainingClues = new List<WallClueUI>();

                // Reverse order in which groups are coloured due to backwards order of grid hierarchy
                currentGroupIndex = GROUP_COUNT - 1;
                foreach (string connection in ungroupedClues.Select(x => x.Connection).Distinct())
                {
                    var group = ungroupedClues.Where(x => x.Connection == connection);
                    MarkAsCorrectGroup(group);
                    remainingClues.AddRange(group);

                    currentGroupIndex--;
                }

                for (int i = 0; i < remainingClues.Count; i++)
                {
                    remainingClues[i].tweenMoveOnCorrectGroupFound.From = remainingClues[i].transform.position;
                    remainingClues[i].transform.parent.SetSiblingIndex(i);
                }

                ClueGrid.ForceGridUpdates();

                foreach (var clueToMove in remainingClues)
                {
                    clueToMove.tweenMoveOnCorrectGroupFound.To = clueToMove.transform.position;
                    clueToMove.transform.position = clueToMove.tweenMoveOnCorrectGroupFound.From;
                    clueToMove.tweenMoveOnCorrectGroupFound.Begin();
                }
            }

            ontoConnections = true;
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

            if (onFinalPair)
            {
                Lives[LIVES_COUNT - livesRemaining].gameObject.SetVisible(false);
                livesRemaining--;

                if (livesRemaining == 0)
                    StopTimeBar(outOfLives: true);
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

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextAnswer()
        {
            if (!ontoConnections)
                return;

            if (!AnswerContainer.activeInHierarchy)
            {
                TimeAndLivesContainer.SetVisible(true);
                TimeBox.gameObject.SetVisible(false);
                LivesContainer.gameObject.SetVisible(false);

                Clues = GetComponentsInChildren<WallClueUI>().ToList();

                currentGroupIndex = GROUP_COUNT - 1;
                string firstConnection = Clues[currentGroupIndex * CLUES_PER_GROUP].Connection;

                AnswerContainer.SetActive(true);
                foreach (var tween in AnswerContainer.GetComponents<TweenHandler>())
                    tween.Begin(onComplete: delegate {
                        foreach (var clue in Clues.Where(x => x.Connection != firstConnection))
                        {
                            var clueFadeTween = clue.transform.parent.GetComponent<TweenHandler>();
                            clueFadeTween.To.x = CLUE_DISABLED_ALPHA;
                            clueFadeTween.Begin();
                        }
                    });

                AnswerText.text = firstConnection;
                AnswerText.gameObject.SetActive(false);
                AnswerText.gameObject.SetVisible(false);
            }
            else
            {
                if (!AnswerText.gameObject.activeInHierarchy)
                {
                    AnswerText.gameObject.SetActive(true);
                    AnswerText.GetComponent<TweenHandler>().Begin();

                    if (currentGroupIndex == 0)
                        ontoConnections = false;
                }
                else
                {
                    currentGroupIndex--;
                    string currentConnection = Clues[currentGroupIndex * CLUES_PER_GROUP].Connection;

                    foreach (var clue in Clues)
                    {
                        var clueFadeTween = clue.transform.parent.GetComponent<TweenHandler>();

                        if (clue.Connection == currentConnection)
                            clueFadeTween.To.x = 1;
                        else
                            clueFadeTween.To.x = CLUE_DISABLED_ALPHA;

                        clueFadeTween.Begin();
                    }

                    AnswerText.GetComponent<TweenHandler>().Begin(reverse: true, onComplete: delegate {
                        AnswerText.text = currentConnection;
                        AnswerText.gameObject.SetActive(false);
                    });

                    awardedPointsForCurrentGroup = false;
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void AwardPointsForCurrentAnswer()
        {
            if (!awardedPointsForCurrentGroup && ontoConnections)
            {
                Score++;
                awardedPointsForCurrentGroup = true;
                int clueIndexInverted = GROUP_COUNT - currentGroupIndex - 1;
                TicksContainer.transform.GetChild(clueIndexInverted).gameObject.SetVisible(true);
            }
        }
    }
}
