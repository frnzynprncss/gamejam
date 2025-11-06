using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HouseSpawner : MonoBehaviour
{
    public GameObject housePrefab; // Assign your house prefab in inspector
    public float spawnInterval = 3f; // Editable in inspector

    private float timer;

    void Start()
    {
        timer = spawnInterval; // Start spawning immediately
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            SpawnHouse();
            timer = spawnInterval; // Reset timer
        }
    }

    void SpawnHouse()
    {
        if (housePrefab != null)
        {
            Instantiate(housePrefab, transform.position, transform.rotation);
        }
        else
        {
            Debug.LogWarning("House prefab not assigned in HouseSpawner!");
        }
    }
}