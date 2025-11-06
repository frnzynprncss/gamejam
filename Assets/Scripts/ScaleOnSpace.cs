using UnityEngine;

public class ScaleOnSpace : MonoBehaviour
{
    [Header("Spring Expansion Settings")]
    public float maxDistance = 5f;       // Max expansion distance
    public float returnSpeed = 2f;       // How fast springs return automatically
    public float cooldownTime = 2f;      // Cooldown time before next expansion
    public float holdTime = 0.5f;        // How long to stay expanded

    [Header("Anti-Collapse Settings")]
    public float upwardForce = 10f;      // Force applied when colliding to push up
    public float minY = -5f;             // Safety floor clamp (don’t fall below this Y)
    public float maxY = 5f;              // Maximum height allowed for the wheel

    private SpringJoint2D[] springs;
    private float[] originalDistances;
    private bool isExpanded = false;     // Tracks if currently expanded
    private float cooldownTimer = 0f;    // Countdown timer
    private float holdTimer = 0f;        // Timer for automatic shrinking
    private Rigidbody2D rb;              // Rigidbody of parent

    void Start()
    {
        // Collect all SpringJoint2D components in children
        springs = GetComponentsInChildren<SpringJoint2D>();

        // Save the original distances
        originalDistances = new float[springs.Length];
        for (int i = 0; i < springs.Length; i++)
        {
            originalDistances[i] = springs[i].distance;
        }

        // Cache the parent Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>();
        }
    }

    void Update()
    {
        // Update cooldown timer
        if (cooldownTimer > 0f)
        {
            cooldownTimer -= Time.deltaTime;
        }

        // Expansion on Left Mouse Click
        if (Input.GetMouseButtonDown(0) && !isExpanded && cooldownTimer <= 0f)
        {
            for (int i = 0; i < springs.Length; i++)
            {
                springs[i].distance = maxDistance;
            }
            isExpanded = true;
            holdTimer = holdTime; // Start hold timer
            cooldownTimer = cooldownTime;
        }

        // Automatic shrinking back
        if (isExpanded)
        {
            // Smoothly return springs
            for (int i = 0; i < springs.Length; i++)
            {
                springs[i].distance = Mathf.Lerp(
                    springs[i].distance,
                    originalDistances[i],
                    Time.deltaTime * returnSpeed
                );
            }

            // Countdown hold timer
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0f)
            {
                isExpanded = false; // Done expanding, fully shrinking automatically
            }
        }

        // Safety check: if wheel falls too low, push it up
        if (transform.position.y < minY)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f); // stop downward momentum
            rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
        }

        // Clamp max upward position
        if (transform.position.y > maxY)
        {
            transform.position = new Vector3(transform.position.x, maxY, transform.position.z);
            rb.velocity = new Vector2(rb.velocity.x, 0f); // stop upward motion
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // If colliding with solid, give upward push to prevent collapse
        if (collision.collider != null && rb != null)
        {
            if (transform.position.y < maxY)
            {
                rb.velocity = new Vector2(rb.velocity.x, 0f); // reset falling velocity
                rb.AddForce(Vector2.up * upwardForce, ForceMode2D.Impulse);
            }
        }
    }
}
