using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreText : MonoBehaviour
{
    [SerializeField] private int team = 1;

    private TextMeshProUGUI textbox;

    private void Awake()
    {
        textbox = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        GameManager.OnScoreChanged.AddListener(UpdateScore);
    }

    private void UpdateScore(int team1Score, int team2Score)
    {
        if (team == 1)
        {
            textbox.text = team1Score.ToString();
        }
        else
        {
            textbox.text = team2Score.ToString();
        }
    }
}
