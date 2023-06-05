using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreOverlayUI : MonoBehaviour
{
    [SerializeField] private GameObject TeamContainerA;
    [SerializeField] private TMP_Text TeamNameA;
    [SerializeField] private TMP_Text ScoreA;
    [SerializeField] private TMP_Text AddedScoreA;

    [SerializeField] private GameObject TeamContainerB;
    [SerializeField] private TMP_Text TeamNameB;
    [SerializeField] private TMP_Text ScoreB;
    [SerializeField] private TMP_Text AddedScoreB;

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void SetTeamNames(string teamA, string teamB)
    {
        TeamNameA.text = teamA;
        TeamNameB.text = teamB;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void UpdateScore(int addedScore, int totalScore, bool isTeamA)
    {
        var scoreUI = isTeamA ? ScoreA : ScoreB;
        var addedScoreUI = isTeamA ? AddedScoreA : AddedScoreB;

        addedScoreUI.text = (addedScore >= 0 ? "+" : "") + addedScore;

        addedScoreUI.GetComponentInParent<TweenHandler>().Begin(onComplete: delegate
        {
            scoreUI.text = totalScore.ToString();
            addedScoreUI.GetComponentInParent<TweenHandler>().Begin(reverse: true);
        });
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void SetActiveTeam(bool activeTeamIsA)
    {
        if (activeTeamIsA)
        {
            TeamContainerA.SetVisible(1);
            TeamContainerB.SetVisible(0.5f);
        }
        else
        {
            TeamContainerA.SetVisible(0.5f);
            TeamContainerB.SetVisible(1);
        }
    }
}
