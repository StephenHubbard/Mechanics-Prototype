using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Score : MonoBehaviour
{
    public int CurrentScore { get; private set; }

    private TMP_Text _scoreText;

    private void Awake() {
        _scoreText = GetComponent<TMP_Text>();
    }

    public void EnemyKilled() {
        CurrentScore++;
        _scoreText.text = CurrentScore.ToString("D3");
    }
}
