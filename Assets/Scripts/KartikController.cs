using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class CharacterController2D : MonoBehaviour
{
    // Variables for Rigidbody2D and Animator
    private Rigidbody2D rb;
    private Animator animator;

    // Animator parameter hashes
    private int isJumpingHash;
    private int isRunningHash;
    private int isAttackingHash;
    private int isEndedHash;
    // Movement variables
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float maxJumpTime = 0.5f; // Maximum time allowed to hold the jump
    private float jumpTimeCounter; // To track how long the jump button has been held

    // State variables
    private bool isGrounded = true;
    private bool isJumping = false;
    private bool isAttacking = false;
    private bool canAttack = true; // To track if the player can attack

    // Dash variables
    public float dashDistance = 2f;  // The distance to dash, adjust as needed
    public float dashDuration = 0.1f; // The duration of the dash, adjust as needed
    public float dashCooldown = 1f; // Cooldown time between dashes

    // Reference to the GameObject to move during the dash
    public GameObject camera_container;
    private CircleCollider2D circleCollider;

    // Reference to the Game_Manager
    public Game_manager gameManager;
    public Slider cooldownSlider;
    private int frameCounter;

    // lists
    private List<string> actionDataList = new List<string>();

    public string actionDataString = "";

    void Start()
    {
        // Get the Rigidbody2D and Animator components attached to the character
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();

        // Initialize animator parameter hashes
        isJumpingHash = Animator.StringToHash("isJumping");
        isRunningHash = Animator.StringToHash("isRunning");
        isAttackingHash = Animator.StringToHash("isAttacking");
        isEndedHash = Animator.StringToHash("isEnded");
    }


    void FormatActionData(int actionId)
    {
        int currentFrame = frameCounter;

        // Construct the string in the format "(currentFrame, actionId)"
        string actionData = $"({currentFrame}, {actionId})";

        // Add the formatted string to the list
        actionDataList.Add(actionData);

        // Join all elements in the list into a single string with commas
        actionDataString = string.Join(",", actionDataList);
    }

    void Update()
    {   
        
        // Handle jumping
        if (Input.GetKeyDown(KeyCode.UpArrow) && isGrounded && !isAttacking)
        {
            StartJump();
            FormatActionData(1);
        }

        if (Input.GetKey(KeyCode.UpArrow) && isJumping)
        {
            ContinueJump();
        }

        if (Input.GetKeyUp(KeyCode.UpArrow))
        {
            EndJump();
            FormatActionData(2);
        }

        // Handle attacking
        if (Input.GetKeyDown(KeyCode.RightArrow) && !isAttacking && canAttack)
        {
            StartCoroutine(Attack());
            FormatActionData(3);
        }

        // Update animator parameters
        animator.SetBool(isRunningHash, isGrounded && !isAttacking);
        animator.SetBool(isJumpingHash, !isGrounded);
    }

    void FixedUpdate()
    {   
        frameCounter += 1;
        // Move the character to the right continuously
        rb.velocity = new Vector2(moveSpeed, rb.velocity.y);

        if (rb.velocity.y > 0)
        {
            // Moving upwards, deactivate the CircleCollider2D
            circleCollider.enabled = false;
        }
        else
        {
            // Moving downwards or stationary, activate the CircleCollider2D
            circleCollider.enabled = true;
        }
    }

    void StartJump()
    {
        isJumping = true;
        jumpTimeCounter = maxJumpTime;
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        isGrounded = false;
        animator.SetBool(isJumpingHash, true);
    }

    void ContinueJump()
    {
        if (jumpTimeCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpTimeCounter -= Time.deltaTime;
        }
        else
        {
            EndJump();
        }
    }

    void EndJump()
    {
        isJumping = false;
    }
  IEnumerator Attack()
    {
        // Start attack
        isAttacking = true;
        canAttack = false; // Prevent attacking again until cooldown
        animator.SetBool(isAttackingHash, true);
        Debug.Log("Attack started");

        // Immediately set the slider to 0 (cooldown starts)
        if (cooldownSlider != null)
        {
            cooldownSlider.value = 0f;
        }

        // Calculate the dash speed
        float dashSpeed = dashDistance / dashDuration;
        float dashTimeElapsed = 0f;

        // Start fading effect
        StartCoroutine(SpawnFadeObjects());

        while (dashTimeElapsed < dashDuration)
        {
            // Translate the player to the right
            transform.Translate(Vector2.right * dashSpeed * Time.deltaTime);

            // Translate the imported GameObject to the right
            if (camera_container != null)
            {
                camera_container.transform.Translate(Vector2.right * 1.99f * Time.deltaTime);
            }

            dashTimeElapsed += Time.deltaTime;
            yield return null;
        }

        // End attack
        Debug.Log("Attack ended");
        isAttacking = false;
        animator.SetBool(isAttackingHash, false);

        // Replenish the cooldown slider over time
        float cooldownTimeElapsed = 0f;
        while (cooldownTimeElapsed < dashCooldown)
        {
            cooldownTimeElapsed += Time.deltaTime;
            if (cooldownSlider != null)
            {
                // Replenish from 0 to 100 over dashCooldown time
                cooldownSlider.value = Mathf.Lerp(0f, 100f, cooldownTimeElapsed / dashCooldown);
            }
            yield return null;
        }

        canAttack = true; // Allow attacking again after cooldown
    }
    IEnumerator SpawnFadeObjects()
    {
        float spawnDelay = 0.01f; // Reduced delay for less distance

        for (int i = 0; i < 3; i++)
        {
            // Instantiate the fade object
            GameObject fadeObject = new GameObject("FadeObject");
            SpriteRenderer spriteRenderer = fadeObject.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = GetComponent<SpriteRenderer>().sprite; // Use the same sprite as the player
            spriteRenderer.color = new Color(1f, 1f, 1f, 1f); // Set initial color with full alpha

            // Set scale to match the player's scale
            fadeObject.transform.localScale = transform.localScale;

            // Position the fade object at the player's position
            fadeObject.transform.position = transform.position;

            // Start fading coroutine
            StartCoroutine(FadeOutAndDestroy(fadeObject, spriteRenderer));

            // Delay before spawning the next object
            yield return new WaitForSeconds(spawnDelay);
        }
    }

    IEnumerator FadeOutAndDestroy(GameObject fadeObject, SpriteRenderer spriteRenderer)
    {
        float fadeDuration = 0.2f; // Duration for fade out
        float fadeTimeElapsed = 0f;

        while (fadeTimeElapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(1f, 0f, fadeTimeElapsed / fadeDuration);
            spriteRenderer.color = new Color(1f, 1f, 1f, alpha);

            fadeTimeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the sprite is fully transparent before destroying
        spriteRenderer.color = new Color(1f, 1f, 1f, 0f);
        Destroy(fadeObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the character landed on the ground
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Check if the character is moving downward
            if (rb.velocity.y <= 0.3f)
            {
                isGrounded = true;
                isJumping = false;
                animator.SetBool(isJumpingHash, false);
                // Enable collision detection when moving downward
                rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Handle trigger with the finish object
        if (collision.gameObject.CompareTag("Finish"))
        {
            animator.SetBool(isEndedHash, true);
            HandleFinishCollision();
        }

        // Handle trigger with the "eth" object
        if (collision.gameObject.CompareTag("eth"))
        {
            // Access and play the Particle System
            ParticleSystem particleSystem = collision.gameObject.GetComponent<ParticleSystem>();
            if (particleSystem != null)
            {
                particleSystem.Play();
            }

            // Destroy the first child of the game object
            if (collision.gameObject.transform.childCount > 0)
            {
                Destroy(collision.gameObject.transform.GetChild(0).gameObject);
            }

            // Disable the BoxCollider2D
            BoxCollider2D boxCollider = collision.gameObject.GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                gameManager.IncreaseScore();
                boxCollider.enabled = false;
            }

            // Reset the attack cooldown
            canAttack = true;
            StopAllCoroutines(); // Stop any ongoing cooldown coroutine

            // Reset the slider to 100%
            if (cooldownSlider != null)
            {
                cooldownSlider.value = 100f;
            }
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        // Re-enable collision detection when the character exits collision
        if (collision.gameObject.CompareTag("Ground"))
        {
            // Enable collision detection when exiting collision
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    
        }
    }

    private void HandleFinishCollision()
    {
        // Set camera_container BaseSpeed and CatchupSpeed to 0
        if (camera_container != null)
        {
            var cameraMovement = camera_container.GetComponent<Camera_controller>();
            if (cameraMovement != null)
            {
                cameraMovement.baseSpeed = 0;
                cameraMovement.catchupSpeed = 0;
            }
        }

        // Update the Game_Manager state
        if (gameManager != null)
        {
            gameManager.isGameFinished = true;

            // Display the game recap
            if (gameManager.panel != null)
            {
                gameManager.panel.SetActive(true);
            }
        }

        // Disable this script (CharacterController2D)
        gameManager.actionDataString = actionDataString;
        this.enabled = false;
 
    }
}
