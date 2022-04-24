using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private Transform playerSpawn;
    [SerializeField] private Transform balloonSpawn;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject balloonPrefab;

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

    public static void EndRound()
    {
        //TODO: manage points/score
        Balloon.transform.position = instance.balloonSpawn.position;
    }
}
