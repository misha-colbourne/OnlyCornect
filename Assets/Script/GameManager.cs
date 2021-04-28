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

        public TeamNameEntryUI TeamNameEntry;
        public RoundNameScreenUI RoundNameScreen;
        public GlyphSelectionUI GlyphSelectionScreen;
        public EORTeamScoresUI EORTeamScores;
        public ConnectionAndSequenceRoundUI ConnectionAndSequencesRound;
        public WallRoundUI WallRound;
        public MissingVowelsRoundUI MissingVowelsRound;
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
        private bool finished;

        // --------------------------------------------------------------------------------------------------------------------------------------
        // Start is called before the first frame update
        void Start()
        {
            Application.targetFrameRate = TARGET_FRAME_RATE;

            quizData = YmlParser.ParseQuiz();
            //UtilitiesForUI.LoadPictures(quizData);

            wasHandedOver = false;
            scoreHasBeenGrantedThisQuestion = false;
            finished = false;

            teamA = new Team();
            teamB = new Team();
            activeTeam = teamA;

            GlyphSelectionScreen.SetInactive();
            RoundNameScreen.SetInactive();
            TeamNameEntry.SetInactive();
            EORTeamScores.SetInactive();
            ConnectionAndSequencesRound.SetInactive();
            WallRound.SetInactive();
            MissingVowelsRound.SetInactive();

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
                        TeamNameEntry.SetInactive();
                        TeamNameEntry.SetTeamNames(teamA, teamB);

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
                        ConnectionAndSequencesRound.SetInactive();

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                            MoveToQuestionSelection();
                        else
                            MoveToEORTeamScores();
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        WallRound.SetInactive();

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                        {
                            WallRound.Init(quizData.WallRound[1]);
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
                        MissingVowelsRound.SetInactive();
                        finished = true;
                        MoveToEORTeamScores();
                    }
                    break;
                case EPhase.EORTeamScoresScreen:
                    {
                        if (!finished)
                        {
                            EORTeamScores.SetInactive();
                            MoveToNextRoundNameScreen();
                        }
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
            TeamNameEntry.SetActive();
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
                    ConnectionAndSequencesRound.Init(quizData.ConnectionRound);
                    break;
                case ERound.SequencesRound:
                    ConnectionAndSequencesRound.Init(quizData.SequencesRound);
                    break;
                case ERound.WallRound:
                    WallRound.Init(quizData.WallRound[0]);
                    break;
                case ERound.MissingVowelsRound:
                    MissingVowelsRound.Init(quizData.MissingVowelsRound, teamA, teamB);
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

                ConnectionAndSequencesRound.SetActive();
                ConnectionAndSequencesRound.NextQuestion();
            }
            else if (currentRound == ERound.WallRound)
            {
                currentPhase = EPhase.WallQuestion;

                WallRound.SetActive();
                WallRound.StartTimeBar();
            }
            else if (currentRound == ERound.MissingVowelsRound)
            {
                currentPhase = EPhase.MissingVowelsQuestion;

                MissingVowelsRound.SetActive();
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToEORTeamScores()
        {
            currentPhase = EPhase.EORTeamScoresScreen;
            EORTeamScores.SetNamesAndScores(teamA, teamB, finished);
            EORTeamScores.SetActive();
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
                            TeamNameEntry.SwitchInputFocus();
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
                            ConnectionAndSequencesRound.NextClue();
                        }

                        if (Input.GetKeyDown(KeyCode.Space))
                        {
                            ConnectionAndSequencesRound.StopTimeBar();
                        }

                        if (Input.GetKeyDown(KeyCode.UpArrow))
                        {
                            SwapActiveTeam();
                            wasHandedOver = true;
                            ConnectionAndSequencesRound.HandOverToOtherTeam();
                        }

                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            StartCoroutine(ConnectionAndSequencesRound.ShowAnswer());
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            if (!scoreHasBeenGrantedThisQuestion && !ConnectionAndSequencesRound.TimeBarRunning)
                                HandleScoring();
                        }

                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            ConnectionAndSequencesRound.StopTimeBar();
                            scoreHasBeenGrantedThisQuestion = false;
                            NextPhase();
                        }
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        if (Input.GetKeyDown(KeyCode.A))
                        {
                            WallRound.ResolveWall();
                        }

                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            WallRound.NextAnswer();
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            WallRound.AwardPointsForCurrentAnswer();
                        }

                        if (Input.GetKeyDown(KeyCode.Backspace))
                        {
                            if (!scoreHasBeenGrantedThisQuestion)
                            {
                                HandleScoring();
                            }
                            else
                            {
                                WallRound.StopTimeBar();
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
                            if (!MissingVowelsRound.OutOfQuestions)
                            {
                                MissingVowelsRound.Next();
                            }
                            else
                            {
                                if (teamA.Score == teamB.Score)
                                {
                                    MissingVowelsRound.Next(showTiebreaker: true);
                                }
                                else
                                {
                                    NextPhase();
                                }
                            }
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

                        if (Input.GetKeyDown(KeyCode.Escape))
                        {
                            Application.Quit();
                        }
                    }
                    break;
            }

            if (Input.GetKeyDown(KeyCode.Minus) || Input.GetKeyDown(KeyCode.Equals) ||
                Input.GetKeyDown(KeyCode.LeftBracket) || Input.GetKeyDown(KeyCode.RightBracket))
            {
                if (Input.GetKeyDown(KeyCode.Minus))
                {
                    teamA.Score--;
                }
                if (Input.GetKeyDown(KeyCode.Equals))
                {
                    teamA.Score++;
                }
                if (Input.GetKeyDown(KeyCode.LeftBracket))
                {
                    teamB.Score--;
                }
                if (Input.GetKeyDown(KeyCode.RightBracket))
                {
                    teamB.Score++;
                }

                if (currentRound == ERound.MissingVowelsRound)
                    MissingVowelsRound.SetTeamScores(teamA, teamB);
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
                        score = ConnectionAndSequencesRound.ScoreForCurrentQuestion;
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        score = WallRound.Score;
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
