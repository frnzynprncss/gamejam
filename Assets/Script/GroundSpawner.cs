using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GroundSpawner : MonoBehaviour
{
    public GameObject groundPrefab;
    public float minSpawnInterval = 1f;
    public float maxSpawnInterval = 3f;
    public float minSpawnX = 12f;
    public float maxSpawnX = 18f;
    public float startGroundSpeed = 4f; // All ground starts with this speed
    public float minGroundLength = 3f;
    public float maxGroundLength = 8f;
    public Vector3 spawnPosition;

    [Header("Progressive Difficulty")]
    public float speedIncreaseInterval = 10f; // First increase at 10 seconds
    public float speedIncreaseAmount = 0.5f; // Speed increase each interval
    public float maxMaxSpeed = 12f; // Maximum speed cap

    [Header("Object Spawning")]
    public SpawnableObject[] spawnPrefabs; // Now uses custom class with offset
    public float spawnChance = 0.4f; // 40% chance to spawn an object
    [Tooltip("Minimum distance from the edge of the platform to spawn an obstacle.")]
    public float objectEdgePadding = 0.5f;

    [Header("Height Variation")]
    [Tooltip("The actual offsets (positive or negative) from the baseYPosition when a random height is chosen.")]
    public float[] heightOffsets = { -2f, -1f, 0f, 1f, 2f }; // Different height levels/offsets
    public float normalHeightChance = 0.7f; // 70% chance for normal height (baseYPosition)

    // Timer variables
    private float timer;
    private float gameTime = 0f;
    private float currentGroundSpeed; // Single speed value for all ground
    private float baseYPosition; // Original Y position of the spawner (the default ground height)

    // NEW: Reference to the civilian spawner
    private PasaheroSpawner civilianSpawner;


    // GroundSpawner.cs
    // ...
    
// ...
// TELL THE CIVILIAN SPAWNER WHICH NEW GROUND TO USE AS PARENT

// ...


    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        [Tooltip("Additional vertical offset added to the calculated spawn height.")]
        public float heightOffset = 1f; // Individual offset for each prefab
    }

    void Start()
    {
        // FIND THE CIVILIAN SPAWNER AND CACHE THE REFERENCE
        civilianSpawner = FindObjectOfType<PasaheroSpawner>();
        if (civilianSpawner == null)
        {
            Debug.LogError("GroundSpawner: Could not find CivilianSpawner in the scene. Civilians will not be parented correctly.");
        }

        GameObject newGround = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);
        if (civilianSpawner != null)
        {
            civilianSpawner.SetGroundParent(newGround.transform); // <-- Passes the new ground transform
        }

        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        currentGroundSpeed = startGroundSpeed;
        baseYPosition = transform.position.y; // Store the original Y position

        // Initial spawn to give the player a starting platform
        SpawnGround();
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        // Handle ground spawning
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnGround();
            timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        // Update speed based on game time
        UpdateDifficulty();
    }

    void SpawnGround()
    {
        float randomX = Random.Range(minSpawnX, maxSpawnX);
        float randomY = GetRandomHeight(); // Get random height based on weighted chance
        Vector3 spawnPosition = new Vector3(randomX, randomY, transform.position.z);

        GameObject newGround = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);

        // All ground gets the same current speed
        GroundMoving movingGround = newGround.GetComponent<GroundMoving>();
        if (movingGround != null)
        {
            movingGround.moveSpeed = currentGroundSpeed;
        }

        // Apply width randomization
        RandomizeGroundWidth(newGround);

        // TELL THE CIVILIAN SPAWNER WHICH NEW GROUND TO USE AS PARENT
        if (civilianSpawner != null)
        {
            civilianSpawner.SetGroundParent(newGround.transform);
        }

        // Try to spawn an obstacle on top
        TrySpawnObjectOnGround(newGround);
    }

    /// <summary>
    /// Returns the Y position for the ground based on the weighted chance.
    /// </summary>
    float GetRandomHeight()
    {
        // 70% chance for normal height, 30% chance for a height offset from the array
        if (Random.value <= normalHeightChance)
        {
            // Return base position (most common height)
            return baseYPosition;
        }
        else
        {
            // Choose a random offset from the heightOffsets array
            if (heightOffsets.Length > 0)
            {
                // Note: Since heightOffsets contains negative values, this handles lowered platforms too
                return baseYPosition + heightOffsets[Random.Range(0, heightOffsets.Length)];
            }
            else
            {
                // Fallback to base position if array is empty
                return baseYPosition;
            }
        }
    }

    void RandomizeGroundWidth(GameObject groundObject)
    {
        float randomLength = Random.Range(minGroundLength, maxGroundLength);
        Vector3 originalScale = groundObject.transform.localScale;
        groundObject.transform.localScale = new Vector3(randomLength, originalScale.y, originalScale.z);
    }

    void TrySpawnObjectOnGround(GameObject groundObject)
    {
        // Check if we should spawn an object (40% chance) and if we have prefabs
        if (Random.value <= spawnChance && spawnPrefabs != null && spawnPrefabs.Length > 0)
        {
            // 1. Choose a random spawnable object from the array
            SpawnableObject spawnable = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

            if (spawnable.prefab != null)
            {
                // 2. Calculate the base Y position (top surface of the ground)
                Renderer groundRenderer = groundObject.GetComponent<Renderer>();
                if (groundRenderer == null) return; // Exit if no renderer for bounds calculation

                Vector3 groundPosition = groundObject.transform.position;
                float groundHeight = groundRenderer.bounds.size.y;

                // Calculate the surface Y position of the platform
                float surfaceY = groundPosition.y + (groundHeight / 2);

                // 3. Randomize the X position along the ground platform's width
                float platformWidth = groundObject.transform.localScale.x;
                float minXOffset = -platformWidth / 2 + objectEdgePadding;
                float maxXOffset = platformWidth / 2 - objectEdgePadding;

                // Ensure minX is less than maxX in case padding is too large
                float randomXOffset = Random.Range(minXOffset, maxXOffset);

                Vector3 spawnPos = new Vector3(
                    groundPosition.x + randomXOffset, // Apply the random X offset
                    surfaceY + spawnable.heightOffset, // Apply object's specific offset
                    groundPosition.z);

                // 4. Spawn the object
                GameObject spawnedObject = Instantiate(spawnable.prefab, spawnPos, Quaternion.identity);

                // 5. Synchronize movement speed
                GroundMoving objectMovingScript = spawnedObject.GetComponent<GroundMoving>();
                if (objectMovingScript == null)
                {
                    objectMovingScript = spawnedObject.AddComponent<GroundMoving>();
                }
                objectMovingScript.moveSpeed = currentGroundSpeed;
            }
        }
    }

    void UpdateDifficulty()
    {
        // Calculate how many speed increases have occurred
        int speedIncreases = Mathf.FloorToInt(gameTime / speedIncreaseInterval);

        // Apply speed increases with cap
        float newSpeed = startGroundSpeed + (speedIncreases * speedIncreaseAmount);
        currentGroundSpeed = Mathf.Min(newSpeed, maxMaxSpeed);
    }

    public void ResetGame()
    {
        gameTime = 0f;
        currentGroundSpeed = startGroundSpeed;
        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}
