using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    [SelectionBase]
    public class ScorePopupUI : MonoBehaviour
    {
        [SerializeField] private Image teamNameBox;
        [SerializeField] private TMP_Text teamNameText;
        [SerializeField] private TMP_Text addedScoreText;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Awake()
        {
            gameObject.SetVisible(false);
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void ShowScoreChange(string teamName, int addedScore, bool isTeamA)
        {
            teamNameText.text = teamName;
            addedScoreText.text = (addedScore >= 0 ? "+" : "") + addedScore;

            teamNameBox.sprite = isTeamA ? UtilitiesForUI.Instance.TEAM_BOX_A : UtilitiesForUI.Instance.TEAM_BOX_B;

            foreach (var tween in GetComponents<TweenHandler>())
                tween.Begin();
        }
    }
}
