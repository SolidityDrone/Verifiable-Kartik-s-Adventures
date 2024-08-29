using UnityEngine;

public class Camera_controller : MonoBehaviour
{
    public Transform target; // The target (player character) that the camera should follow
    public float baseSpeed = 8.5f; // The fixed speed at which the camera moves horizontally
    public float catchupSpeed = 12.0f; // The speed at which the camera catches up to the player
    public float max_distance = 5.0f; // The maximum x-axis distance before increasing speed

    void FixedUpdate()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not assigned.");
            return;
        }

        // Calculate the distance between the camera and the player on the x-axis
        float xDistance = Mathf.Abs(transform.position.x - target.position.x);

        // Determine the speed to use based on distance
        float currentSpeed = (xDistance > max_distance) ? catchupSpeed : baseSpeed;

        // Move the camera horizontally at the determined speed
        Vector3 newPosition = new Vector3(transform.position.x + currentSpeed * Time.deltaTime, transform.position.y, transform.position.z);
        transform.position = newPosition;
    }
}
