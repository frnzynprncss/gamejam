using UnityEngine;

public class VehicleConnector : MonoBehaviour
{
    [Header("Wheel Settings")]
    public Transform[] wheels;       // Assign wheel GameObjects here in Inspector
    public Vector2 attachOffset = Vector2.zero; // Offset for positioning body above wheels

    [Header("Physics Settings")]
    public bool usePhysics = true;   // If true, body follows wheels with physics
    public float followSpeed = 10f;  // How quickly the body follows wheels (if physics = false)

    [Header("Tilt Settings")]
    public float maxTiltAngle = 15f; // Maximum tilt angle (degrees)
    public float tiltSpeed = 5f;     // How smoothly body tilts

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        if (wheels == null || wheels.Length == 0) return;

        // --- POSITION ---
        Vector2 avgPos = Vector2.zero;
        foreach (Transform wheel in wheels)
        {
            avgPos += (Vector2)wheel.position;
        }
        avgPos /= wheels.Length;

        Vector2 targetPos = avgPos + attachOffset;

        if (usePhysics && rb != null)
        {
            Vector2 newPos = Vector2.Lerp(rb.position, targetPos, Time.fixedDeltaTime * followSpeed);
            rb.MovePosition(newPos);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.fixedDeltaTime * followSpeed);
        }

        // --- ROTATION (LIMITED TILT) ---
        if (wheels.Length >= 2)
        {
            // Calculate angle between first and last wheel
            Vector2 wheelDir = wheels[wheels.Length - 1].position - wheels[0].position;
            float targetAngle = Mathf.Atan2(wheelDir.y, wheelDir.x) * Mathf.Rad2Deg;

            // Clamp to max tilt
            targetAngle = Mathf.Clamp(targetAngle, -maxTiltAngle, maxTiltAngle);

            // Smoothly rotate body
            float smoothAngle = Mathf.LerpAngle(transform.eulerAngles.z, targetAngle, Time.fixedDeltaTime * tiltSpeed);

            if (usePhysics && rb != null)
            {
                rb.MoveRotation(smoothAngle);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, smoothAngle);
            }
        }
    }
}
