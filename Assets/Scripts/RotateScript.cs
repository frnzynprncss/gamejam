using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateScript : MonoBehaviour
{
    [Header("Wheel Rotation Settings")]
    public float rotationSpeed = 360f;   // Adjust how fast wheels rotate relative to move speed
    public string wheelTag = "Wheel";    // Tag all your wheel objects with "Wheel"

    private Movement movementScript;
    private List<Transform> wheels = new List<Transform>();

    void Start()
    {
        // Get reference to the parent Movement script
        movementScript = GetComponent<Movement>();

        // Find all wheels in the children with the given tag
        foreach (Transform child in GetComponentsInChildren<Transform>())
        {
            if (child.CompareTag(wheelTag))
            {
                wheels.Add(child);
            }
        }
    }

    void Update()
    {
        if (movementScript == null || wheels.Count == 0) return;

        // Rotate every wheel continuously
        foreach (Transform wheel in wheels)
        {
            wheel.Rotate(0f, 0f, -movementScript.moveSpeed * rotationSpeed * Time.deltaTime);
        }
    }
}
