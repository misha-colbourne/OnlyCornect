using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace OnlyCornect
{
    public class EORTeamScoresUI : MonoBehaviour
    {
        [SerializeField] private GameObject TeamContainerA;
        [SerializeField] private TMP_Text TeamNameA;
        [SerializeField] private TMP_Text ScoreA;

        [SerializeField] private GameObject TeamContainerB;
        [SerializeField] private TMP_Text TeamNameB;
        [SerializeField] private TMP_Text ScoreB;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SetNamesAndScores(GameManager.Team teamA, GameManager.Team teamB)
        {
            TeamNameA.text = teamA.Name;
            ScoreA.text = teamA.Score.ToString();

            TeamNameB.text = teamB.Name;
            ScoreB.text = teamB.Score.ToString();
        }
    }
}
