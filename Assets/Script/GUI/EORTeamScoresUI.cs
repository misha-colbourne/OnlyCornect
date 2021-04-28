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
        [SerializeField] private GameObject CrownA;

        [SerializeField] private GameObject TeamContainerB;
        [SerializeField] private TMP_Text TeamNameB;
        [SerializeField] private TMP_Text ScoreB;
        [SerializeField] private GameObject CrownB;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SetNamesAndScores(GameManager.Team teamA, GameManager.Team teamB, bool finished)
        {
            TeamNameA.text = teamA.Name;
            ScoreA.text = teamA.Score.ToString();

            TeamNameB.text = teamB.Name;
            ScoreB.text = teamB.Score.ToString();

            CrownA.SetVisible(false);
            CrownB.SetVisible(false);

            if (finished)
            {
                if (teamA.Score > teamB.Score)
                    CrownA.SetVisible(true);
                else
                    CrownB.SetVisible(true);
            }
        }
    }
}
