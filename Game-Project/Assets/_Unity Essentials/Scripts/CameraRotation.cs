using UnityEngine;

public class CameraRotation : MonoBehaviour
{
    public PlayerSettings playerSettings; // Reference to PlayerSettings ScriptableObject
    public float yrotation = 0f; // Variable to store the current y rotation
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y") * playerSettings.mouseSensitivity * Time.deltaTime;
        yrotation -= mouseY; // Adjust the y rotation based on mouse movement
        yrotation = Mathf.Clamp(yrotation, -90f, 90f); // Clamp the rotation to prevent flipping
        transform.localRotation = Quaternion.Euler(yrotation, 0f, 0f); // Rotate camera based on mouse movement
    }
}
