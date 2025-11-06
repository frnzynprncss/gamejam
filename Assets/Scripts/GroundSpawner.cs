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
    public float startGroundSpeed = 4f;
    public float minGroundLength = 3f;
    public float maxGroundLength = 8f;
    public Vector3 spawnPosition;

    [Header("Progressive Difficulty")]
    public float speedIncreaseInterval = 10f;
    public float speedIncreaseAmount = 0.5f;
    public float maxMaxSpeed = 12f;

    [Header("Object Spawning")]
    public SpawnableObject[] spawnPrefabs;
    public float spawnChance = 0.4f;
    [Tooltip("Minimum distance from the edge of the platform to spawn an obstacle.")]
    public float objectEdgePadding = 0.5f;

    [Header("Height Variation")]
    [Tooltip("The actual offsets (positive or negative) from the baseYPosition when a random height is chosen.")]
    public float[] heightOffsets = { -2f, -1f, 0f, 1f, 2f };
    public float normalHeightChance = 0.7f;

    [Header("Civilian Spawning Control")]
    [Tooltip("Chance (0.0 to 1.0) that a civilian will be spawned on a new platform.")]
    [Range(0.0f, 1.0f)]
    public float civilianSpawnChance = 0.6f;

    // Timer variables
    private float timer;
    private float gameTime = 0f;
    private float currentGroundSpeed;
    private float baseYPosition;
    private bool shouldSpawnCivilianOnNextGround = false;

    private PasaheroSpawner civilianSpawner;

    [System.Serializable]
    public class SpawnableObject
    {
        public GameObject prefab;
        [Tooltip("Additional vertical offset added to the calculated spawn height.")]
        public float heightOffset = 1f;
    }

    void Start()
    {
        civilianSpawner = FindObjectOfType<PasaheroSpawner>();
        if (civilianSpawner == null)
        {
            Debug.LogError("GroundSpawner: Could not find PasaheroSpawner in the scene. Civilians will not be parented correctly.");
        }

        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        currentGroundSpeed = startGroundSpeed;
        baseYPosition = transform.position.y;

        SpawnInitialGround();

        shouldSpawnCivilianOnNextGround = (Random.value <= civilianSpawnChance);
    }

    void SpawnInitialGround()
    {
        Vector3 startPos = new Vector3(0f, baseYPosition, transform.position.z);

        GameObject initialGround = Instantiate(groundPrefab, startPos, Quaternion.identity);

        MovingGround movingGround = initialGround.GetComponent<MovingGround>();
        if (movingGround != null)
        {
            movingGround.moveSpeed = currentGroundSpeed;
        }

        float fixedLength = maxGroundLength;
        Vector3 originalScale = initialGround.transform.localScale;
        initialGround.transform.localScale = new Vector3(fixedLength, originalScale.y, originalScale.z);

        if (civilianSpawner != null)
        {
            civilianSpawner.SetGroundParent(initialGround.transform);
        }
    }


    void Update()
    {
        gameTime += Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f)
        {          
            shouldSpawnCivilianOnNextGround = (Random.value <= civilianSpawnChance);

            SpawnGround();
            timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
        UpdateDifficulty();
    }

    void SpawnGround()
    {
        float randomX = Random.Range(minSpawnX, maxSpawnX);
        float randomY = GetRandomHeight();
        Vector3 spawnPosition = new Vector3(randomX, randomY, transform.position.z);

        GameObject newGround = Instantiate(groundPrefab, spawnPosition, Quaternion.identity);

        MovingGround movingGround = newGround.GetComponent<MovingGround>();
        if (movingGround != null)
        {
            movingGround.moveSpeed = currentGroundSpeed;
        }

        RandomizeGroundWidth(newGround);

        if (civilianSpawner != null)
        {
            civilianSpawner.SetGroundParent(newGround.transform);

            if (shouldSpawnCivilianOnNextGround)
            {
                civilianSpawner.SpawnCiviliansOnGround();
            }
        }


        TrySpawnObjectOnGround(newGround);
    }

    float GetRandomHeight()
    {
        if (Random.value <= normalHeightChance)
        {
            return baseYPosition;
        }
        else
        {
            if (heightOffsets.Length > 0)
            {
                return baseYPosition + heightOffsets[Random.Range(0, heightOffsets.Length)];
            }
            else
            {
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
        if (Random.value <= spawnChance && spawnPrefabs != null && spawnPrefabs.Length > 0)
        {
            SpawnableObject spawnable = spawnPrefabs[Random.Range(0, spawnPrefabs.Length)];

            if (spawnable.prefab != null)
            {
                Renderer groundRenderer = groundObject.GetComponent<Renderer>();
                if (groundRenderer == null) return;

                Vector3 groundPosition = groundObject.transform.position;
                float groundHeight = groundRenderer.bounds.size.y;

                float surfaceY = groundPosition.y + (groundHeight / 2);

                float platformWidth = groundObject.transform.localScale.x;
                float minXOffset = -platformWidth / 2 + objectEdgePadding;
                float maxXOffset = platformWidth / 2 - objectEdgePadding;

                float randomXOffset = Random.Range(minXOffset, maxXOffset);

                Vector3 spawnPos = new Vector3(
                    groundPosition.x + randomXOffset,
                    surfaceY + spawnable.heightOffset,
                    groundPosition.z);

                GameObject spawnedObject = Instantiate(spawnable.prefab, spawnPos, Quaternion.identity);

                MovingGround objectMovingScript = spawnedObject.GetComponent<MovingGround>();
                if (objectMovingScript == null)
                {
                    objectMovingScript = spawnedObject.AddComponent<MovingGround>();
                }
                objectMovingScript.moveSpeed = currentGroundSpeed;
            }
        }
    }

    void UpdateDifficulty()
    {
        int speedIncreases = Mathf.FloorToInt(gameTime / speedIncreaseInterval);

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
