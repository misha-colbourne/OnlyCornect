using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    public class RoundNameScreenUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text RoundNameText;
        [SerializeField] private List<string> RoundNames;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void NextRoundNameText(GameManager.ERound round)
        {
            RoundNameText.text = RoundNames[(int)round];
        }
    }
}
