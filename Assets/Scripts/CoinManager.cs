using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance;

    public int coinCount = 0;
    public TextMeshProUGUI coinText;

    // Coin spawning variables
    public Transform[] spawnPositions; 
    public GameObject coinPrefab; 
    public float minSpawnInterval = 10f; 
    public float maxSpawnInterval = 25f; 
    public float coinSpawnDelay = 0.3f; 
    public int coinsPerSpawn = 10; 

    [Header("Coin Speed Settings")]
    public float startCoinSpeed = 5f; 
    public float maxCoinSpeed = 15f; 

    [Header("Progressive Difficulty")]
    public float speedIncreaseInterval = 10f; 
    public float speedIncreaseAmount = 0.5f; 

    private float timer;
    private bool isSpawning = false;
    private float currentCoinSpeed;
    private float gameTime = 0f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateCoinText();
        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        currentCoinSpeed = startCoinSpeed;
    }

    void Update()
    {
        gameTime += Time.deltaTime;

        timer -= Time.deltaTime;
        if (timer <= 0f && !isSpawning)
        {
            StartCoroutine(SpawnCoinsOneByOne());
            timer = Random.Range(minSpawnInterval, maxSpawnInterval);
        }

        UpdateCoinSpeed();
    }

    public void AddCoin()
    {
        coinCount++;
        UpdateCoinText();
    }

    private void UpdateCoinText()
    {
        if (coinText != null)
        {
            coinText.text = " " + coinCount;
        }
    }

    private IEnumerator SpawnCoinsOneByOne()
    {
        isSpawning = true;

        for (int i = 0; i < coinsPerSpawn; i++)
        {
            Transform randomSpawnPoint = spawnPositions[Random.Range(0, spawnPositions.Length)];

            GameObject newCoin = Instantiate(coinPrefab, randomSpawnPoint.position, Quaternion.identity);

            Coin coinScript = newCoin.GetComponent<Coin>();
            if (coinScript != null)
            {
                coinScript.moveSpeed = currentCoinSpeed;
            }

            yield return new WaitForSeconds(coinSpawnDelay);
        }

        isSpawning = false;
    }

    private void UpdateCoinSpeed()
    {
        int speedIncreases = Mathf.FloorToInt(gameTime / speedIncreaseInterval);

        float newSpeed = startCoinSpeed + (speedIncreases * speedIncreaseAmount);
        currentCoinSpeed = Mathf.Min(newSpeed, maxCoinSpeed);
    }

    public void ResetCoins()
    {
        currentCoinSpeed = startCoinSpeed;
        gameTime = 0f;
        timer = Random.Range(minSpawnInterval, maxSpawnInterval);
    }
}