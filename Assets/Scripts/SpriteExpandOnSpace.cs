using UnityEngine;

public class SpriteExpandOnClick : MonoBehaviour
{
    [Header("Expansion Settings")]
    public Vector3 expandedScale = new Vector3(2f, 2f, 1f); // Target scale when expanded
    public float expandSpeed = 5f;  // How fast it expands
    public float returnSpeed = 5f;  // How fast it returns automatically
    public float holdTime = 0.5f;   // How long to stay expanded

    private Vector3 originalScale;
    private bool isExpanded = false; // Tracks if currently expanded
    private float holdTimer = 0f;

    void Start()
    {
        // Save the starting scale
        originalScale = transform.localScale;
    }

    void Update()
    {
        // Left mouse click triggers expansion
        if (Input.GetMouseButtonDown(0) && !isExpanded)
        {
            isExpanded = true;
            holdTimer = holdTime; // Start hold timer
        }

        // Handle expansion
        if (isExpanded)
        {
            // Smoothly scale up
            transform.localScale = Vector3.Lerp(transform.localScale, expandedScale, Time.deltaTime * expandSpeed);

            // Countdown hold time
            holdTimer -= Time.deltaTime;
            if (holdTimer <= 0f)
            {
                isExpanded = false; // Start automatic return
            }
        }
        else
        {
            // Automatically shrink back to original size
            transform.localScale = Vector3.Lerp(transform.localScale, originalScale, Time.deltaTime * returnSpeed);
        }
    }
}
