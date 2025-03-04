using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnSouls : MonoBehaviour
{
    [SerializeField] List<GameObject> objectPrefabs; // List of prefabs to spawn
    [SerializeField] float spawnInterval = 1f; // Time between spawns
    [SerializeField] float spawnDistance = 5f; // Distance in front of the player to spawn objects
    [SerializeField] float minY = 1f; // Minimum Y position for spawning
    [SerializeField] float maxY = 5f; // Maximum Y position for spawning
    [SerializeField] float minX = -5f; // Minimum X position for spawning
    [SerializeField] float maxX = 5f; // Maximum X position for spawning

    private float timer;
    private Transform player;
    // Start is called before the first frame update
    void Start()
    {
        // Find the player object (make sure it has a "Player" tag)
        player = GameObject.FindGameObjectWithTag("Player").transform;

        if (player == null)
        {
            Debug.LogError("Player not found! Make sure the player has the 'Player' tag.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Count down the timer
        timer -= Time.deltaTime;

        // Spawn a new object when the timer reaches zero
        if (timer <= 0f)
        {
            SpawnObject();
            timer = spawnInterval; // Reset the timer
        }
    }
    void SpawnObject()
    {
        if (player == null) return;

        GameObject prefab = objectPrefabs[Random.Range(0, objectPrefabs.Count)];
        // Define lanes (e.g., -2, 0, 2 for left, middle, right)
        float[] lanes = { -2f, 0f, 2f };
        float lane = lanes[Random.Range(0, lanes.Length)];

        Vector3 spawnPosition = new Vector3(
            lane, // Fixed X position (lane)
            Random.Range(minY, maxY), // Random Y position
            player.transform.position.z + spawnDistance // Fixed Z position
        );

        Instantiate(prefab, spawnPosition, Quaternion.identity);
    }
}