using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;
using Photon.Pun.UtilityScripts;

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager instance;

    [SerializeField] private int winScore = 5;

    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform balloonSpawn;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject balloonPrefab;

    [SerializeField] private Color team1Color;
    public static Color Team1Color { get => instance.team1Color; }

    [SerializeField] private Color team2Color;
    public static Color Team2Color { get => instance.team2Color; }

    public static int Team1Score { get; private set; }
    public static int Team2Score { get; private set; }
    public static UnityEvent<int, int> OnScoreChanged { get; private set; }
    public static UnityEvent<int> OnGameEnd { get; private set; }

    public static Player Player { get; private set; }
    public static Balloon Balloon { get; private set; }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;

        Team1Score = 0;
        Team2Score = 0;
        Player = null;
        Balloon = null;

        OnScoreChanged = new UnityEvent<int, int>();
        OnGameEnd = new UnityEvent<int>();

        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.EmptyRoomTtl = 1;
        }
    }

    private void Start()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, playerSpawn.position, Quaternion.identity);
        Player = player.GetComponent<Player>();

        if (PhotonNetwork.IsMasterClient)
        {
            GameObject balloon = PhotonNetwork.Instantiate(balloonPrefab.name, balloonSpawn.position, Quaternion.identity);
            Balloon = balloon.GetComponent<Balloon>();
        }
    }

    public static void LeaveGame()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.CurrentRoom.IsOpen = false;
        }
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("Lobby");
    }

    public static void EndRound()
    {
        if (Balloon.Team == 0)
        {
            Team1Score++;
            if (Team1Score >= instance.winScore)
            {
                instance.photonView.RPC("EndGame", RpcTarget.All, 1);
            }
        }
        else if (Balloon.Team == 1)
        {
            Team2Score++;
            if (Team2Score >= instance.winScore)
            {
                instance.photonView.RPC("EndGame", RpcTarget.All, 2);
            }
        }
        instance.photonView.RPC("UpdateScores", RpcTarget.All, Team1Score, Team2Score);

        Balloon.transform.position = instance.balloonSpawn.position;
        Balloon.photonView.RPC("ChangeTeam", RpcTarget.All, -1);
    }

    [PunRPC]
    public void EndGame(int winningTeam)
    {
        OnGameEnd.Invoke(winningTeam);
    }

    [PunRPC]
    public void UpdateScores(int team1Score, int team2Score)
    {
        Team1Score = team1Score;
        Team2Score = team2Score;
        OnScoreChanged.Invoke(Team1Score, Team2Score);
    }
}
