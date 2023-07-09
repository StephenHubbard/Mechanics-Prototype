using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Score : MonoBehaviour
{
    public int CurrentScore { get; private set; }

    private TMP_Text _scoreText;

    private void OnEnable() {
        Health.OnDeath += EnemyKilled;
    }

    private void OnDisable() {
        Health.OnDeath -= EnemyKilled;
    }

    private void Awake() {
        _scoreText = GetComponent<TMP_Text>();
    }

    public void EnemyKilled(Health sender) {
        CurrentScore++;
        _scoreText.text = CurrentScore.ToString("D3");
    }
}
