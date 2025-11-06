using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public float moveSpeed = 5f;       // Speed of movement
    private Rigidbody2D rb;
    private bool isGrounded = false;   // Ground check

    [Header("Ground Check")]
    public Transform groundCheck;      // Empty object at bottom of player
    public float groundRadius = 0.2f;  // Radius for ground detection
    public LayerMask whatIsGround;     // Set to "Ground" layer in Inspector

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Check if touching the ground
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        // Only move when grounded
        if (isGrounded)
        {
            // Constant movement to the right (change to -moveSpeed for left)
            rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
        }
        else
        {
            // No horizontal movement in the air
            rb.velocity = new Vector2(0f, rb.velocity.y);
        }
    }

    public bool GetIsGrounded()
    {
        return isGrounded;
    }
}
