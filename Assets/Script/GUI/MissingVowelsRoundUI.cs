using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

namespace OnlyCornect
{
    public class MissingVowelsRoundUI : MonoBehaviour
    {
        public const int CHARS_PER_SPACE_DELTA = 4;

        public TMP_Text TeamAName;
        public TMP_Text TeamAScore;
        public TMP_Text TeamBName;
        public TMP_Text TeamBScore;

        public GameObject CategoryBox;
        public GameObject ClueBox;
        public TMP_Text CategoryText;
        public TMP_Text ClueText;

        public bool IsOutOfCluesForCurrentQuestion { get { return ShowingAnswer && (currentClue >= questions[currentCategory].Clues.Count - 1); } }

        public bool ShowingAnswer;

        private List<MissingVowelsQuestion> questions;
        private int currentCategory;
        private int currentClue;


        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init(List<MissingVowelsQuestion> questions, GameManager.Team teamA, GameManager.Team teamB)
        {
            TeamAName.text = teamA.Name;
            TeamAScore.text = teamA.Score.ToString();

            TeamBName.text = teamB.Name;
            TeamBScore.text = teamB.Score.ToString();

            this.questions = questions;

            currentCategory = -1;
            currentClue = -1;
            ShowingAnswer = false;

            CategoryText.gameObject.SetVisible(false);
            ClueText.gameObject.SetVisible(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Next()
        {
            if (currentCategory == -1 || IsOutOfCluesForCurrentQuestion)
            {
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

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void NextCategory()
        {
            currentCategory++;
            currentClue = -1;

            CategoryText.text = questions[currentCategory].Connection;

            CategoryText.gameObject.SetVisible(true);
            ClueText.gameObject.SetVisible(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void NextQuestion()
        {
            currentClue++;
            ShowingAnswer = false;

            string clue = questions[currentCategory].Clues[currentClue];
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
                    List<int> possibleSpaceIndices = Enumerable.Range(2, clue.Length - 3).ToList();

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
        private void RevealAnswer()
        {
            ClueText.text = questions[currentCategory].Clues[currentClue].ToUpper();
            ShowingAnswer = true;
        }
    }
}