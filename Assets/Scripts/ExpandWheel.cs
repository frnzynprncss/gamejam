using UnityEngine;

public class ExpandWheel : MonoBehaviour
{
    [Header("Wheel Expansion Settings")]
    public float expandMultiplier = 1.5f; // How much bigger wheels get
    public float expandSpeed = 5f;        // How fast they scale up
    public float returnSpeed = 5f;        // How fast they return automatically
    public float cooldownTime = 2f;       // Cooldown before expanding again
    public float holdTime = 0.5f;         // How long to stay expanded

    [Header("Flip Settings")]
    public float flipTorque = 300f;       // Torque force for backflip

    private Transform[] wheels;           // Wheels (children)
    private Vector3[] originalScales;     // Save default sizes
    private Vector3[] targetScales;       // Expanded target sizes
    private bool isExpanded = false;
    private float cooldownTimer = 0f;
    private float holdTimer = 0f;

    private Rigidbody2D rb;               // Rigidbody for whole car

    void Start()
    {
        // Cache Rigidbody
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody2D>(); // Auto add if missing
        }

        // Collect all child objects (assumed wheels)
        wheels = GetComponentsInChildren<Transform>();

        originalScales = new Vector3[wheels.Length];
        targetScales = new Vector3[wheels.Length];

        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i] != this.transform) // Ignore parent (vehicle body)
            {
                originalScales[i] = wheels[i].localScale;
                targetScales[i] = originalScales[i] * expandMultiplier;
            }
        }
    }

    void Update()
    {
        // Handle cooldown
        if (cooldownTimer > 0f)
            cooldownTimer -= Time.deltaTime;

        // Trigger expansion with left mouse click (no holding)
        if (Input.GetMouseButtonDown(0) && !isExpanded && cooldownTimer <= 0f)
        {
            isExpanded = true;
            holdTimer = holdTime;
            cooldownTimer = cooldownTime;

            // Apply backflip torque to whole car
            rb.AddTorque(flipTorque, ForceMode2D.Impulse);
        }

        // Expand wheels
        for (int i = 0; i < wheels.Length; i++)
        {
            if (wheels[i] != this.transform)
            {
                if (isExpanded)
                {
                    // Smoothly scale up
                    wheels[i].localScale = Vector3.Lerp(
                        wheels[i].localScale,
                        targetScales[i],
                        Time.deltaTime * expandSpeed
                    );
                }
                else
                {
                    // Automatically shrink back
                    wheels[i].localScale = Vector3.Lerp(
                        wheels[i].localScale,
                        originalScales[i],
                        Time.deltaTime * returnSpeed
                    );
                }
            }
        }

        // Countdown hold time
        if (isExpanded)
        {
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0f)
            {
                isExpanded = false; // Stop expanding, wheels return automatically
            }
        }
    }
}
