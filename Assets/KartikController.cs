using System.Collections;
using UnityEngine;

public class CharacterController2D : MonoBehaviour
{
    // Variables for Rigidbody2D and Animator
    private Rigidbody2D rb;
    private Animator animator;

    // Animator parameter hashes
    private int isJumpingHash;
    private int isRunningHash;
    private int isAttackingHash;

    // Movement variables
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // State variables
    private bool isGrounded = true;
    private bool isAttacking = false;

    void Start()
    {
        // Get the Rigidbody2D and Animator components attached to the character
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        // Initialize animator parameter hashes
        isJumpingHash = Animator.StringToHash("isJumping");
        isRunningHash = Animator.StringToHash("isRunning");
        isAttackingHash = Animator.StringToHash("isAttacking");
    }

    void Update()
    {
        // Handle jumping
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && !isAttacking)
        {
            Jump();
        }

        // Handle attacking
        if (Input.GetKeyDown(KeyCode.RightArrow) && !isAttacking)
        {
            StartCoroutine(Attack());
        }

        // Update animator parameters
        animator.SetBool(isRunningHash, isGrounded && !isAttacking);
        animator.SetBool(isJumpingHash, !isGrounded);
    }

    void FixedUpdate()
    {
        // Move the character to the right continuously
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);
    }

    void Jump()
    {
        // Apply an upward force to the character's Rigidbody2D
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false; // Character is no longer on the ground
        animator.SetBool(isJumpingHash, true);
    }

    IEnumerator Attack()
    {
        // Start attack
        isAttacking = true;
        animator.SetBool(isAttackingHash, true);
        Debug.Log("Attack started");

        // Simulate attack duration
        yield return new WaitForSeconds(0.5f); // Adjust the duration as needed

        // End attack
        Debug.Log("Attack ended");
        isAttacking = false;
        animator.SetBool(isAttackingHash, false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            animator.SetBool(isJumpingHash, false);
        }
    }
}
