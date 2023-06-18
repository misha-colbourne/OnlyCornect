using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BuzzerRegistrationContainerUI : MonoBehaviour, ISelectHandler, IDeselectHandler
{
    public TMP_Text Text;
    public Button SelectableButton;

    [HideInInspector] public bool IsSelected = false;
    [HideInInspector] public string IP;

    private void Start()
    {
        Text.text = string.Empty;
    }

    // --------------------------------------------------------------------------------------------------------------------------------------
    public void OnSelect(BaseEventData eventData)
    {
        IsSelected = true;
    }

    public void OnDeselect(BaseEventData eventData)
    {
        IsSelected = false;
    }
}
