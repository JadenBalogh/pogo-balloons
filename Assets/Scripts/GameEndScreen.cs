using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class GameEndScreen : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI endText;
    [SerializeField] private TextMeshProUGUI returnText;
    [SerializeField] private int returnTime = 5;

    private CanvasGroup canvasGroup;

    private void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
    }

    private void Start()
    {
        GameManager.OnGameEnd.AddListener(ShowEndScreen);
    }

    private void ShowEndScreen(int winningTeam)
    {
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        endText.color = winningTeam == 1 ? GameManager.Team1Color : GameManager.Team2Color;
        endText.text = "Team " + winningTeam + " is the winner!";
        StartCoroutine(ReturnTimer());
    }

    private IEnumerator ReturnTimer()
    {
        while (returnTime > 0)
        {
            yield return new WaitForSeconds(1);
            returnTime--;
            returnText.text = "Returning to main menu in " + returnTime + " seconds...";
        }
        GameManager.LeaveGame();
    }
}
