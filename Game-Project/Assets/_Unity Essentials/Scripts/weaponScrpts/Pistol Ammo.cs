using UnityEngine;

public class PistolAmmo : MonoBehaviour
{
    public int ammoAmount = 20; // Amount of ammo this pickup restores

    private void OnTriggerEnter(Collider other)
    {
        // Check if the player picked up the ammo
        if (other.CompareTag("Player"))
        {
            // Find the pistolCode script on the player or their weapon
            pistolCode pistol = other.GetComponentInChildren<pistolCode>();
            if (pistol != null && pistol.currentAmmo < pistol.maxAmmo)
            {
                pistol.AddAmmo(ammoAmount);

                // Optionally, play a pickup sound or effect here

                // Destroy the pickup object
                Destroy(gameObject);
            }
        }
    }
}