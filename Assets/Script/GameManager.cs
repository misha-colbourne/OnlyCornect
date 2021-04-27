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

        public class Team
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
        public MissingVowelsRoundUI MissingVowelsRoundScreen;
        public ScorePopupUI ScorePopup;

        [SerializeField] private bool skipTeamNaming;

        private QuizData quizData;
        private EPhase currentPhase;
        private ERound currentRound;

        private Team teamA;
        private Team teamB;
        private Team activeTeam;

        private bool wasHandedOver;
        private bool scoreHasBeenGrantedThisQuestion;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = TARGET_FRAME_RATE;

            quizData = YmlParser.ParseQuiz();
            //UtilitiesForUI.LoadPictures(quizData);

            wasHandedOver = false;
            scoreHasBeenGrantedThisQuestion = false;

            GlyphSelectionScreen.SetInactive();
            RoundNameScreen.SetInactive();
            TeamNameEntryScreen.SetInactive();
            EORTeamScoresScreen.SetInactive();
            ConnectionAndSequencesRoundScreen.SetInactive();
            WallRoundScreen.SetInactive();
            MissingVowelsRoundScreen.SetInactive();

            teamA = new Team();
            teamB = new Team();
            activeTeam = teamA;

            if (skipTeamNaming)
            {
                currentRound = ERound.WallRound;
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
                        TeamNameEntryScreen.SetInactive();
                        TeamNameEntryScreen.SetTeamNames(teamA, teamB);

                        MoveToNextRoundNameScreen();
                    }
                    break;
                case EPhase.RoundNameScreen:
                    {
                        RoundNameScreen.SetInactive();

                        if (currentRound != ERound.MissingVowelsRound)
                        {
                            GlyphSelectionScreen.Init(currentRound == ERound.WallRound);
                            MoveToQuestionSelection();
                        }
                        else
                        {
                            MoveToNextQuestion();
                        }
                    }
                    break;
                case EPhase.GlyphSelection:
                    {
                        GlyphSelectionScreen.SetInactive();
                        MoveToNextQuestion();
                    }
                    break;
                case EPhase.ConnectionQuestion:
                case EPhase.SequencesQuestion:
                    {
                        ConnectionAndSequencesRoundScreen.SetInactive();

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                            MoveToQuestionSelection();
                        else
                            MoveToEORTeamScores();
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        WallRoundScreen.SetInactive();

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                        {
                            WallRoundScreen.Init(quizData.WallRound[1]);
                            MoveToQuestionSelection();
                        }
                        else
                        {
                            MoveToEORTeamScores();
                        }
                    }
                    break;
                case EPhase.MissingVowelsQuestion:
                    {
                        MissingVowelsRoundScreen.SetInactive();
                    }
                    break;
                case EPhase.EORTeamScoresScreen:
                    {
                        EORTeamScoresScreen.SetInactive();
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
            TeamNameEntryScreen.SetActive();
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
                    MissingVowelsRoundScreen.Init(quizData.MissingVowelsRound, teamA, teamB);
                    break;
            }

            RoundNameScreen.SetActive();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToQuestionSelection()
        {
            currentPhase = EPhase.GlyphSelection;
            SwapActiveTeam();

            GlyphSelectionScreen.SetActive();
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

                ConnectionAndSequencesRoundScreen.SetActive();
                ConnectionAndSequencesRoundScreen.NextQuestion();
            }
            else if (currentRound == ERound.WallRound)
            {
                currentPhase = EPhase.WallQuestion;

                WallRoundScreen.SetActive();
                WallRoundScreen.StartTimeBar();
            }
            else if (currentRound == ERound.MissingVowelsRound)
            {
                currentPhase = EPhase.MissingVowelsQuestion;

                MissingVowelsRoundScreen.SetActive();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToEORTeamScores()
        {
            currentPhase = EPhase.EORTeamScoresScreen;
            EORTeamScoresScreen.SetNamesAndScores(teamA, teamB);
            EORTeamScoresScreen.SetActive();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void SwapActiveTeam()
        {
            activeTeam = (activeTeam.Name == teamA.Name) ? teamB : teamA;
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
                            SwapActiveTeam();
                            wasHandedOver = true;
                            ConnectionAndSequencesRoundScreen.HandOverToOtherTeam();
                        }

                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            StartCoroutine(ConnectionAndSequencesRoundScreen.ShowAnswer());
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            if (!scoreHasBeenGrantedThisQuestion && !ConnectionAndSequencesRoundScreen.TimeBarRunning)
                                HandleScoring();
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
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            WallRoundScreen.ResolveWall();
                        }

                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            WallRoundScreen.NextAnswer();
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            WallRoundScreen.AwardPointsForCurrentAnswer();
                        }

                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            if (!scoreHasBeenGrantedThisQuestion)
                            {
                                HandleScoring();
                            }
                            else
                            {
                                WallRoundScreen.StopTimeBar();
                                scoreHasBeenGrantedThisQuestion = false;
                                NextPhase();
                            }
                        }
                    }
                    break;
                case EPhase.MissingVowelsQuestion:
                    {
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            MissingVowelsRoundScreen.Next();
                        }
                    }
                    break;
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
        private void HandleScoring()
        {
            int score = 0;

            switch (currentPhase)
            {
                case EPhase.ConnectionQuestion:
                case EPhase.SequencesQuestion:
                    {
                        score = ConnectionAndSequencesRoundScreen.ScoreForCurrentQuestion;
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        score = WallRoundScreen.Score;
                        if (score == WallRoundUI.ALL_GROUPS_AND_CONNECTIONS_SCORE)
                            score += WallRoundUI.ALL_GROUPS_AND_CONNECTIONS_BONUS;
                    }
                    break;
            }

            ScorePopup.ShowScoreChange(
                activeTeam.Name,
                score,
                activeTeam.Name == teamA.Name
            );

            activeTeam.Score += score;

            scoreHasBeenGrantedThisQuestion = true;
            if (wasHandedOver)
            {
                SwapActiveTeam();
                wasHandedOver = false;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
    }
}
