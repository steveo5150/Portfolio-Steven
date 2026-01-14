using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class bigEnemy : MonoBehaviour
{
    [Header("Movement Settings")]
    public float walkSpeed = 1.5f;
    public float stopDistance = 4f;           // Enemy stops approaching if closer than this

    [Header("Charge Settings")]
    public float chargeSpeed = 10f;
    public float chargeTriggerDistance = 7f;  // Enemy can only charge if within this distance
    public float chargeCooldown = 4f;
    public float stunDuration = 2f;           // Time after charge where enemy can't move or charge
    public int chargeOvershoot = 4;          // How far past the player the charge goes
    public float chargeKnockbackForce = 100f; // Knockback force applied to player


    [Header("Combat Settings")]
    public int maxHealth = 40;
    public int damage = 15;

    // Private state
    private int currentHealth;
    private Transform player;
    private CharacterController controller;
    private Vector3 velocity; // For gravity
    public healthManager HM;

    private bool isCharging = false;
    private bool isStunned = false;
    private float lastChargeTime = -Mathf.Infinity;
    private float stunTimer = 0f;
    private Vector3 chargeDirection;
    private float chargeDistance = 0f;
    private float chargeTraveled = 0f;
    private bool hasHitPlayerThisCharge = false;


    void Start()
    {
        currentHealth = maxHealth;
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        controller = GetComponent<CharacterController>();
    }

    void Update()
    {
        if (player == null) return;

        // Gravity
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += Physics.gravity.y * Time.deltaTime;

        if (isStunned)
        {
            stunTimer += Time.deltaTime;
            if (stunTimer >= stunDuration)
            {
                isStunned = false;
                stunTimer = 0f;
            }
            // Still apply gravity while stunned
            controller.Move(new Vector3(0, velocity.y, 0) * Time.deltaTime);
            return;
        }

        if (isCharging)
        {
            ChargeMove();
        }
        else
        {
            NormalMove();
        }
    }

    void NormalMove()
    {
        Vector3 toPlayer = player.position - transform.position;
        toPlayer.y = 0;
        float distance = toPlayer.magnitude;
        Vector3 walkDir = toPlayer.normalized;

        Vector3 move = Vector3.zero;
        // Only approach if farther than stopDistance
        if (distance > stopDistance)
        {
            if (walkDir != Vector3.zero)
                transform.rotation = Quaternion.LookRotation(walkDir);

            move = walkDir * walkSpeed;
        }
        move.y = velocity.y;
        controller.Move(move * Time.deltaTime);

        // Start charge if cooldown is over and within chargeTriggerDistance
        if (Time.time - lastChargeTime >= chargeCooldown &&
            distance <= chargeTriggerDistance)
        {
            isCharging = true;
            chargeDirection = walkDir;
            chargeTraveled = 0f;
            chargeDistance = distance + chargeOvershoot;
            hasHitPlayerThisCharge = false; // Reset hit flag for new charge

        }
    }

    void ChargeMove()
    {
        // Move in the charge direction for the set charge distance
        float moveStep = chargeSpeed * Time.deltaTime;
        Vector3 move = chargeDirection * moveStep;
        move.y = velocity.y;
        controller.Move(move);

        // Only count horizontal distance for charge
        Vector3 horizontalMove = new Vector3(move.x, 0, move.z);
        chargeTraveled += horizontalMove.magnitude;

        if (chargeTraveled >= chargeDistance)
        {
            isCharging = false;
            lastChargeTime = Time.time;
            isStunned = true;
            stunTimer = 0f;

        }
    }


    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (isCharging && !hasHitPlayerThisCharge && hit.gameObject.CompareTag("Player"))
        {
            Debug.Log("Big Enemy hit player during charge!");
            var health = hit.gameObject.GetComponent<healthManager>();
            if (health != null)
            {
                Debug.Log("Big Enemy dealing damage: " + damage);
                health.TakeDamage(damage, true);
            }

            // Try to apply knockback if the player has a method for it
            var playerController = hit.gameObject.GetComponent<PlayerController>();
            if (playerController != null)
            {
                Vector3 knockbackDir = (hit.gameObject.transform.position - transform.position).normalized;
                knockbackDir.y = 0; // Only horizontal knockback
                playerController.TakeKnockback(knockbackDir, chargeKnockbackForce);
            }

            hasHitPlayerThisCharge = true;
        }
    }
}