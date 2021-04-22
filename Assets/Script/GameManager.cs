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
        public ConnectionAndSequenceRoundUI ConnectionAndSequencesRoundScreen;
        public WallRoundUI WallRoundScreen;

        public ScorePopupUI ScorePopup;

        [SerializeField] private bool skipTeamNaming;

        private QuizData quizData;
        private EPhase currentPhase;
        private ERound currentRound;

        private Team teamA;
        private Team teamB;
        private bool activeTeamIsA;
        private bool scoreHasBeenGrantedThisQuestion;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = TARGET_FRAME_RATE;
            quizData = YmlParser.ParseQuiz();

            GlyphSelectionScreen.Hide();
            RoundNameScreen.Hide();
            TeamNameEntryScreen.Hide();
            EORTeamScoresScreen.Hide();
            ConnectionAndSequencesRoundScreen.Hide();
            WallRoundScreen.Hide();

            //UtilitiesForUI.LoadPictures(quizData);

            if (skipTeamNaming)
            {
                currentRound = ERound.SequencesRound;
                teamA.Name = "Team A";
                teamB.Name = "Team B";
                StartCoroutine(Utilities.WaitAFrameThenRun(MoveToNextRoundNameScreen));
            }
            else
            {
                StartCoroutine(Utilities.WaitAFrameThenRun(MoveToTeamNameEntry));
            }
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
                        TeamNameEntryScreen.SetTeamNames(teamA, teamB);
                        MoveToNextRoundNameScreen();
                    }
                    break;
                case EPhase.RoundNameScreen:
                    {
                        RoundNameScreen.Hide();
                        GlyphSelectionScreen.Init(currentRound == ERound.WallRound);

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
                case EPhase.SequencesQuestion:
                    {
                        ConnectionAndSequencesRoundScreen.Hide();

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                            MoveToQuestionSelection();
                        else
                            MoveToEORTeamScores();
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        WallRoundScreen.Hide();
                    }
                    break;
                case EPhase.MissingVowelsQuestion:
                    {
                    }
                    break;
                case EPhase.EORTeamScoresScreen:
                    {
                        EORTeamScoresScreen.Hide();
                        MoveToNextRoundNameScreen();
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
        public void MoveToNextRoundNameScreen()
        {
            currentPhase = EPhase.RoundNameScreen;
            RoundNameScreen.NextRoundNameText(currentRound);
            currentRound++;

            switch (currentRound)
            {
                case ERound.ConnectionRound:
                    ConnectionAndSequencesRoundScreen.Init(quizData.ConnectionRound);
                    break;
                case ERound.SequencesRound:
                    ConnectionAndSequencesRoundScreen.Init(quizData.SequencesRound);
                    break;
                case ERound.WallRound:
                    WallRoundScreen.Init(quizData.WallRound[0]);
                    break;
                case ERound.MissingVowelsRound:
                    break;
            }

            RoundNameScreen.Show();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToQuestionSelection()
        {
            currentPhase = EPhase.GlyphSelection;
            activeTeamIsA = !activeTeamIsA;

            GlyphSelectionScreen.Show();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToNextQuestion()
        {
            if (currentRound == ERound.ConnectionRound || currentRound == ERound.SequencesRound)
            {
                if (currentRound == ERound.ConnectionRound)
                    currentPhase = EPhase.ConnectionQuestion;
                else if (currentRound == ERound.SequencesRound)
                    currentPhase = EPhase.SequencesQuestion;

                ConnectionAndSequencesRoundScreen.Show();
                ConnectionAndSequencesRoundScreen.NextQuestion();
            }
            else if (currentRound == ERound.WallRound)
            {
                currentPhase = EPhase.WallQuestion;

                WallRoundScreen.Show();
                WallRoundScreen.StartTimeBar();
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
                case EPhase.SequencesQuestion:
                    {
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            ConnectionAndSequencesRoundScreen.NextClue();
                        }

                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            ConnectionAndSequencesRoundScreen.StopTimeBar();
                        }

                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            ConnectionAndSequencesRoundScreen.HandOverToOtherTeam();
                        }

                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            StartCoroutine(ConnectionAndSequencesRoundScreen.ShowAnswer());
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            if (!scoreHasBeenGrantedThisQuestion && !ConnectionAndSequencesRoundScreen.TimeBarRunning)
                            {
                                ScorePopup.ShowScoreChange(
                                    activeTeamIsA ? teamA.Name : teamB.Name,
                                    ConnectionAndSequencesRoundScreen.ScoreForCurrentQuestion,
                                    activeTeamIsA
                                );
                                scoreHasBeenGrantedThisQuestion = true;
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            ConnectionAndSequencesRoundScreen.StopTimeBar();
                            scoreHasBeenGrantedThisQuestion = false;
                            NextPhase();
                        }
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                    }
                    break;
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
