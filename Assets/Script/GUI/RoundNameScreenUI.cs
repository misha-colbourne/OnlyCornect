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
        [SerializeField] private TMP_Text RoundName;

        // --------------------------------------------------------------------------------------------------------------------------------------
        public void SetText(string text)
        {
            RoundName.text = text;
        }
    }
}
