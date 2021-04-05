using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace OnlyCornect
{
    public class GameManager : MonoBehaviour
    {
        public enum EPhase
        {
            None,
            MainMenu,
            TeamNameEntry,
            RoundNameScreen,
            GlyphSelection,
            ConnectionQuestion,
            SequencesQuestion,
            WallQuestion,
            MissingVowelsQuestion,
            EORTeamScoresScreen
        }

        public enum ERound
        {
            None,
            ConnectionRound,
            SequencesRound,
            WallRound,
            MissingVowelsRound
        }

        public struct Team
        {
            public string Name;
            public int Score;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private const int TARGET_FRAME_RATE = 120;

        public GlyphSelectionUI GlyphSelectionScreen;
        public RoundNameScreenUI RoundNameScreen;
        public TeamNameEntryUI TeamNameEntryScreen;
        public EORTeamScoresUI EORTeamScoresScreen;
        public ConnectionRoundUI ConnectionRoundScreen;

        private QuizData quiz;
        private EPhase currentPhase;
        private ERound currentRound;

        private Team teamA;
        private Team teamB;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = TARGET_FRAME_RATE;
            quiz = YmlParser.ParseQuiz();

            GlyphSelectionScreen.Init();
            ConnectionRoundScreen.Init(quiz.ConnectionRound);

            GlyphSelectionScreen.Hide();
            RoundNameScreen.Hide();
            TeamNameEntryScreen.Hide();
            ConnectionRoundScreen.Hide();
            EORTeamScoresScreen.Hide();

            UtilitiesForUI.LoadPictures(quiz);

            StartCoroutine(Utilities.WaitAFrameThenRun(MoveToTeamNameEntry));
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Update is called once per frame
        void Update()
        {
            HandleInput();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextPhase()
        {
            switch (currentPhase)
            {
                case EPhase.MainMenu:
                    {
                    }
                    break;
                case EPhase.TeamNameEntry:
                    {
                        TeamNameEntryScreen.Hide();
                        MoveToRoundNameScreen();
                    }
                    break;
                case EPhase.RoundNameScreen:
                    {
                        currentRound++;
                        RoundNameScreen.Hide();
                        MoveToQuestionSelection();
                    }
                    break;
                case EPhase.GlyphSelection:
                    {
                        GlyphSelectionScreen.Hide();
                        MoveToNextQuestion();
                    }
                    break;
                case EPhase.ConnectionQuestion:
                    {
                        ConnectionRoundScreen.Hide();

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                            MoveToQuestionSelection();
                        else
                            MoveToEORTeamScores();
                    }
                    break;
                case EPhase.SequencesQuestion:
                    {
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                    }
                    break;
                case EPhase.MissingVowelsQuestion:
                    {
                    }
                    break;
                case EPhase.EORTeamScoresScreen:
                    {
                        EORTeamScoresScreen.Hide();
                        MoveToRoundNameScreen();
                    }
                    break;
                default:
                    {
                        Debug.LogError("Unexpected phase: " + currentPhase);
                    }
                    break;
            }
        }
        
        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToTeamNameEntry()
        {
            currentPhase = EPhase.TeamNameEntry;
            TeamNameEntryScreen.Show();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToRoundNameScreen()
        {
            currentPhase = EPhase.RoundNameScreen;
            RoundNameScreen.Show();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToQuestionSelection()
        {
            currentPhase = EPhase.GlyphSelection;
            GlyphSelectionScreen.Show();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToNextQuestion()
        {
            if (currentRound == ERound.ConnectionRound)
            {
                currentPhase = EPhase.ConnectionQuestion;
                ConnectionRoundScreen.Show();
                ConnectionRoundScreen.NextQuestion();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToEORTeamScores()
        {
            currentPhase = EPhase.EORTeamScoresScreen;
            EORTeamScoresScreen.SetNamesAndScores(teamA, teamB);
            EORTeamScoresScreen.Show();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void HandleInput()
        {
            switch (currentPhase)
            {
                //case EPhase.MainMenu:
                //    {
                //    }
                //    break;
                case EPhase.TeamNameEntry:
                    {
                        if (Input.GetKeyDown(KeyCode.Tab))
                        {
                            TeamNameEntryScreen.SwitchInputFocus();
                        }
                    }
                    break;
                //case EPhase.RoundNameScreen:
                //    {
                //    }
                //    break;
                case EPhase.GlyphSelection:
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            if (GlyphSelectionScreen.SelectionMade)
                            {
                                GlyphSelectionScreen.SelectionMade = false;
                                NextPhase();
                            }
                        }
                    }
                    break;
                case EPhase.ConnectionQuestion:
                    {
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            if (!ConnectionRoundScreen.IsOutOfCluesForCurrentQuestion)
                                ConnectionRoundScreen.NextClue();
                        }

                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            ConnectionRoundScreen.StopTimeBar();
                        }

                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            StartCoroutine(ConnectionRoundScreen.ShowAnswer());
                        }

                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            ConnectionRoundScreen.StopTimeBar();
                            NextPhase();
                        }
                    }
                    break;
                //case EPhase.SequencesQuestion:
                //    {
                //    }
                //    break;
                //case EPhase.WallQuestion:
                //    {
                //    }
                //    break;
                //case EPhase.MissingVowelsQuestion:
                //    {
                //    }
                //    break;
                //case EPhase.EORTeamScoresScreen:
                //    {
                //    }
                //    break;
                default:
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0))
                        {
                            NextPhase();
                        }
                    }
                    break;
            }

        }

        // --------------------------------------------------------------------------------------------------------------------------------------
    }
}
