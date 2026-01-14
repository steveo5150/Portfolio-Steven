using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "Scriptable Objects/PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    public float speed = 5.0f;
    public float jumpHeight = 2.0f;
    public float gravity = -9.81f;
    public float mouseSensitivity = 200.0f; // Sensitivity of mouse movement
    public int maxHealth = 100; // Player health
    public int difficultyLevel = 1; // Difficulty level of the game
    public float dashDistance = 2.0f; // Distance for dash movement
    public float dashCooldown = 1.0f; // Cooldown time for dash
    public float dashDuration = 0.1f; // Duration of the dash effect
    public int maxDashCharges = 2; // Number of charges for dash
    public int maxJumps = 2; // Maximum number of jumps (for double jump)
    public float fastFallVelocity = -10.0f; // Velocity for fast fall
}
