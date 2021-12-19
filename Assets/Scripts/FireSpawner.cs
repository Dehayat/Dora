using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireSpawner : MonoBehaviour
{
    [SerializeField]
    private float spawnInterval = 0.5f;
    [SerializeField]
    private Transform[] spawnPoints;
    [SerializeField]
    private GameObject spawnedPrefab;

    private float nextSpawn;

    private void Start()
    {
        nextSpawn = Time.time + spawnInterval;
    }
    private void Update()
    {
        if (Time.time > nextSpawn)
        {
            int randomPosition = Random.Range(0, spawnPoints.Length);
            Instantiate(spawnedPrefab, spawnPoints[randomPosition].position, Quaternion.identity);
            nextSpawn = Time.time + spawnInterval;
        }
    }

}
