using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f; // Adjust this value to control movement speed
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>(); // Getting reference to Rigidbody2D component
        spriteRenderer = GetComponent<SpriteRenderer>(); // Getting reference to SpriteRenderer component
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Input handling for movement
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        Vector2 movement = new Vector2(moveHorizontal, moveVertical);
        movement.Normalize(); // Normalize the vector to ensure consistent movement speed regardless of direction

        // Applying movement
        rb.velocity = movement * moveSpeed;

        // Flipping the sprite based on movement direction
        if (movement.x > 0) // Moving right
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x < 0) // Moving left
        {
            spriteRenderer.flipX = false;
        }

        // Setting animation parameters based on movement
        if (movement.magnitude > 0) // If there is movement
        {
            animator.SetBool("Run", true);
        }
        else // If there is no movement
        {
            animator.SetBool("Run", false);
        }
    }

}


