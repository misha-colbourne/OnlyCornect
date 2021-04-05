using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnlyCornect
{
    [SelectionBase]
    public class TimeBoxUI : MonoBehaviour
    {
        public TMP_Text Text;
        public GameObject FillBar;
        [SerializeField] private Image Overlay;
    }
}
