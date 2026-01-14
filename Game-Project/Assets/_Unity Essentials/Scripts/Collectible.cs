using UnityEngine;

public class Collectible : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float rotationSpeed;
    public GameObject onCollectEffect;
    void Start()
    {

}

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0, rotationSpeed,0);
}
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Destroy the collectible when the player collides with it
            Destroy(gameObject);
            Instantiate(onCollectEffect, transform.position, transform.rotation);
        }
    }
}
