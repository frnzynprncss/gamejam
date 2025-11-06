using UnityEngine;

public class HumpSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject humpPrefab;          // Prefab of your hump
    public Transform[] spawnPoints;        // Assign spawn point transforms in Inspector

    [Header("Spawn Interval Settings")]
    public float minSpawnInterval = 1f;    // Minimum time between spawns
    public float maxSpawnInterval = 3f;    // Maximum time between spawns
    private float nextSpawnTime;           // Time until next hump spawn

    [Header("Hump Size Settings")]
    public float minScale = 0.8f;          // Minimum uniform scale
    public float maxScale = 2f;            // Maximum uniform scale

    [Header("Movement Settings")]
    public float moveSpeed = 5f;           // Speed of moving left
    public float destroyX = -10f;          // Destroy when it passes this X

    private float timer;

    void Start()
    {
        // Set first random spawn delay
        nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnHump();
            timer = 0f;

            // Reset with a new random interval
            nextSpawnTime = Random.Range(minSpawnInterval, maxSpawnInterval);
        }
    }

    void SpawnHump()
    {
        if (spawnPoints.Length == 0) return;

        // Pick a random spawn point
        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

        // Create the hump using prefab's own rotation
        GameObject hump = Instantiate(humpPrefab, spawnPoint.position, humpPrefab.transform.rotation);

        // Randomize **uniform scale**
        float randomScale = Random.Range(minScale, maxScale);
        hump.transform.localScale = new Vector3(randomScale, randomScale, 1f);

        // Attach mover
        hump.AddComponent<HumpMover>().Init(moveSpeed, destroyX);
    }
}

public class HumpMover : MonoBehaviour
{
    private float speed;
    private float destroyX;

    public void Init(float moveSpeed, float destroyLimitX)
    {
        speed = moveSpeed;
        destroyX = destroyLimitX;
    }

    void Update()
    {
        // Move left
        transform.position += Vector3.left * speed * Time.deltaTime;

        // Destroy when off-screen
        if (transform.position.x < destroyX)
        {
            Destroy(gameObject);
        }
    }
}
