using UnityEngine;

public class healthManager : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;
    public playerUI playerUI; // Reference to player UI

    void Start()
    {
        currentHealth = maxHealth;
        UpdateUI();
    }

    public void TakeDamage(int amount, bool isPlayer)
    {
        currentHealth -= amount;
        Debug.Log("did: " + amount);
        if (isPlayer)
        {
            UpdateUI();
        }
        if (currentHealth <= 0)
        {
            // Handle death (destroy, play animation, etc.)
            if (!isPlayer)
            {
                GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
                if (playerObj != null)
                {
                    PlayerController pc = playerObj.GetComponent<PlayerController>();
                    if (pc != null)
                    {
                        pc.RefillDashCharge();
                        Debug.Log("Enemy killed, refilling dash charge.");
                    }
                }
                Destroy(gameObject);
            }
        }
    }

    public void setMaxHealth(int amount)
    {
        currentHealth = amount;
        maxHealth = amount;
        UpdateUI();
    }

    public int GetHealth()
    {
        return currentHealth;
    }

    private void UpdateUI()
    {
        if (playerUI != null)
            playerUI.SetUIHealth(currentHealth, maxHealth);
    }
}