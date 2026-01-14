using UnityEngine;

public class FistHitboxLogic : MonoBehaviour
{
    public FistCode owner;

    void OnTriggerEnter(Collider other)
    {
        // Example: Only damage enemies
        if (other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            int damage = owner.hasAmmo() ? owner.weaponData.fistDamage : owner.weaponData.fistNoAmmoDamage;
            var health = other.GetComponent<healthManager>();
            if (health != null)
            {
                health.TakeDamage(damage, false);
            }
        }
    }
}