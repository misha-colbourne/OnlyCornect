using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class TeamNameEntryUI : MonoBehaviour
    {
        [SerializeField] private GameObject TeamContainerA;
        [SerializeField] private TMP_InputField TeamNameEntryA;

        [SerializeField] private GameObject TeamContainerB;
        [SerializeField] private TMP_InputField TeamNameEntryB;

        [SerializeField] private Button StartQuizButton;

        // --------------------------------------------------------------------------------------------------------------------------------------
        private void Update()
        {
            StartQuizButton.interactable = TeamNameEntryA.text.Length > 0 && TeamNameEntryB.text.Length > 0;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SetTeamNames(GameManager.Team teamA, GameManager.Team teamB)
        {
            teamA.Name = TeamNameEntryA.text;
            teamB.Name = TeamNameEntryB.text;
        }

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SwitchInputFocus()
        {
            if (TeamNameEntryA.isFocused)
                TeamNameEntryB.ActivateInputField();
            else
                TeamNameEntryA.ActivateInputField();
        }
    }
}
