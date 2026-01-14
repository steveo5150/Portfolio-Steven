using UnityEngine;

public class FodderEnemyController : MonoBehaviour
{
    //intialize components and variables with enemy settings
    public weaponData weaponData; // Reference to weaponData ScriptableObject
    private CharacterController controller;
    private Vector3 velocity;
    private Transform player;
    public float moveSpeed = 2.0f; // Speed of the enemy
    public float approachDistance = 8.0f; // Distance at which the enemy starts moving towards the player
    public float backupdistance = 25.0f; //maximum distance to which the enemy will back up
    public float attackDistance = 2.0f; // Distance at which the enemy attacks the player
    public int backupmodifier = 2; // How much the enemy backs up when too close
    //take damage and knockback settings
    private Vector3 knockback = Vector3.zero; // Current knockback force
    public float knockbackForce = 25.0f; // Force applied to the enemy when hit by the player
    public healthManager healthManager; // Reference to health manager script
    //attack settings
    private float lastAttack;
    public float attackCooldown = 2.0f; // Time between attacks
    public float attackDamage = 2.0f; // Force applied to the enemy when backing up
    //shooting settings
    public GameObject projectilePrefab; // Assign in Inspector
    public Transform projectileSpawnPoint; // Assign in Inspector (e.g., enemy's hand or mouth)
    public float shotCooldown = 2.0f; // Time between shots
    private float lastShot; // Timer for shot cooldown
    //hitbox settings
    public Vector3 hitboxSize = new Vector3(.5f, .5f, 3f); // Size of the hitbox
    public float hitboxDuration = 0.4f; // Duration for which the hitbox is active
    public float hitboxCooldown = .5f; // Cooldown before the next hitbox can be spawned
    private bool canSpawnHitbox = true; // Flag to control hitbox spawning

    private void Start()
    {
        //initialize controller component and mouse settings

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        controller = GetComponent<CharacterController>();
        lastAttack = Time.time;
        lastShot = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 lookDir = player.position - transform.position;
        lookDir.y = 0; // Keep only horizontal rotation
        if (lookDir != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(lookDir);

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);
        Vector3 move = Vector3.zero;

        if (Time.time - lastAttack < attackCooldown)
        {
            move = -(player.position - transform.position).normalized * moveSpeed * 2; // Move away from the player if in attack cooldown
        }
        else if (distanceToPlayer < approachDistance && distanceToPlayer > attackDistance)
        {
            // Move towards the player
            move = (player.position - transform.position).normalized * moveSpeed;

        }
        else if (distanceToPlayer <= attackDistance)
        {
            // Attack the player if within attack distance
            AttackPlayer();
        }
        else if (distanceToPlayer > approachDistance)
        {
            // Only call shooting logic once per frame if out of approach range
            callShotFire();

            if (distanceToPlayer < backupdistance)
            {
                move = -(player.position - transform.position).normalized * moveSpeed / backupmodifier;
            }
        }

        //gravity and grounding force
        if (controller.isGrounded && velocity.y < 0)
            velocity.y = -2f;
        velocity.y += Physics.gravity.y * Time.deltaTime;
        move.y = velocity.y;
        //add and dampen knockback
        move += knockback;
        knockback = Vector3.Lerp(knockback, Vector3.zero, Time.deltaTime * 5f); // Dampen knockback over time
        //move
        controller.Move(move * Time.deltaTime);

    }

