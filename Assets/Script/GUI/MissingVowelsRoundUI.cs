using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace OnlyCornect
{
    public class MissingVowelsRoundUI : MonoBehaviour
    {
        public const int CHARS_PER_SPACE_DELTA = 5;
        public const string TIEBREAKER_CATEGORY_TEXT = "Tiebreaker";

        public GameObject CategoryBox;
        public GameObject ClueBox;
        public TMP_Text CategoryText;
        public TMP_Text ClueText;

        public bool IsOutOfCluesForCurrentQuestion { get { return ShowingAnswer && (currentClue >= questions[currentCategory].Clues.Count - 1); } }

        [HideInInspector] public bool ShowingAnswer;
        [HideInInspector] public bool OutOfQuestions;
        [HideInInspector] public bool ShowingTiebreaker;

        private List<MissingVowelsQuestion> questions;
        private MissingVowelsQuestion tiebreaker;
        private int currentCategory;
        private int currentClue;


        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init(List<MissingVowelsQuestion> questions, GameManager.Team teamA, GameManager.Team teamB)
        {
            this.questions = questions.Where(x => !x.Tiebreaker).ToList();
            tiebreaker = questions.FirstOrDefault(x => x.Tiebreaker);

            currentCategory = -1;
            currentClue = -1;
            ShowingAnswer = false;
            OutOfQuestions = false;

            CategoryBox.SetVisible(false);
            ClueBox.SetVisible(false);
            CategoryText.gameObject.SetVisible(false);
            ClueText.gameObject.SetVisible(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Next(bool showTiebreaker = false)
        {
            if (!CategoryBox.IsVisible())
            {
                foreach (var tween in CategoryBox.GetComponents<TweenHandler>())
                    tween.Begin();
                return;
            }
            
            if (CategoryText.gameObject.IsVisible() && !ClueBox.gameObject.IsVisible())
            {
                foreach (var tween in ClueBox.GetComponents<TweenHandler>())
                    tween.Begin();
                return;
            }

            if (showTiebreaker)
            {
                if (!ShowingTiebreaker)
                {
                    NextCategory(TIEBREAKER_CATEGORY_TEXT);
                    NextQuestion(tiebreaker.Clues[0]);
                    ShowingTiebreaker = true;
                }
                else
                    RevealAnswer(tiebreaker.Clues[0]);
            }
            else
            {
                if (currentCategory == -1 || IsOutOfCluesForCurrentQuestion)
                {
                    if (currentCategory + 1 >= questions.Count)
                        OutOfQuestions = true;
                    else
                        NextCategory();
                }
                else if (currentClue == -1 || ShowingAnswer)
                {
                    NextQuestion();
                }
                else
                {
                    RevealAnswer();
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void NextCategory(string categoryOverride = null)
        {
            currentCategory++;
            currentClue = -1;

            CategoryText.text = categoryOverride ?? questions[currentCategory].Connection;

            CategoryText.gameObject.SetVisible(true);
            ClueText.gameObject.SetVisible(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void NextQuestion(string clueOverride = null)
        {
            currentClue++;
            ShowingAnswer = false;

            string clue = clueOverride ?? questions[currentCategory].Clues[currentClue];
            clue = clue.ToUpper();
            clue = Regex.Replace(clue, "[^A-Z]+", "");
            clue = Regex.Replace(clue, "[AEIOU]+", "");

            if (clue.Length > 3)
            {
                int roughSpaceCount = clue.Length / CHARS_PER_SPACE_DELTA;
                int min = System.Math.Max(1, roughSpaceCount);
                int max = min + (CHARS_PER_SPACE_DELTA / 2);
                int numberOfSpacesToAdd = Random.Range(min, max);

                for (int i = 0; i < numberOfSpacesToAdd; i++)
                {
                    List<int> possibleSpaceIndices = Enumerable.Range(1, clue.Length - 2).ToList();

                    possibleSpaceIndices = possibleSpaceIndices.Where(x =>
                        (x - 2 < 0 || clue[x - 2] != ' ') &&
                        (x - 1 < 0 || clue[x - 1] != ' ') &&
                        clue[x] != ' ' &&
                        (x + 1 > clue.Length - 1 || clue[x + 1] != ' ') &&
                        (x + 2 > clue.Length - 1 || clue[x + 2] != ' ')
                    ).ToList();

                    if (possibleSpaceIndices.Count > 0)
                    {
                        int possibleSpaceIndex = possibleSpaceIndices[Random.Range(0, possibleSpaceIndices.Count)];
                        clue = clue.Insert(possibleSpaceIndex, " ");
                    }
                    else
                    {
                        break;
                    }
                }
            }

            ClueText.gameObject.SetVisible(true);
            ClueText.text = clue;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void RevealAnswer(string overrideAnswer = null)
        {
            ClueText.text = overrideAnswer ?? questions[currentCategory].Clues[currentClue];
            ClueText.text = ClueText.text.ToUpper();
            ShowingAnswer = true;
        }
    }
}