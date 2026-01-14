using UnityEngine;

public class FistCode : MonoBehaviour
{
    public Vector3 hitboxSize = new Vector3(1f, 1f, 5f); // Size of the hitbox
    public float hitboxDuration = 0.4f; // Duration for which the hitbox is active
    public float hitboxCooldown = .5f; // Cooldown before the next hitbox can be spawned
    public weaponData weaponData; // Reference to weaponData ScriptableObject

    public playerUI playerUI; // Reference to player UI script

    //ammo
    private int currentAmmo; //current ammo
    private GameObject hitbox; // Reference to the currently active hitbox
    private bool canSpawnHitbox = true; // Flag to control hitbox spawning
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentAmmo = weaponData.fistAmmo; // Initialize current ammo to max ammo
        playerUI.SetAmmo(currentAmmo, weaponData.fistAmmo);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && canSpawnHitbox) // Left mouse button
        {
            StartCoroutine(SpawnHitbox());
        }
    }

    private System.Collections.IEnumerator SpawnHitbox()
    {
        canSpawnHitbox = false; // Prevent spawning another hitbox immediately
        // Create a new GameObject for the hitbox
        Transform cameraLocation = GameObject.FindGameObjectWithTag("MainCamera").transform;
        hitbox = new GameObject("DynamicHitbox");
        hitbox.tag = "fistHitBox";
        hitbox.transform.parent = cameraLocation;
        hitbox.transform.position = cameraLocation.position + cameraLocation.forward * 2; // 2 units in front of the character

        hitbox.transform.rotation = cameraLocation.rotation;

        // Add a BoxCollider (set as trigger)
        BoxCollider collider = hitbox.AddComponent<BoxCollider>();
        collider.size = hitboxSize;
        collider.isTrigger = true;

        // Add your custom logic script
        var logic = hitbox.AddComponent<FistHitboxLogic>();
        logic.owner = this;
        // (Optional) Add a visual for debugging
        hitbox.AddComponent<HitboxGizmo>();
        if (hasAmmo())
        {
            currentAmmo--; // Decrease ammo count
            playerUI.SetAmmo(currentAmmo, weaponData.fistAmmo);
        }
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
    public bool hasAmmo()
    {
        return currentAmmo > 0;
    }
    void OnEnable()
    {
        // Optionally reset or update ammo UI when the pistol is enabled
        if (playerUI != null && weaponData != null)
            playerUI.SetAmmo(currentAmmo, weaponData.fistAmmo);
    }
    void OnDisable()
    {
        if (hitbox != null)
        {
            Destroy(hitbox);
            hitbox = null;
        }
        canSpawnHitbox = true; // Reset so you can punch again when re-enabled
    }
    // Optional: Draws a visible box in the editor/game for debugging
    public class HitboxGizmo : MonoBehaviour
    {
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, GetComponent<BoxCollider>().size);
        }
    }
}
