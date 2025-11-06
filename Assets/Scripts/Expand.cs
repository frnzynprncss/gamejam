using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expand : MonoBehaviour
{
    [Header("Settings")]
    public float expandForce = 5f;            // Force applied to each circle
    public float returnSpeed = 5f;            // Speed to return to original position
    public GameObject[] circles;              // Circles connected by SpringJoint2D
    private Vector2[] originalPositions;      // Store original positions

    void Start()
    {
        // Store original positions of all circles
        if (circles != null && circles.Length > 0)
        {
            originalPositions = new Vector2[circles.Length];
            for (int i = 0; i < circles.Length; i++)
            {
                originalPositions[i] = circles[i].transform.position;
            }
        }
    }

    void Update()
    {
        if (circles == null || circles.Length == 0) return;

        if (Input.GetKey(KeyCode.Space))
        {
            Explode();
        }
        else
        {
            ReturnToOriginal();
        }
    }

    void Explode()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            if (circles[i] == null) continue;

            // Direction from center (this object) to circle
            Vector2 direction = ((Vector2)circles[i].transform.position - (Vector2)transform.position).normalized;

            // Apply force outward using Rigidbody2D
            Rigidbody2D rb = circles[i].GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.AddForce(direction * expandForce);
            }
        }
    }

    void ReturnToOriginal()
    {
        for (int i = 0; i < circles.Length; i++)
        {
            if (circles[i] == null) continue;

            // Smoothly move back to original position
            circles[i].transform.position = Vector2.MoveTowards(
                circles[i].transform.position,
                originalPositions[i],
                returnSpeed * Time.deltaTime
            );
        }
    }
}
