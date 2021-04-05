using System;
using System.Collections;
using System.Collections.Generic;
using OnlyCornect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class ConnectionRoundUI : MonoBehaviour
    {
        public GameObject CluesAndTimeBoxContainer;
        public TimeBoxUI TimeBox;

        public GameObject BigPictureContainer;
        public Image BigPicture;

        public GameObject AnswerContainer;
        public GameObject AnswerBox;
        public TMP_Text AnswerText;

        public List<ClueUI> Clues;
        [SerializeField] private List<int> Scores;

        public bool IsOutOfCluesForCurrentQuestion { get { return currentClue >= Clues.Count; } }

        private int currentQuestion;
        private int currentClue;
        private bool timeBarRunning;
        private List<ConnectionQuestion> connectionRound;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init(List<ConnectionQuestion> connectionRoundList)
        {
            connectionRound = connectionRoundList;

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

            AnswerContainer.SetActive(false);
            AnswerText.text = question.Connection;

            // Set big pic container to on if picture round
            BigPictureContainer.SetActive(isPictureRound);

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
                timeBarRunning = true;
                foreach (var tween in TimeBox.FillBar.GetComponents<TweenHandler>())
                    tween.Begin();
            }

            if (currentClue < Clues.Count)
            {
                // New clue anim start and position
                Clues[currentClue].gameObject.SetVisible(true);
                foreach (var tween in Clues[currentClue].GetComponents<TweenHandler>())
                    tween.Begin();

                if (Clues[currentClue].Picture.gameObject.activeInHierarchy)
                {
                    BigPicture.sprite = Clues[currentClue].Picture.sprite;
                }

                if (!AnswerContainer.activeInHierarchy)
                {
                    // Reposition timebox to new clue
                    var clueContainerPos = Clues[currentClue].transform.parent.position;
                    TimeBox.transform.position = new Vector3 (clueContainerPos.x, TimeBox.transform.position.y, clueContainerPos.z);

                    // Animate moving of timebox to new clue
                    if (currentClue > 0)
                    {
                        TimeBox.gameObject.SetVisible(false);
                        foreach (var tween in TimeBox.GetComponents<TweenHandler>())
                            tween.Begin();
                    }

                    int score = Scores[currentClue];
                    TimeBox.Text.text = score + " " + (score != 1 ? "Points" : "Point");
                }

                currentClue++;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void StopTimeBar()
        {
            timeBarRunning = false;
            foreach (var tween in TimeBox.FillBar.GetComponents<TweenHandler>())
                tween.Cancel();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void ShowAnswer()
        {
            if (!timeBarRunning)
            {
                AnswerContainer.SetActive(true);
                BigPictureContainer.SetActive(false);

                while (!IsOutOfCluesForCurrentQuestion)
                    NextClue();

                bool first = true;
                foreach (var tween in AnswerBox.GetComponents<TweenHandler>())
                {
                    if (first)
                    {
                        tween.Begin(onComplete: delegate 
                        {
                            foreach (var tween2 in CluesAndTimeBoxContainer.GetComponents<TweenHandler>())
                            {
                                tween2.Begin();
                            }
                        });
                    }
                    else
                    {
                        tween.Begin();
                    }
                }
            }
        }
    }
}
