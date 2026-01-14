using UnityEngine;

// Controls player movement and rotation.
public class PlayerController : MonoBehaviour
{


    public PlayerSettings playerSettings; // Reference to PlayerSettings ScriptableObject
    public weaponData weaponData; // Reference to weaponData ScriptableObject
    private CharacterController controller;
    private Vector3 velocity;
    private bool isGrounded;
    private Vector3 knockback = Vector3.zero; // Current knockback force
    public healthManager healthManager; // Reference to the health manager script
    //UI
    public playerUI playerUI; // Reference to the player UI script
    //dash settings
    private float dashRechargeTimer = 0f; // recharge timer for dash
    private Vector3 dashVelocity = Vector3.zero;
    private int dashCharges = 0; // Number of available dashes
    private Coroutine dashCoroutine;
    private bool isDashing = false; // Flag to check if the player is dashing
    //jump settings
    private int jumpCount; // Flag to allow double jump
    private bool isFastFalling = false; // Flag to check if the player is fast falling

    private void Start()
    {
        //initialize controller component and mouse settings
        Cursor.lockState = CursorLockMode.Locked; // Lock the cursor to the center of the screen
        Cursor.visible = false; // Hide the cursor
        controller = GetComponent<CharacterController>();

        healthManager.setMaxHealth(playerSettings.maxHealth); // Initialize health in health manager
        if (playerUI != null)
            playerUI.SetUIHealth(healthManager.GetHealth(), playerSettings.maxHealth);
    }

    // Update is called once per frame
    void Update()
    {
        if (controller.isGrounded)
        {
            jumpCount = 0; // Reset jump count when grounded
            if (isFastFalling)
            {
                isFastFalling = false; // Reset fast fall flag when grounded
            }
        }
        if (isDashing)
            return;
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // Small downward force to keep grounded
        }
        //add horizontal rotation based on mouse movement
        float moveX = Input.GetAxis("Horizontal");
        float moveZ = Input.GetAxis("Vertical");
        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        move *= playerSettings.speed;
        if (Input.GetKeyDown(KeyCode.LeftShift) && dashCharges > 0 && !isDashing)
        {
            Vector3 dashDir = (transform.right * moveX + transform.forward * moveZ).normalized;
            if (dashDir == Vector3.zero)
                dashDir = transform.forward; // Default to forward if no input

            if (dashCoroutine != null)
                StopCoroutine(dashCoroutine);

            dashCoroutine = StartCoroutine(Dash(dashDir, playerSettings.dashDistance, playerSettings.dashDuration)); // 0.2f = dash duration in seconds
            dashCharges--; // Decrease dash charges
        }
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < playerSettings.maxJumps) //add double jump check)
        {
            
            velocity.y = Mathf.Sqrt(playerSettings.jumpHeight * -2f * playerSettings.gravity);
            jumpCount++; // Increment jump count
        }
        if (Input.GetKeyDown(KeyCode.LeftControl) && !controller.isGrounded && !isFastFalling)
        {
            velocity.y = playerSettings.fastFallVelocity;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            //add pause menu functionality here
            Cursor.lockState = CursorLockMode.None; // Unlock the cursor
            Cursor.visible = true; // Show the cursor
        }
        //gravity
        velocity.y += playerSettings.gravity * Time.deltaTime;
        move.y = velocity.y;
        //add and dampen knockback
        move += knockback;
        knockback = Vector3.Lerp(knockback, Vector3.zero, Time.deltaTime * 5f); // Dampen knockback over time

        //move the player
        controller.Move(move * Time.deltaTime);

        // Rotate the player based on mouse movement left and right
        float mouseX = Input.GetAxis("Mouse X") * playerSettings.mouseSensitivity * Time.deltaTime;
        transform.Rotate(0f, mouseX, 0f); // Rotate camera based on mouse movement
        //refill dashes
        if (dashCharges < playerSettings.maxDashCharges)
        {
            dashRechargeTimer += Time.deltaTime;
            if (dashRechargeTimer >= playerSettings.dashCooldown)
            {
                dashCharges++;
                dashRechargeTimer = 0f;
            }
            if (playerUI != null)
                playerUI.SetUIDash(dashCharges, playerSettings.maxDashCharges);
        }
        else
        {
            dashRechargeTimer = 0f; // Reset timer if at max
        }
    }
    public void RefillDashCharge()
    {
        if (dashCharges < playerSettings.maxDashCharges)
        {
            dashCharges++;
            if (playerUI != null)
                playerUI.SetUIDash(dashCharges, playerSettings.maxDashCharges);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        //hit by hitbox, take damage
        if (other.CompareTag("fodderHitBox"))
        {
            TakeDamage(weaponData.fodderDamage * playerSettings.difficultyLevel); // fodder does 5 damage
            Vector3 hitDirection = (other.transform.position - transform.position).normalized;
            hitDirection.y = 0; // Ignore vertical direction for knockback
            TakeKnockback(hitDirection, weaponData.fodderKnockbackForce);
            Destroy(other.gameObject); // Destroy the hitbox after it hits the player
        }
    }

    void TakeDamage(int amount)
    { 
        healthManager.TakeDamage(amount, true); // Pass true for player

    }
    public void TakeKnockback(Vector3 direction, float force)
    {
        // Apply knockback force to the enemy
        direction.y = 0; // Ignore vertical direction for knockback
        knockback += direction.normalized * force;
        //Debug.Log("Enemy knocked back!");
    }
    void gameOver()
    {
               // Handle game over logic here, e.g., show game over screen or restart level
        Debug.Log("Game Over!");
    }
    void Die()
    {
        // Handle player death logic here, e.g., respawn or game over
        Debug.Log("Player has died.");
    }

    private System.Collections.IEnumerator Dash(Vector3 dashDir, float dashDistance, float dashDuration)
    {
        isDashing = true;
        float elapsed = 0f;
        Vector3 start = transform.position;
        Vector3 target = start + dashDir * dashDistance;

        while (elapsed < dashDuration)
        {
            float t = elapsed / dashDuration;
            Vector3 nextPos = Vector3.Lerp(start, target, t);
            controller.Move(nextPos - transform.position);
            elapsed += Time.deltaTime;
            yield return null;
        }
        // Ensure final position
        controller.Move(target - transform.position);
        isDashing = false;
    }
}