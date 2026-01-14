using UnityEngine;

public class pistolCode : MonoBehaviour
{
    public Transform firePoint; // Where the ray starts (assign gun barrel or camera in Inspector)
    public Transform cosmeticFirePoint; // Where the tracer starts
    public LayerMask hitMask; // What layers the pistol can hit (set in Inspector)
    public int maxAmmo = 100; // Maximum ammo in the pistol
    public int currentAmmo; // Current ammo in the pistol
    public weaponData weaponData; // Reference to weaponData ScriptableObject
    public playerUI playerUI; // Reference to player UI script

    //cosmetic tracer settings
    public GameObject tracerPrefab; // Assign your LineRenderer prefab in the Inspector
    public float tracerDuration = 0.05f; // How long the tracer is visible

    public GameObject cosmeticBulletPrefab; // Assign in Inspector
    public float cosmeticBulletSpeed = 200f; // Units per second


    void Start()
    {
        // Initialize current ammo to max ammo
        currentAmmo = weaponData.pistolAmmo;
        playerUI.SetAmmo(currentAmmo, weaponData.pistolAmmo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && currentAmmo > 0) // Left mouse button
        {
            Shoot();
        }
    }

    void Shoot()
    {
        Transform origin = firePoint != null ? firePoint : transform;
        Transform cosmeticOrigin = cosmeticFirePoint != null ? cosmeticFirePoint : transform;
        Ray ray = new Ray(origin.position, origin.forward);
        RaycastHit hit;
        currentAmmo--; // Decrease ammo count
        playerUI.SetAmmo(currentAmmo, weaponData.pistolAmmo);
        Vector3 tracerEnd;
        if (Physics.Raycast(ray, out hit, weaponData.pistolRange, hitMask))
        {
            // Try to damage the hit object
            var health = hit.collider.GetComponent<healthManager>();
            if (health != null)
            {
                health.TakeDamage(weaponData.pistolDamage, false);
                //Debug.Log($"Hit {hit.collider.name} for {damage} damage.");
            }
            tracerEnd = hit.point;
            // Optional: Visual effect at hit point
            Debug.DrawLine(origin.position, hit.point, Color.red, 0.2f);
        }
        else
        {
            // Optional: Visualize the miss
            tracerEnd = origin.position + origin.forward * weaponData.pistolRange;
            Debug.DrawLine(origin.position, origin.position + origin.forward * weaponData.pistolRange, Color.gray, 0.2f);
        }
        SpawnCosmeticBullet(cosmeticOrigin.position, tracerEnd);

    }
    void OnEnable()
    {
        // Optionally reset or update ammo UI when the pistol is enabled
        if (playerUI != null && weaponData != null)
            playerUI.SetAmmo(currentAmmo, weaponData.pistolAmmo);
    }
    void SpawnTracer(Vector3 start, Vector3 end)
    {
        if (tracerPrefab == null) return;
        GameObject tracer = Instantiate(tracerPrefab, start, Quaternion.identity);
        LineRenderer lr = tracer.GetComponent<LineRenderer>();
        if (lr != null)
        {
            lr.SetPosition(0, start);
            lr.SetPosition(1, end);
        }
        Destroy(tracer, tracerDuration);
    }
    void SpawnCosmeticBullet(Vector3 start, Vector3 end)
    {
        if (cosmeticBulletPrefab == null) return;
        Vector3 direction = (end - start).normalized;
        Quaternion rotation = Quaternion.LookRotation(direction);
        GameObject bullet = Instantiate(cosmeticBulletPrefab, start, rotation);
        StartCoroutine(MoveCosmeticBullet(bullet, start, end));
    }
    public void AddAmmo(int amount)
    {
        currentAmmo = Mathf.Min(currentAmmo + amount, weaponData.pistolAmmo); // Clamp to max ammo
        if (playerUI != null)
            playerUI.SetAmmo(currentAmmo, weaponData.pistolAmmo);
    }
    System.Collections.IEnumerator MoveCosmeticBullet(GameObject bullet, Vector3 start, Vector3 end)
    {
        float distance = Vector3.Distance(start, end);
        float travelTime = distance / cosmeticBulletSpeed;
        float elapsed = 0f;
        while (elapsed < travelTime)
        {
            if (bullet == null) yield break;
            bullet.transform.position = Vector3.Lerp(start, end, elapsed / travelTime);
            bullet.transform.rotation = Quaternion.LookRotation(end - bullet.transform.position); // Keep it facing the right way not needed for cosmetic bullet with straight path

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (bullet != null)
            bullet.transform.position = end;
        Destroy(bullet);
    }
}