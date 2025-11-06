using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class PasaheroSpawner : MonoBehaviour
{
    [Header("Assets")]
    [Tooltip("Drag the civilian GameObject Prefab here.")]
    public GameObject civilianPrefab;

    [Tooltip("The Transform where civilians will be spawned (e.g., just off-screen to the right).")]
    public Transform spawnPoint;

    [Tooltip("The Transform of the current ground object. New civilians will be parented to this.")]
    public Transform currentGroundParent; // <-- This is set by GroundSpawner.cs

    [Header("Spawning Range")]
    [Tooltip("Minimum number of civilians to spawn at one time.")]
    [Range(1, 4)]
    public int minSpawnCount = 1;

    [Tooltip("Maximum number of civilians to spawn at one time.")]
    [Range(1, 4)]
    public int maxSpawnCount = 4;

    [Header("Timing")]
    [Tooltip("The minimum time (seconds) to wait before the next spawn wave.")]
    public float minSpawnTime = 1.0f;

    [Tooltip("The maximum time (seconds) to wait before the next spawn wave.")]
    public float maxSpawnTime = 3.0f;

    private void Start()
    {
        // Check for essential references before starting the spawn loop
        if (civilianPrefab == null || spawnPoint == null || currentGroundParent == null)
        {
            // If the key components (prefab or spawn point) are missing, log an error and disable the script.
            if (civilianPrefab == null || spawnPoint == null)
            {
                Debug.LogError("Civilian Spawner requires 'Civilian Prefab' and 'Spawn Point' to be assigned in the Inspector!");
                enabled = false;
                return;
            }
        }

        // Start the continuous spawning process
        StartCoroutine(SpawnLoop());
    }

    /// <summary>
    /// The main coroutine that handles the timing and initiation of spawn waves.
    /// </summary>
    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            // 1. Determine the wait time
            float delay = UnityEngine.Random.Range(minSpawnTime, maxSpawnTime);
            yield return new WaitForSeconds(delay);

            // 2. Determine how many civilians to spawn
            int count = UnityEngine.Random.Range(minSpawnCount, maxSpawnCount + 1); // +1 because Random.Range is exclusive for the max integer value

            // 3. Spawn the civilians
            SpawnCivilians(count);
        }
    }


    private void SpawnCivilians(int count)
    {
        if (currentGroundParent == null)
        {
            // If the GroundSpawner hasn't run yet, we skip this spawn wave and try again later.
            Debug.LogWarning("Civilian Spawner waiting for GroundSpawner to set initial parent.");
            return;
        }

        // A small offset ensures civilians don't spawn perfectly stacked on top of each other.
        Vector3 spawnPosition = spawnPoint.position;
        float spacingOffset = 0.05f;

        for (int i = 0; i < count; i++)
        {
            // Apply a small random vertical offset for visual variety
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-spacingOffset, spacingOffset),
                UnityEngine.Random.Range(-spacingOffset, spacingOffset),
                0 // Assuming 2D, Z is zero
            );

            // Instantiate the civilian at the adjusted position
            GameObject newCivilian = Instantiate( // <-- Capturing the new instance
                civilianPrefab,
                spawnPosition + randomOffset,
                Quaternion.identity // No rotation needed for basic 2D
            );

            newCivilian.transform.parent = currentGroundParent;

        }

        Debug.Log($"Spawned a wave of {count} civilians, parented to: {currentGroundParent.name}");
    }

 
    public void SetGroundParent(Transform newParent)
    {
        currentGroundParent = newParent;
        Debug.Log($"Civilian Spawner updated parent to: {newParent.name}");
    }

}
