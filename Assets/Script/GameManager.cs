using OnlyCornect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace OnlyCornect
{
    public class GameManager : MonoBehaviour
    {
        public static class InputKeys
        {
            public const KeyCode TeamNameEntry_SwitchInputFocus = KeyCode.Tab;

            public const KeyCode GlyphSelection_SelectGlyph = KeyCode.Mouse0;

            public const KeyCode ConnectionAndSequences_NextClue = KeyCode.RightArrow;
            public const KeyCode ConnectionAndSequences_StopTimeBar = KeyCode.Space;
            public const KeyCode ConnectionAndSequences_HandOver = KeyCode.UpArrow;
            public const KeyCode ConnectionAndSequences_ShowAnswer = KeyCode.A;
            public const KeyCode ConnectionAndSequences_HandleScoring = KeyCode.P;
            public const KeyCode ConnectionAndSequences_NextPhase = KeyCode.BackQuote;

            public const KeyCode WallQuestion_ResolveWall = KeyCode.A;
            public const KeyCode WallQuestion_NextAnswer = KeyCode.RightArrow;
            public const KeyCode WallQuestion_AwardPoints = KeyCode.P;
            public const KeyCode WallQuestion_NextPhase = KeyCode.BackQuote;

            public const KeyCode MissingVowelsQuestion_Next = KeyCode.RightArrow;

            public const KeyCode Default_NextPhase = KeyCode.Mouse0;
            public const KeyCode Default_Quit = KeyCode.Escape;

            public const KeyCode Scores_TeamA_MinusOne = KeyCode.Minus;
            public const KeyCode Scores_TeamA_PlusOne = KeyCode.Equals;
            public const KeyCode Scores_TeamB_MinusOne = KeyCode.LeftBracket;
            public const KeyCode Scores_TeamB_PlusOne = KeyCode.RightBracket;
        }

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
            public int TotalScore;
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
        public ScoreOverlayUI ScoreOverlay;
        public NetworkManager NetworkManager;

        [SerializeField] private bool skipTeamNaming;

        private QuizData quizData;
        private EPhase currentPhase;
        private ERound currentRound;

        private Team teamA = new Team();
        private Team teamB = new Team();
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
            UtilitiesForUI.LoadPictures(quizData);

            wasHandedOver = false;
            scoreHasBeenGrantedThisQuestion = false;
            finished = false;

            bool activeTeamIsA = UnityEngine.Random.Range(0, 2) > 0;
            activeTeam = activeTeamIsA ? teamA : teamB;
            ScoreOverlay.SetActiveTeam(activeTeamIsA);

            GlyphSelectionScreen.SetActive(false);
            RoundNameScreen.SetActive(false);
            TeamNameEntry.SetActive(false);
            EORTeamScores.SetActive(false);
            ConnectionAndSequencesRound.SetActive(false);
            WallRound.SetActive(false);
            MissingVowelsRound.SetActive(false);
            ScoreOverlay.SetActive(false);

            if (skipTeamNaming)
            {
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
                        TeamNameEntry.SetActive(false);
                        TeamNameEntry.SetTeamNames(teamA, teamB);
                        ScoreOverlay.SetTeamNames(teamA.Name, teamB.Name);

                        MoveToNextRoundNameScreen();
                    }
                    break;
                case EPhase.RoundNameScreen:
                    {
                        RoundNameScreen.SetActive(false);
                        ScoreOverlay.SetActive(true);

                        if (currentRound != ERound.MissingVowelsRound)
                        {
                            GlyphSelectionScreen.Init(currentRound == ERound.WallRound);
                            MoveToQuestionSelection();
                        }
                        else
                        {
                            ScoreOverlay.SetBothTeamsActive();
                            MoveToNextQuestion();
                        }
                    }
                    break;
                case EPhase.GlyphSelection:
                    {
                        GlyphSelectionScreen.SetActive(false);
                        MoveToNextQuestion();
                    }
                    break;
                case EPhase.ConnectionQuestion:
                case EPhase.SequencesQuestion:
                    {
                        ConnectionAndSequencesRound.SetActive(false);

                        if (GlyphSelectionScreen.GlyphBoxes.Any(x => !x.Selected))
                            MoveToQuestionSelection();
                        else
                            MoveToEORTeamScores();
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        WallRound.SetActive(false);

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
                        MissingVowelsRound.SetActive(false);
                        finished = true;
                        MoveToEORTeamScores();
                    }
                    break;
                case EPhase.EORTeamScoresScreen:
                    {
                        if (!finished)
                        {
                            EORTeamScores.SetActive(false);
                            ScoreOverlay.SetActive(true);
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
            TeamNameEntry.SetActive(true);
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

            RoundNameScreen.SetActive(true);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToQuestionSelection()
        {
            currentPhase = EPhase.GlyphSelection;
            SwapActiveTeam();

            GlyphSelectionScreen.SetActive(true);
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

                ConnectionAndSequencesRound.SetActive(true);
                ConnectionAndSequencesRound.NextQuestion();
            }
            else if (currentRound == ERound.WallRound)
            {
                currentPhase = EPhase.WallQuestion;

                WallRound.SetActive(true);
                WallRound.StartTimeBar();
            }
            else if (currentRound == ERound.MissingVowelsRound)
            {
                currentPhase = EPhase.MissingVowelsQuestion;

                MissingVowelsRound.SetActive(true);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void MoveToEORTeamScores()
        {
            currentPhase = EPhase.EORTeamScoresScreen;
            EORTeamScores.SetNamesAndScores(teamA, teamB, finished);
            EORTeamScores.SetActive(true);
            ScoreOverlay.SetActive(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void SwapActiveTeam()
        {
            bool activeTeamIsA = ReferenceEquals(activeTeam, teamA);
            activeTeam = activeTeamIsA ? teamB : teamA; // Note this is swapping them around
            ScoreOverlay.SetActiveTeam(!activeTeamIsA);
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
                        if (Input.GetKeyDown(InputKeys.TeamNameEntry_SwitchInputFocus))
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
                        if (Input.GetKeyDown(InputKeys.GlyphSelection_SelectGlyph))
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
                        if (Input.GetKeyDown(InputKeys.ConnectionAndSequences_NextClue))
                        {
                            ConnectionAndSequencesRound.NextClue();
                        }

                        if (Input.GetKeyDown(InputKeys.ConnectionAndSequences_StopTimeBar))
                        {
                            ConnectionAndSequencesRound.StopTimeBar();
                        }

                        if (Input.GetKeyDown(InputKeys.ConnectionAndSequences_HandOver))
                        {
                            if (!wasHandedOver && !ConnectionAndSequencesRound.ShowingAnswer)
                            {
                                SwapActiveTeam();
                                wasHandedOver = true;
                                ConnectionAndSequencesRound.HandOverToOtherTeam();
                            }
                        }

                        if (Input.GetKeyDown(InputKeys.ConnectionAndSequences_ShowAnswer))
                        {
                            StartCoroutine(ConnectionAndSequencesRound.ShowAnswer());
                        }

                        if (Input.GetKeyDown(InputKeys.ConnectionAndSequences_HandleScoring))
                        {
                            HandleScoring();
                        }

                        if (Input.GetKeyDown(InputKeys.ConnectionAndSequences_NextPhase))
                        {
                            if (wasHandedOver)
                            {
                                SwapActiveTeam();
                                wasHandedOver = false;
                            }

                            ConnectionAndSequencesRound.StopTimeBar();
                            scoreHasBeenGrantedThisQuestion = false;
                            NextPhase();
                        }
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        if (Input.GetKeyDown(InputKeys.WallQuestion_ResolveWall))
                        {
                            if (!scoreHasBeenGrantedThisQuestion)
                                WallRound.ResolveWall();
                        }

                        if (Input.GetKeyDown(InputKeys.WallQuestion_NextAnswer))
                        {
                            WallRound.NextAnswer();
                        }

                        if (Input.GetKeyDown(InputKeys.WallQuestion_AwardPoints))
                        {
                            WallRound.AwardPointsForCurrentAnswer();
                        }

                        if (Input.GetKeyDown(InputKeys.WallQuestion_NextPhase))
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
                        if (Input.GetKeyDown(InputKeys.MissingVowelsQuestion_Next))
                        {
                            if (!MissingVowelsRound.OutOfQuestions)
                            {
                                MissingVowelsRound.Next();
                            }
                            else
                            {
                                if (teamA.TotalScore == teamB.TotalScore || (MissingVowelsRound.ShowingTiebreaker && !MissingVowelsRound.ShowingAnswer))
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
                        if (Input.GetKeyDown(InputKeys.Default_NextPhase))
                        {
                            NextPhase();
                        }

                        if (Input.GetKeyDown(InputKeys.Default_Quit))
                        {
                            Application.Quit();
                        }
                    }
                    break;
            }

            if (Input.GetKeyDown(InputKeys.Scores_TeamA_MinusOne) || Input.GetKeyDown(InputKeys.Scores_TeamA_PlusOne) ||
                Input.GetKeyDown(InputKeys.Scores_TeamB_MinusOne) || Input.GetKeyDown(InputKeys.Scores_TeamB_PlusOne))
            {
                if (Input.GetKeyDown(InputKeys.Scores_TeamA_MinusOne))
                {
                    teamA.TotalScore--;
                    ScoreOverlay.UpdateScore(-1, teamA.TotalScore, true);
                }
                if (Input.GetKeyDown(InputKeys.Scores_TeamA_PlusOne))
                {
                    teamA.TotalScore++;
                    ScoreOverlay.UpdateScore(1, teamA.TotalScore, true);
                }
                if (Input.GetKeyDown(InputKeys.Scores_TeamB_MinusOne))
                {
                    teamB.TotalScore--;
                    ScoreOverlay.UpdateScore(-1, teamB.TotalScore, false);
                }
                if (Input.GetKeyDown(InputKeys.Scores_TeamB_PlusOne))
                {
                    teamB.TotalScore++;
                    ScoreOverlay.UpdateScore(1, teamB.TotalScore, false);
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void HandleScoring()
        {
            if (scoreHasBeenGrantedThisQuestion || ConnectionAndSequencesRound.TimeBarRunning)
                return;

            int questionScore = 0;

            switch (currentPhase)
            {
                case EPhase.ConnectionQuestion:
                case EPhase.SequencesQuestion:
                    {
                        questionScore = ConnectionAndSequencesRound.ScoreForCurrentQuestion;
                    }
                    break;
                case EPhase.WallQuestion:
                    {
                        questionScore = WallRound.Score;
                        if (questionScore == WallRoundUI.ALL_GROUPS_AND_CONNECTIONS_SCORE)
                            questionScore += WallRoundUI.ALL_GROUPS_AND_CONNECTIONS_BONUS;
                    }
                    break;
            }

            activeTeam.TotalScore += questionScore;
            ScoreOverlay.UpdateScore(questionScore, activeTeam.TotalScore, ReferenceEquals(activeTeam, teamA));

            scoreHasBeenGrantedThisQuestion = true;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
    }
}
