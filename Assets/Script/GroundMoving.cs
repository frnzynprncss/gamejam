using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundMoving : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the ground moves to the left

    // Update is called once per frame
    void Update()
    {
        // Move the ground to the left
        transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);

        // Check if the ground is out of screen and destroy it
        if (IsOutOfScreen())
        {
            // When the ground is destroyed, all its children (obstacles, civilians)
            // are also destroyed automatically, preventing memory leaks.
            Destroy(gameObject);
        }
    }

    // Check if the ground is outside the screen bounds
    private bool IsOutOfScreen()
    {
        // Get the right edge of the ground (assuming pivot is at center)
        float groundRightEdge = transform.position.x + (GetComponent<Renderer>().bounds.size.x / 2);

        // Convert to viewport coordinates
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(new Vector3(groundRightEdge, transform.position.y, transform.position.z));

        // Destroy when the right edge is off-screen to the left
        return screenPoint.x < -0.1f;
    }
}
