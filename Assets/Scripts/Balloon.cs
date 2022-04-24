using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class Balloon : MonoBehaviourPun
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float bounceForce = 10f;

    public UnityEvent<int> OnScoreChanged { get; private set; }

    // The Team number that last hit the balloon
    public int Team { get; private set; }

    private int score = 0;

    private new Rigidbody2D rigidbody2D;
    private new Collider2D collider2D;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        OnScoreChanged = new UnityEvent<int>();

        if (!PhotonNetwork.IsMasterClient)
        {
            collider2D.enabled = false;
        }

        Team = -1;
    }

    private void Start()
    {
        UpdateColor();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // If we hit the ground
        if (((1 << col.gameObject.layer) & groundMask) != 0)
        {
            GameManager.EndRound();
        }

        // If we hit a player
        if (((1 << col.gameObject.layer) & playerMask) != 0)
        {
            score++;
            OnScoreChanged.Invoke(score);

            Vector2 hitNormal = col.GetContact(0).normal;
            rigidbody2D.AddForce(hitNormal * bounceForce, ForceMode2D.Impulse);

            Player player = col.gameObject.GetComponent<Player>();
            if (player.TeamNumber != Team)
            {
                photonView.RPC("ChangeTeam", RpcTarget.All, player.TeamNumber);
            }
        }
    }

    [PunRPC]
    public void ChangeTeam(int team)
    {
        Team = team;
        UpdateColor();
    }

    private void UpdateColor()
    {
        if (Team == 0)
        {
            spriteRenderer.color = GameManager.Team1Color;
        }
        else if (Team == 1)
        {
            spriteRenderer.color = GameManager.Team2Color;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }
}
