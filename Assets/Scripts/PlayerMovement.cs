using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f; // Speed at which the player moves

    private Rigidbody playerRigidbody;

    void Start()
    {
        // Get the Rigidbody component from the player GameObject
        playerRigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        // Get input from the horizontal axis (left and right arrow keys, "A" and "D")
        float moveHorizontal = Input.GetAxis("Horizontal");

        // Calculate the movement vector
        Vector3 movement = new Vector3(moveHorizontal, 0.0f, 0.0f);

        // Apply the movement to the player's Rigidbody
        playerRigidbody.MovePosition(transform.position + movement * speed * Time.fixedDeltaTime);
    }
}