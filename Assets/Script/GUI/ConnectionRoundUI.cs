using System;
using System.Collections;
using System.Collections.Generic;
using OnlyCornect;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class ConnectionRoundUI : MonoBehaviour
    {
        public TimeBoxUI TimeBox;
        public Image BigPicture;
        [SerializeField] private GameObject BoxesSpacer;

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
            bool isPictureRound = false;

            for (int i = 0; i < question.Clues.Count; i++)
            {
                Clues[i].gameObject.SetVisible(false);
                Clues[i].Text.text = question.Clues[i];
                Clues[i].Text.gameObject.SetActive(true);

                if (question.Pictures != null && i < question.Pictures.Count && UtilitiesForUI.Pictures.ContainsKey(question.Pictures[i])) 
                {
                    Clues[i].Picture.gameObject.SetActive(true);
                    Clues[i].Picture.sprite = UtilitiesForUI.Pictures[question.Pictures[i]];
                    Clues[i].Text.gameObject.SetActive(false);
                    isPictureRound = true;
                }
                else
                {
                    Clues[i].Picture.gameObject.SetActive(false);
                }
            }

            // Set big pic container to on if picture round
            BigPicture.transform.parent.gameObject.SetActive(isPictureRound);
            // Disable spacer for centering if picture round
            BoxesSpacer.gameObject.SetActive(!isPictureRound);

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
                new List<TweenHandler>(TimeBox.FillBar.GetComponents<TweenHandler>()).ForEach(x => x.Begin());
            }

            if (currentClue < Clues.Count)
            {
                // New clue anim start and position
                Clues[currentClue].gameObject.SetVisible(true);
                new List<TweenHandler>(Clues[currentClue].GetComponents<TweenHandler>()).ForEach(x => x.Begin());

                if (Clues[currentClue].Picture.isActiveAndEnabled)
                {
                    BigPicture.sprite = Clues[currentClue].Picture.sprite;
                }

                // Reposition timebox to new clue
                var clueContainerPos = Clues[currentClue].transform.parent.position;
                TimeBox.transform.position = new Vector3 (clueContainerPos.x, TimeBox.transform.position.y, clueContainerPos.z);

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

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void StopTimeBar()
        {
            new List<TweenHandler>(TimeBox.FillBar.GetComponents<TweenHandler>()).ForEach(x => x.Cancel());
        }

    }
}
