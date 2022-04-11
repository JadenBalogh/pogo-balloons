using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Controls")]
    [SerializeField] private float correctForceProportion = 0.5f;
    [SerializeField] private float inputRotationForce = 2f;
    [SerializeField] private float springCompPerSecond = 2f;
    [SerializeField] private float springDecompPerSecond = 12f;
    [SerializeField] private float minBounceForce = 5f;
    [SerializeField] private float maxBounceForce = 15f;
    [SerializeField] private float angleOffset = -90f;

    [Header("Spring")]
    [SerializeField] private Transform playerVisual;
    [SerializeField] private float minHeight = -0.4f;
    [SerializeField] private float maxHeight = 0f;

    [Header("Grounding")]
    [SerializeField] private Transform footRef;
    [SerializeField] private float groundedRadius = 0.5f;
    [SerializeField] private LayerMask groundMask;

    [Header("Debug")]
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float knockback = 10f;

    private float inputH = 0;
    private float springComp = 0;
    private bool isGrounded = true;

    private Rigidbody2D rb2D;

    private void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        inputH = Input.GetAxis("Horizontal");

        Collider2D groundCol = Physics2D.OverlapCircle(footRef.position, groundedRadius, groundMask);
        isGrounded = groundCol != null;

        if (Input.GetKey(KeyCode.Space))
        {
            springComp = Mathf.Min(springComp + springCompPerSecond * Time.deltaTime, 1f);
        }
        else
        {
            springComp = Mathf.Max(springComp - springDecompPerSecond * Time.deltaTime, 0f);
        }

        playerVisual.transform.localPosition = Vector3.up * Mathf.Lerp(maxHeight, minHeight, springComp);

        if (isGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            float rotAngle = (rb2D.rotation + angleOffset) * Mathf.Deg2Rad;
            Vector2 bounceDir = new Vector2(Mathf.Cos(rotAngle), Mathf.Sin(rotAngle)).normalized;
            float bounceForce = Mathf.Lerp(minBounceForce, maxBounceForce, springComp);
            rb2D.AddForce(bounceDir * bounceForce, ForceMode2D.Impulse);
        }

        if (Input.GetButtonDown("Fire1"))
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Collider2D jumpCol = Physics2D.OverlapCircle(mousePos, 3f, playerLayer);
            if (jumpCol != null)
            {
                Vector2 targetPos = jumpCol.transform.position;
                jumpCol.GetComponent<Rigidbody2D>().AddForce((targetPos - mousePos).normalized * knockback, ForceMode2D.Impulse);
            }
        }
    }

    private void FixedUpdate()
    {
        rb2D.AddTorque(-inputH * inputRotationForce);
        rb2D.AddTorque(-rb2D.rotation * correctForceProportion);
    }
}
