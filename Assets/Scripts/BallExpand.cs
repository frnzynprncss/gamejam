using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallExpand : MonoBehaviour
{
    [Header("Settings")]
    public GameObject circlePrefab;   // Prefab for each small circle
    public int circleCount = 10;      // Number of circles
    public float radius = 2f;         // Default distance from center
    public float expandedRadius = 4f; // Distance when space is pressed
    public float expandSpeed = 5f;    // How fast it expands/contracts

    private List<GameObject> circles = new List<GameObject>();
    private List<SpringJoint2D> playerSprings = new List<SpringJoint2D>(); // only springs to player
    private float currentRadius;
    private Rigidbody2D playerRb;

    void Start()
    {
        currentRadius = radius;

        // Get the player's Rigidbody2D
        playerRb = GetComponent<Rigidbody2D>();
        if (playerRb == null)
        {
            Debug.LogError("BallExpand must be attached to a GameObject with a Rigidbody2D!");
            return;
        }

        SpawnCircles();
    }

    void Update()
    {
        if (playerSprings.Count == 0) return;

        // Check if space is held
        float targetRadius = Input.GetKey(KeyCode.Space) ? expandedRadius : radius;

        // Smooth transition
        currentRadius = Mathf.MoveTowards(currentRadius, targetRadius, expandSpeed * Time.deltaTime);

        // Update ONLY the springs to the player
        foreach (SpringJoint2D spring in playerSprings)
        {
            spring.distance = currentRadius;
        }
    }

    void SpawnCircles()
    {
        circles.Clear();
        playerSprings.Clear();

        for (int i = 0; i < circleCount; i++)
        {
            float angle = i * Mathf.PI * 2f / circleCount;
            Vector2 spawnPos = (Vector2)transform.position + new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius;

            GameObject c = Instantiate(circlePrefab, spawnPos, Quaternion.identity, transform); // spawn as child
            Rigidbody2D rb = c.GetComponent<Rigidbody2D>();
            if (rb == null) rb = c.AddComponent<Rigidbody2D>();

            circles.Add(c);

            // Connect circle to player
            SpringJoint2D springToPlayer = c.AddComponent<SpringJoint2D>();
            springToPlayer.connectedBody = playerRb;
            springToPlayer.autoConfigureDistance = false;
            springToPlayer.distance = radius;
            springToPlayer.dampingRatio = 0.8f;
            springToPlayer.frequency = 4f;

            playerSprings.Add(springToPlayer);
        }

        // Connect each circle to both neighbors (keep constant distance)
        for (int i = 0; i < circleCount; i++)
        {
            GameObject current = circles[i];

            // Next neighbor
            GameObject next = circles[(i + 1) % circleCount];
            Rigidbody2D rbNext = next.GetComponent<Rigidbody2D>();
            SpringJoint2D springToNext = current.AddComponent<SpringJoint2D>();
            springToNext.connectedBody = rbNext;
            springToNext.autoConfigureDistance = false;
            springToNext.distance = Vector2.Distance(current.transform.position, next.transform.position);
            springToNext.dampingRatio = 0.8f;
            springToNext.frequency = 4f;

            // Previous neighbor
            GameObject prev = circles[(i - 1 + circleCount) % circleCount];
            Rigidbody2D rbPrev = prev.GetComponent<Rigidbody2D>();
            SpringJoint2D springToPrev = current.AddComponent<SpringJoint2D>();
            springToPrev.connectedBody = rbPrev;
            springToPrev.autoConfigureDistance = false;
            springToPrev.distance = Vector2.Distance(current.transform.position, prev.transform.position);
            springToPrev.dampingRatio = 0.8f;
            springToPrev.frequency = 4f;
        }
    }
}
