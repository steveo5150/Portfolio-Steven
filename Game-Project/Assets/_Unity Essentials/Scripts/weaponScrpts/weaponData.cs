using UnityEngine;

[CreateAssetMenu(fileName = "weaponData", menuName = "Scriptable Objects/weaponData")]
public class weaponData : ScriptableObject
{
    public int fistDamage = 100; // Damage dealt by the player
    public float fistKnockbackForce = 25.0f; // Knockback force applied by the player's fist
    public int fistAmmo = 2; // Ammo for the player's fist (can be thought of as a cooldown or charge system)
    public int fistNoAmmoDamage = 5; // Damage dealt when the fist is out of ammo
    public int pistolAmmo = 100;
    public float pistolRange = 100.0f; // Range of the pistol shot
    public int pistolDamage = 10; // Damage dealt by the pistol
    public int fodderDamage = 5; // Damage dealt by the fodder enemy
    public float fodderKnockbackForce = 5.0f; // Knockback force applied by the fodder enemy
    public float fodderSpeed = 25.0f; // Speed of the fodder shot

}
