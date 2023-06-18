using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuzzerOverlayUI : MonoBehaviour
{
    public TMP_Text Text;
    public Image Image;
    [SerializeField] private Sprite teamA;
    [SerializeField] private Sprite teamB;

    public void Show(string player, bool isTeamA = true)
    {
        Image.sprite = isTeamA ? teamA : teamB;
        Text.text = player;
        foreach (var tween in GetComponents<TweenHandler>())
            tween.Begin();
    }
}
