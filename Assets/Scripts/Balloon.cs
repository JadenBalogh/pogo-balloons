using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Balloon : MonoBehaviour
{
    [SerializeField] private Transform spawnPos;
    [SerializeField] private LayerMask playerMask;
    [SerializeField] private LayerMask groundMask;

    public UnityEvent<int> OnScoreChanged { get; private set; }

    private int score = 0;

    private void Awake()
    {
        OnScoreChanged = new UnityEvent<int>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        // If we hit the ground
        if (((1 << col.gameObject.layer) & groundMask) != 0)
        {
            Respawn();
        }

        // If we hit the ground
        if (((1 << col.gameObject.layer) & playerMask) != 0)
        {
            score++;
            OnScoreChanged.Invoke(score);
        }
    }

    public void Respawn()
    {
        transform.position = spawnPos.position;
    }
}
