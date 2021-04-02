using System;
using System.Collections;
using System.Collections.Generic;
using OnlyCornect;
using UnityEngine;

namespace OnlyCornect
{
    public class ConnectionRoundUI : MonoBehaviour
    {
        public TimeBoxUI TimeBox;
        [SerializeField] private Vector3 TimeBoxSpacing;
        public List<ClueUI> Clues;
        [SerializeField] private List<int> Scores;

        public bool IsOutOfCluesForCurrentQuestion { get { return currentClue >= Clues.Count; } }


        private int currentQuestion;
        private int currentClue;
        private List<ConnectionQuestion> connectionRound;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init(List<ConnectionQuestion> connectionRoundList)
        {
            this.connectionRound = connectionRoundList;

            currentQuestion = 0;
            currentClue = 0;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Show()
        {
            gameObject.SetActive(true);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Hide()
        {
            gameObject.SetActive(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextQuestion()
        {
            ConnectionQuestion question = connectionRound[currentQuestion];
            for (int i = 0; i < question.Clues.Count; i++)
            {
                Clues[i].Text.text = question.Clues[i];
                Clues[i].gameObject.SetVisible(false);
            }

            currentQuestion++;
            currentClue = 0;

            NextClue();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextClue()
        {
            // Start time bar ticking down
            if (currentClue == 0)
            {
                TimeBox.gameObject.SetVisible(true);
                new List<TweenHandler>(TimeBox.FillBar.GetComponents<TweenHandler>()).ForEach(x => x.Begin());
            }

            if (currentClue < Clues.Count)
            {
                // New clue anim start and position
                Clues[currentClue].gameObject.SetVisible(true);
                new List<TweenHandler>(Clues[currentClue].GetComponents<TweenHandler>()).ForEach(x => x.Begin());

                // Reposition timebox to new clue
                var clueContainerPos = Clues[currentClue].transform.parent.position;
                TimeBox.transform.position = clueContainerPos + TimeBoxSpacing;

                // Animate moving of timebox to new clue 
                if (currentClue > 0)
                {
                    TimeBox.gameObject.SetVisible(false);
                    new List<TweenHandler>(TimeBox.GetComponents<TweenHandler>()).ForEach(x => x.Begin());
                }

                int score = Scores[currentClue];
                TimeBox.Text.text = score + " " + (score != 1 ? "Points" : "Point");

                currentClue++;
            }
        }

    }
}
