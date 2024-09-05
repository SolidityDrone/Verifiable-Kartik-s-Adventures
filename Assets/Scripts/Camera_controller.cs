using UnityEngine;

public class Camera_controller : MonoBehaviour
{
    public Transform target;
    public float baseSpeed = 8.5f; 
    public float catchupSpeed = 12.0f;
    public float max_distance = 5.0f; 

    void Update()
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
