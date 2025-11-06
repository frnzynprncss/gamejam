using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PasaheroSpawner : MonoBehaviour
{
    [Header("Assets")]
    [Tooltip("Drag the civilian GameObject Prefab here.")]
    public GameObject civilianPrefab;

    [Tooltip("The Transform where civilians will be spawned (e.g., just off-screen to the right).")]
    public Transform spawnPoint;

    private Transform currentGroundParent;

    [Header("Spawning Range")]
    [Tooltip("Minimum number of civilians to spawn when a ground spawns.")]
    [Range(1, 4)]
    public int minSpawnCount = 1;

    [Tooltip("Maximum number of civilians to spawn when a ground spawns.")]
    [Range(1, 4)]
    public int maxSpawnCount = 4;

    public void SpawnCiviliansOnGround()
    {
        if (civilianPrefab == null || spawnPoint == null)
        {
            Debug.LogError("Civilian Spawner requires 'Civilian Prefab' and 'Spawn Point' to be assigned in the Inspector!");
            return;
        }

        if (currentGroundParent == null)
        {
            Debug.LogWarning("Civilian Spawner failed to spawn: currentGroundParent is not set. Is the GroundSpawner running?");
            return;
        }

        int count = UnityEngine.Random.Range(minSpawnCount, maxSpawnCount + 1);

        Vector3 spawnPosition = spawnPoint.position;
        float spacingOffset = 0.05f;

        for (int i = 0; i < count; i++)
        {
            Vector3 randomOffset = new Vector3(
                UnityEngine.Random.Range(-spacingOffset, spacingOffset),
                UnityEngine.Random.Range(-spacingOffset, spacingOffset),
                0 
            );

            GameObject newCivilian = Instantiate(
                civilianPrefab,
                spawnPosition + randomOffset,
                Quaternion.identity
            );
            newCivilian.transform.parent = currentGroundParent;

        }
        Debug.Log($"Spawned a wave of {count} civilians, parented to: {currentGroundParent.name}");
    }

    public void SetGroundParent(Transform newParent)
    {
        currentGroundParent = newParent;
    }

}
