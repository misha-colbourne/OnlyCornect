using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OnlyCornect
{
    public class GameManager : MonoBehaviour
    {
        public enum EPhase
        {
            MainMenu,
            QuestionSelection,
            ConnectionRound,
            SequencesRound,
            WallRound,
            MissingVowelsRound
        }

        public ConnectionRoundUI ConnectionRound;
        public GlyphSelectionUI GlyphSelection;

        private QuizData quiz;
        private EPhase currentPhase = EPhase.MainMenu;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = 60;
            quiz = YmlParser.ParseQuiz();

            GlyphSelection.Init();
            ConnectionRound.Init(quiz.ConnectionRound);

            UtilitiesForUI.LoadPictures(quiz);

            StartCoroutine(Utilities.WaitAFrameThenRun(MoveToQuestionSelection));
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Update is called once per frame
        void Update()
        {
            HandleInput();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToQuestionSelection()
        {
            currentPhase = EPhase.QuestionSelection;

            GlyphSelection.Show();
            ConnectionRound.Hide();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextQuestion()
        {
            currentPhase = EPhase.ConnectionRound;

            ConnectionRound.Show();
            GlyphSelection.Hide();

            ConnectionRound.NextQuestion();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void HandleInput()
        {
            if (currentPhase == EPhase.QuestionSelection)
            {
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    if (GlyphSelection.SelectionMade)
                    {
                        GlyphSelection.SelectionMade = false;
                        NextQuestion();
                    }
                }
            }

            if (currentPhase == EPhase.ConnectionRound)
            {
                if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    if (!ConnectionRound.IsOutOfCluesForCurrentQuestion)
                        ConnectionRound.NextClue();
                }

                if (Input.GetKeyDown(KeyCode.Backspace))
                {
                    ConnectionRound.StopTimeBar();
                    MoveToQuestionSelection();
                }

                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ConnectionRound.StopTimeBar();
                }

                if (Input.GetKeyDown(KeyCode.A))
                {
                    ConnectionRound.ShowAnswer();
                }
            }
        }
    }
}
