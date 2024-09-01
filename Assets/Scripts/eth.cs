using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class eth : MonoBehaviour
{
    // This method is called when another collider enters this object's collider (if it's set as a trigger)
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the object that collided with the eth is the player
        if (other.CompareTag("Player"))
        {
            // Destroy this eth object
            Destroy(gameObject);
        }
    }
}