    void ShootAtPlayer()
    {
        if (projectilePrefab == null || player == null) return;

        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + Vector3.up;
        GameObject proj = Instantiate(projectilePrefab, spawnPos, Quaternion.identity);
        Destroy(proj, 5f);
        Rigidbody rb = proj.GetComponent<Rigidbody>();
        if (rb == null) return;

        Vector3 targetPos = player.position;

        Vector3 velocity = CalculateLobVelocity(spawnPos, targetPos, weaponData.fodderSpeed);
        rb.linearVelocity = velocity;
        lastShot = Time.time; // Reset shot timer
        //Debug.Log("Shooting at player!");
    }
    void AttackPlayer()
    {
        //add melee attack, along with fodder running away after melee for certain time, or until player is out of range, use boolean to determine if enemy is attacking
        // Implement shooting logic here
        if (canSpawnHitbox)
        {
            StartCoroutine(SpawnHitbox());
            //Debug.Log("Spawning hitbox!");
        }
        lastAttack = Time.time; // Reset attack timer
        //Debug.Log("attacking at player!");
    }
    void OnTriggerEnter(Collider other)
    {
        //hit by hitbox, take damage
        if (other.CompareTag("fistHitBox"))
        {
            TakeKnockback(other.transform.forward, weaponData.fistKnockbackForce); // Apply knockback based on hitbox position
        }
    }
    void TakeKnockback(Vector3 direction, float force)
    {
        // Apply knockback force to the enemy
        direction.y = 0; // Ignore vertical direction for knockback
        knockback += direction.normalized * force;
        //Debug.Log("Enemy knocked back!");
    }
    void Die()
    {
        // Handle enemy death (destroy, play animation, etc.)
        Destroy(gameObject);
    }

    private System.Collections.IEnumerator SpawnHitbox()
    {
        canSpawnHitbox = false; // Prevent spawning another hitbox immediately
        // Create a new GameObject for the hitbox
        GameObject hitbox = new GameObject("DynamicHitbox");
        hitbox.tag = "fodderHitBox";
        hitbox.transform.parent = transform;
        hitbox.transform.position = transform.position + transform.forward * 2; // 2 units in front of the character

        hitbox.transform.rotation = transform.rotation;

        // Add a BoxCollider (set as trigger)
        BoxCollider collider = hitbox.AddComponent<BoxCollider>();
        collider.size = hitboxSize;
        collider.isTrigger = true;

        // (Optional) Add a visual for debugging
        hitbox.AddComponent<HitboxGizmo>();

        // Enable for a few seconds
        float timer = 0f;
        while (timer < hitboxDuration)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        Destroy(hitbox);
        yield return new WaitForSeconds(hitboxCooldown);
        canSpawnHitbox = true; // Allow spawning another hitbox after cooldown
    }

    public class HitboxGizmo : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, GetComponent<BoxCollider>().size);
        }
    }

    Vector3 CalculateLobVelocity(Vector3 start, Vector3 end, float speed)
    {
        Vector3 toTarget = end - start;
        Vector3 toTargetXZ = new Vector3(toTarget.x, 0, toTarget.z);

        float y = toTarget.y;
        float xz = toTargetXZ.magnitude;

        float gravity = Mathf.Abs(Physics.gravity.y);
        float speedSquared = speed * speed;

        float underSqrt = speedSquared * speedSquared - gravity * (gravity * xz * xz + 2 * y * speedSquared);

        if (underSqrt < 0)
        {
            // No solution, target is too far or too high for this speed
            return toTarget.normalized * speed;
        }

        float sqrt = Mathf.Sqrt(underSqrt);

        // Two possible angles. We'll use the lower one (faster, flatter shot)
        float angle = Mathf.Atan2(speedSquared - sqrt, gravity * xz);

        Vector3 velocity = toTargetXZ.normalized * speed * Mathf.Cos(angle);
        velocity.y = speed * Mathf.Sin(angle);

        return velocity;
    }

    bool IsPlayerInProjectileRange(Vector3 start, Vector3 end, float speed)
    {
        float gravity = Mathf.Abs(Physics.gravity.y);
        float maxRange = (speed * speed) / gravity;
        Vector3 toTargetXZ = new Vector3(end.x - start.x, 0, end.z - start.z);
        float distanceXZ = toTargetXZ.magnitude;
        return distanceXZ <= maxRange;
    }

    void callShotFire()
    {
        Vector3 spawnPos = projectileSpawnPoint != null ? projectileSpawnPoint.position : transform.position + Vector3.up;
        Vector3 targetPos = player.position;
        if (Time.time - lastShot >= shotCooldown && IsPlayerInProjectileRange(spawnPos, targetPos, weaponData.fodderSpeed)) {
            ShootAtPlayer();
        }
    }
}
