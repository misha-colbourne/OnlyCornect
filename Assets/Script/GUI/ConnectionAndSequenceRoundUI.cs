using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OnlyCornect;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class ConnectionAndSequenceRoundUI : MonoBehaviour
    {
        public GameObject BoxesContainer;
        public GameObject CluesAndTimeBoxContainer;
        public TimeBoxUI TimeBox;

        public GameObject AnswerContainer;
        public GameObject AnswerBox;
        public TMP_Text AnswerText;

        public GameObject ClarificationPositioner;

        public GameObject BigPictureContainer;
        public Image BigPicture;
        public Image QuestionMark;

        public List<ClueUI> Clues;
        public List<ClueUI> Clarifications;
        [SerializeField] private List<int> scores;

        [HideInInspector] public bool TimeBarRunning;
        [HideInInspector] public bool ShowingAnswer;
        [HideInInspector] public int ScoreForCurrentQuestion;

        private bool isSequenceRound;
        private List<Question> questions;
        private Question currentQuestion;
        private int currentQuestionIndex;
        private int currentClueIndex;

        public bool IsOutOfCluesForCurrentQuestion { get { return currentClueIndex >= Clues.Count - (isSequenceRound ? 1 : 0); } }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void Init(List<ConnectionQuestion> questions)
        {
            this.questions = new List<Question>(questions);
            this.isSequenceRound = false;
            InitShared();
        }

        public void Init(List<SequenceQuestion> questions)
        {
            this.questions = new List<Question>(questions);
            this.isSequenceRound = true;
            InitShared();
        }

        private void InitShared()
        {
            currentQuestionIndex = 0;
            currentClueIndex = 0;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextQuestion()
        {
            if (currentQuestionIndex > questions.Count)
            {
                throw new Exception("Question index out of bounds");
            }

            currentQuestion = questions[currentQuestionIndex];

            for (int i = 0; i < currentQuestion.Clues.Count; i++)
            {
                var clueUI = Clues[i];
                clueUI.gameObject.SetVisible(false);
                clueUI.FlashLayer.gameObject.SetActive(false);
                clueUI.Text.text = currentQuestion.Clues[i];
                clueUI.Text.gameObject.SetActive(!isSequenceRound || i != currentQuestion.Clues.Count - 1);

                if (currentQuestion.IsPictureQuestion && 
                    UtilitiesForUI.Pictures.ContainsKey(currentQuestion.Clues[i]))
                {
                    clueUI.Picture.gameObject.SetActive(!isSequenceRound || i != currentQuestion.Clues.Count - 1);
                    clueUI.Picture.sprite = UtilitiesForUI.Pictures[currentQuestion.Clues[i]];
                    clueUI.Text.text = currentQuestion.Answers[i];
                    clueUI.Text.gameObject.SetActive(false);
                }
                else
                {
                    clueUI.Picture.gameObject.SetActive(false);
                }

                if (currentQuestion.Answers != null && currentQuestion.Answers.Count > 0)
                {
                    Clarifications[i].Text.text = currentQuestion.Answers[i];
                }
            }

            ClarificationPositioner.SetActive(false);
            AnswerContainer.SetActive(false);
            AnswerText.text = currentQuestion.Connection;

            // Set big pic container to on if picture round
            BigPictureContainer.SetActive(currentQuestion.IsPictureQuestion);
            BigPictureContainer.SetVisible(currentQuestion.IsPictureQuestion);
            QuestionMark.SetInactive();
            QuestionMark.gameObject.SetVisible(true);

            currentQuestionIndex++;
            currentClueIndex = 0;
            ShowingAnswer = false;

            TimeBarRunning = true;
            TimeBox.FillBar.GetComponent<TweenHandler>().Begin(onComplete: StopTimeBar);

            NextClue();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextClue()
        {
            if (!IsOutOfCluesForCurrentQuestion)
            {
                RevealNextClueAnim();
                MoveTimeBoxAlong();
                currentClueIndex++;

                // Show question mark over last clue along with penultimate clue for sequence round
                if (IsOutOfCluesForCurrentQuestion && isSequenceRound && !QuestionMark.gameObject.activeInHierarchy)
                {
                    if (!AnswerContainer.activeInHierarchy)
                        QuestionMark.SetActive();
                    RevealNextClueAnim();
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void RevealNextClueAnim()
        {
            // New clue anim start and position
            var currentClueUI = Clues[currentClueIndex];
            foreach (var tween in currentClueUI.GetComponents<TweenHandler>())
                tween.Begin();
            currentClueUI.gameObject.SetVisible(true);

            if (currentClueUI.Picture.gameObject.activeInHierarchy)
            {
                BigPicture.sprite = currentClueUI.Picture.sprite;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void MoveTimeBoxAlong()
        {
            if (!AnswerContainer.activeInHierarchy)
            {
                // Reposition timebox to new clue
                var clueContainerPos = Clues[currentClueIndex].transform.parent.position;
                TimeBox.transform.position = new Vector3(clueContainerPos.x, TimeBox.transform.position.y, clueContainerPos.z);

                // Animate moving of timebox to new clue
                if (currentClueIndex > 0)
                {
                    TimeBox.gameObject.SetVisible(false);
                    TimeBox.GetComponent<TweenHandler>().Begin();
                }

                ScoreForCurrentQuestion = scores[currentClueIndex];
                TimeBox.Text.text = ScoreForCurrentQuestion + " Point" + (ScoreForCurrentQuestion != 1 ? "s" : "");
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void HandOverToOtherTeam()
        {
            if (!TimeBarRunning)
            {
                while (!IsOutOfCluesForCurrentQuestion)
                    NextClue();

                if (isSequenceRound)
                    MoveTimeBoxAlong();

                TimeBox.FillBar.transform.localPosition = Vector3.zero;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void StopTimeBar()
        {
            TimeBarRunning = false;
            TimeBox.FillBar.GetComponent<TweenHandler>().Cancel();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public IEnumerator ShowAnswer()
        {
            if (!TimeBarRunning && !ShowingAnswer)
            {
                ShowingAnswer = true;

                if (currentQuestion.IsPictureQuestion)
                    yield return ShrinkBigPic();

                AnswerContainer.SetActive(true);
                while (!IsOutOfCluesForCurrentQuestion)
                    NextClue();

                if (isSequenceRound)
                    RevealQuestionMarkClue();

                yield return AnswerReveal();
                ClarifyClues();
            }

            yield return null;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private IEnumerator ShrinkBigPic()
        {
            BigPictureContainer.GetComponent<TweenHandler>().Begin(onComplete: delegate
            {
                BoxesContainer.GetComponent<LayoutElement>().ignoreLayout = true;
                BoxesContainer.GetComponent<TweenHandler>().Begin(onComplete: delegate
                {
                    BigPictureContainer.SetActive(false);
                    BoxesContainer.GetComponent<LayoutElement>().ignoreLayout = false;
                });
            });

            while (BigPictureContainer.LeanIsTweening() || BoxesContainer.LeanIsTweening())
                yield return null;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void RevealQuestionMarkClue()
        {
            var lastClueUI = Clues[Clues.Count - 1];

            if (QuestionMark.gameObject.activeInHierarchy)
            {
                QuestionMark.GetComponent<TweenHandler>().Begin(onComplete: delegate
                {
                    lastClueUI.Text.gameObject.SetActive(true);
                    foreach (var tween in lastClueUI.Text.GetComponents<TweenHandler>())
                        tween.Begin();

                    lastClueUI.Picture.gameObject.SetActive(currentQuestion.IsPictureQuestion);
                    if (currentQuestion.IsPictureQuestion)
                    {
                        foreach (var tween in lastClueUI.Picture.GetComponents<TweenHandler>())
                            tween.Begin();
                    }
                });
            }
            else
            {
                lastClueUI.Text.gameObject.SetActive(true);
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private IEnumerator AnswerReveal()
        {
            Action onCompleteDelegate = delegate
            {
                foreach (var tween in CluesAndTimeBoxContainer.GetComponents<TweenHandler>())
                {
                    tween.Begin();
                }
            };

            bool first = true;
            foreach (var tween in AnswerBox.GetComponents<TweenHandler>())
            {
                tween.Begin(onComplete: first ? onCompleteDelegate : null);
                first = false;
            }

            while (AnswerBox.LeanIsTweening() || CluesAndTimeBoxContainer.LeanIsTweening())
                yield return null;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void ClarifyClues()
        {
            if (currentQuestion.Answers != null && currentQuestion.Answers.Count > 0)
            {
                if (currentQuestion.IsPictureQuestion)
                {
                    foreach (var clue in Clues)
                    {
                        clue.Text.gameObject.SetActive(true);
                        clue.FlashLayer.gameObject.SetActive(true);
                        clue.FlashLayer.gameObject.SetVisible(UtilitiesForUI.ANSWER_OVERLAY_ALPHA);
                        foreach (var tween in clue.FlashLayer.transform.parent.GetComponents<TweenHandler>())
                            tween.Begin();
                    }
                }
                else
                {
                    foreach (var tween in ClarificationPositioner.GetComponents<TweenHandler>())
                        tween.Begin();
                    ClarificationPositioner.SetActive(true);
                }
            }
        }
    }
}
