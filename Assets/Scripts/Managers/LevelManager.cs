using UnityEngine;
using Unity.Entities;
using Helpers;

public class LevelManager : MonoBehaviour
{
    [Header("Waypoints")]
    [SerializeField] private Transform[] waypoints;

    [Header("Spawner Settings")]
    [SerializeField] private Transform enemySpawnPoint;
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int totalEnemies = 20;

    private EntityManager entityManager;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        SetupLevel();
    }

    private void SetupLevel()
    {
        // Create waypoint entities
        for (int i = 0; i < waypoints.Length; i++)
        {
            var waypointEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(waypointEntity, new WaypointComponent
            {
                Index = i,
                Position = waypoints[i].position
            });
            entityManager.AddComponentData(waypointEntity,
                Unity.Transforms.LocalTransform.FromPosition(waypoints[i].position));
        }

        // Create enemy spawner
        if (enemySpawnPoint != null)
        {
            var spawnerEntity = entityManager.CreateEntity();
            entityManager.AddComponentData(spawnerEntity, new EnemySpawnerComponent
            {
                SpawnInterval = spawnInterval,
                EnemiesToSpawn = totalEnemies,
                EnemyTypeToSpawn = EnemyType.Basic,
                LastSpawnTime = 0f,
                EnemiesSpawned = 0
            });
            entityManager.AddComponentData(spawnerEntity,
                Unity.Transforms.LocalTransform.FromPosition(enemySpawnPoint.position));
        }
    }

    // Method to start next wave
    public void StartNextWave(int waveNumber)
    {
        // You can modify spawn settings based on wave number
        var spawnerQuery = entityManager.CreateEntityQuery(typeof(EnemySpawnerComponent));
        var spawners = spawnerQuery.ToEntityArray(Unity.Collections.Allocator.TempJob);

        foreach (var spawner in spawners)
        {
            var spawnerComponent = entityManager.GetComponentData<EnemySpawnerComponent>(spawner);
            spawnerComponent.EnemiesSpawned = 0;
            spawnerComponent.EnemiesToSpawn = 10 + (waveNumber * 5); // More enemies each wave
            spawnerComponent.SpawnInterval = Mathf.Max(0.5f, 2f - (waveNumber * 0.1f)); // Faster spawning

            // Mix enemy types based on wave
            if (waveNumber > 2)
            {
                spawnerComponent.EnemyTypeToSpawn = EnemyType.Fast;
            }
            if (waveNumber > 5)
            {
                spawnerComponent.EnemyTypeToSpawn = EnemyType.Heavy;
            }

            entityManager.SetComponentData(spawner, spawnerComponent);
        }

        spawners.Dispose();
    }
}