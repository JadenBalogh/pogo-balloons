using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Photon.Pun;

public class Balloon : MonoBehaviour
{
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;
    [SerializeField] private float bounceForce = 10f;

    public UnityEvent<int> OnScoreChanged { get; private set; }

    private int score = 0;

    private new Rigidbody2D rigidbody2D;
    private new Collider2D collider2D;

    private void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        collider2D = GetComponent<Collider2D>();

        OnScoreChanged = new UnityEvent<int>();

        if (!PhotonNetwork.IsMasterClient)
        {
            collider2D.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // If we hit the ground
        if (((1 << col.gameObject.layer) & groundMask) != 0)
        {
            GameManager.EndRound();
        }

        // If we hit the player
        if (((1 << col.gameObject.layer) & playerMask) != 0)
        {
            score++;
            OnScoreChanged.Invoke(score);

            Vector2 hitNormal = col.GetContact(0).normal;
            rigidbody2D.AddForce(hitNormal * bounceForce, ForceMode2D.Impulse);
        }
    }
}
