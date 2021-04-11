using System;
using System.Collections;
using System.Collections.Generic;
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

        public GameObject BigPictureContainer;
        public Image BigPicture;
        public Image QuestionMark;

        public List<ClueUI> Clues;
        [SerializeField] private List<int> scores;

        [HideInInspector] public bool TimeBarRunning;
        [HideInInspector] public int ScoreForCurrentQuestion;

        public bool IsOutOfCluesForCurrentQuestion { get { return currentClue >= Clues.Count - (isSequenceRound ? 1 : 0); } }

        private List<Question> questions;
        private bool isSequenceRound;
        private bool isPictureQuestion;
        private int currentQuestion;
        private int currentClue;
        private bool showingAnswer;

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
            currentQuestion = 0;
            currentClue = 0;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextQuestion()
        {
            Question question = questions[currentQuestion];
            isPictureQuestion = false;

            for (int i = 0; i < question.Clues.Count; i++)
            {
                Clues[i].gameObject.SetVisible(false);
                Clues[i].FlashLayer.gameObject.SetActive(false);
                Clues[i].Text.text = question.Clues[i];
                Clues[i].Text.gameObject.SetActive(!isSequenceRound || i != question.Clues.Count - 1);

                if (question is PictureQuestion picQuestion && 
                    picQuestion.Pictures != null && 
                    i < picQuestion.Pictures.Count && 
                    UtilitiesForUI.Pictures.ContainsKey(picQuestion.Pictures[i]))
                {
                    Clues[i].Picture.gameObject.SetActive(!isSequenceRound || i != question.Clues.Count - 1);
                    Clues[i].Picture.sprite = UtilitiesForUI.Pictures[picQuestion.Pictures[i]];
                    Clues[i].Text.gameObject.SetActive(false);
                    isPictureQuestion = true;
                }
                else
                {
                    Clues[i].Picture.gameObject.SetActive(false);
                }
            }

            AnswerContainer.SetActive(false);
            AnswerText.text = question.Connection;

            // Set big pic container to on if picture round
            BigPictureContainer.SetActive(isPictureQuestion);
            BigPictureContainer.SetVisible(isPictureQuestion);
            QuestionMark.Hide();
            QuestionMark.gameObject.SetVisible(true);

            currentQuestion++;
            currentClue = 0;
            showingAnswer = false;

            TimeBarRunning = true;
            TimeBox.FillBar.GetComponent<TweenHandler>().Begin(StopTimeBar);

            NextClue();
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextClue()
        {
            if (!IsOutOfCluesForCurrentQuestion && !showingAnswer)
            {
                RevealNextClueAnim();
                MoveTimeBoxAlong();
                currentClue++;

                // Show question mark over last clue along with penultimate clue for sequence round
                if (IsOutOfCluesForCurrentQuestion && isSequenceRound && !QuestionMark.gameObject.activeInHierarchy)
                {
                    if (!AnswerContainer.activeInHierarchy)
                        QuestionMark.Show();
                    RevealNextClueAnim();
                }
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void RevealNextClueAnim()
        {
            // New clue anim start and position
            Clues[currentClue].gameObject.SetVisible(true);
            foreach (var tween in Clues[currentClue].GetComponents<TweenHandler>())
                tween.Begin();

            if (Clues[currentClue].Picture.gameObject.activeInHierarchy)
            {
                BigPicture.sprite = Clues[currentClue].Picture.sprite;
            }
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void MoveTimeBoxAlong()
        {
            if (!AnswerContainer.activeInHierarchy)
            {
                // Reposition timebox to new clue
                var clueContainerPos = Clues[currentClue].transform.parent.position;
                TimeBox.transform.position = new Vector3(clueContainerPos.x, TimeBox.transform.position.y, clueContainerPos.z);

                // Animate moving of timebox to new clue
                if (currentClue > 0)
                {
                    TimeBox.gameObject.SetVisible(false);
                    TimeBox.GetComponent<TweenHandler>().Begin();
                }

                ScoreForCurrentQuestion = scores[currentClue];
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
            if (!TimeBarRunning)
            {
                if (isPictureQuestion)
                    yield return ShrinkBigPic();

                AnswerContainer.SetActive(true);
                while (!IsOutOfCluesForCurrentQuestion)
                    NextClue();

                showingAnswer = true;

                if (isSequenceRound)
                {
                    if (QuestionMark.gameObject.activeInHierarchy)
                    {
                        QuestionMark.GetComponent<TweenHandler>().Begin(onComplete: delegate
                        {
                            Clues[Clues.Count - 1].Text.gameObject.SetActive(true);
                            foreach (var tween in Clues[Clues.Count - 1].Text.GetComponents<TweenHandler>())
                                tween.Begin();

                            if (isPictureQuestion)
                            {
                                Clues[Clues.Count - 1].Picture.gameObject.SetActive(true);
                                foreach (var tween in Clues[Clues.Count - 1].Picture.GetComponents<TweenHandler>())
                                    tween.Begin();
                            }
                        });
                    }
                    else
                    {
                        Clues[Clues.Count - 1].Text.gameObject.SetActive(true);
                    }
                }

                yield return AnswerReveal();

                if (isPictureQuestion)
                {
                    foreach (var clue in Clues)
                    {
                        clue.Text.gameObject.SetActive(true);
                        clue.FlashLayer.gameObject.SetActive(true);
                        clue.FlashLayer.gameObject.SetVisible(UtilitiesForUI.PICTURE_ANSWER_OVERLAY_ALPHA);
                        clue.FlashLayer.transform.parent.GetComponent<TweenHandler>().Begin();
                    }
                }
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
        private IEnumerator AnswerReveal()
        {
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
                first = false;
            }

            while (AnswerBox.LeanIsTweening() || CluesAndTimeBoxContainer.LeanIsTweening())
                yield return null;
        }
    }
}
