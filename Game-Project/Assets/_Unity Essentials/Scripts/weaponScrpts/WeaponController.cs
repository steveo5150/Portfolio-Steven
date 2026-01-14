using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public GameObject[] weapons; // Assign FistCode, GunCode, etc. in Inspector
    private int currentWeapon = 0;

    void Start()
    {
        EquipWeapon(currentWeapon);
    }

    void Update()
    {
        // Example: 1 for Fist, 2 for Gun
        if (Input.GetKeyDown(KeyCode.Alpha1))
            EquipWeapon(0); //fist
        if (Input.GetKeyDown(KeyCode.Alpha2))
            EquipWeapon(1); //Pistol
    }

    void EquipWeapon(int index)
    {
        if (index > weapons.Length)
        {
            return;
        }
        for (int i = 0; i < weapons.Length; i++)
        {
            weapons[i].SetActive(i == index);
        }
        currentWeapon = index;
    }
}