using OnlyCornect;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuzzerRegistrationUI : MonoBehaviour
{
    public TMP_Text TeamAText;
    public TMP_Text TeamBText;

    public List<BuzzerRegistrationContainerUI> TeamA;
    public List<BuzzerRegistrationContainerUI> TeamB;

    private IEnumerable<BuzzerRegistrationContainerUI> allBuzzerRegs;

    [SerializeField] private Button StartQuizButton;

    // --------------------------------------------------------------------------------------------------------------------------------------
    private void Start()
    {
        allBuzzerRegs = TeamA.Concat(TeamB);
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void SetTeamNameTexts()
    {
        TeamAText.text = GameManager.TeamA.Name;
        TeamBText.text = GameManager.TeamB.Name;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void RegisterBuzzer(string playerName, string playerIP)
    {
        bool isTeamA = true;
        var buzzer = TeamA.FirstOrDefault(x => x.IsSelected);

        if (!buzzer)
        {
            isTeamA = false;
            buzzer = TeamB.FirstOrDefault(x => x.IsSelected);
        }

        if (buzzer)
        {
            GameManager.TeamA.Players.RemoveAll(x => x.IP == playerIP);
            GameManager.TeamB.Players.RemoveAll(x => x.IP == playerIP);
            foreach (var buzzerReg in allBuzzerRegs)
            {
                if (buzzerReg.IP == playerIP)
                {
                    buzzerReg.Text.text = string.Empty;
                    buzzerReg.IP = string.Empty;
                }
            }

            buzzer.Text.text = playerName;
            buzzer.IP = playerIP;

            var team = isTeamA ? GameManager.TeamA : GameManager.TeamB;
            team.Players.Add(new GameManager.Player()
            {
                Name = playerName,
                IP = playerIP,
                IsCaptain = buzzer == (isTeamA ? TeamA[0] : TeamB[0])
            });

            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}
