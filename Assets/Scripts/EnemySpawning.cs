/*****************************************************************************
// File Name : EnemySpawning.cs
// Author : Drew Higgins
// Creation Date : September 6th, 2025
//
// Brief Description : This script controls how the enemies spawn in the game.
                       Sorry, I couldn't figure out randomized spawning :(
*****************************************************************************/

using UnityEngine;

public class EnemySpawning : MonoBehaviour
{
    [SerializeField] GameObject enemy;

    [SerializeField] private float minSpawnTime;
    [SerializeField] private float maxSpawnTime;
    [SerializeField] private Transform spawnPoint;

    private float nextSpawnTime;

    private PlayerController playerController;

    /// <summary>
    /// Sets up the reference to the PlayerController script and calls the spawning to start
    /// </summary>
    void Start()
    {
        playerController = GameObject.FindFirstObjectByType<PlayerController>();

        SetNextSpawnTime();
    }

    /// <summary>
    /// This function uses the randomization between two decided times to make enemies spawn at random rates
    /// </summary>
    void SetNextSpawnTime()
    {
        nextSpawnTime = Time.time + Random.Range(minSpawnTime, maxSpawnTime);
    }

    /// <summary>
    /// Makes the enemy active in the scene and spawns them at the proper spawn point
    /// </summary>
    void SpawnEnemy()
    {
        gameObject.SetActive(true);
        Instantiate(enemy, spawnPoint.position, Quaternion.identity);
    }

    /// <summary>
    /// Update checks for things that would need to be checked continuously
    /// </summary>
    void Update()
    {
        //If the player dies, all the enemies in the scene immediately die too
        if(playerController.playerHealth <= 0)
        {
            Destroy(gameObject);
        }

        //Keeps track of the time and spawns the enemy when necessary
        else if (Time.time >= nextSpawnTime)
        {
            SpawnEnemy();
            SetNextSpawnTime();
        }
    }
}
