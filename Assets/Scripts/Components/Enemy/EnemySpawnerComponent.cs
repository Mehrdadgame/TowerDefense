using Helpers;
using Unity.Entities;

public struct EnemySpawnerComponent : IComponentData
{
    public float SpawnInterval;
    public float LastSpawnTime;
    public int EnemiesToSpawn;
    public int EnemiesSpawned;
    public EnemyType EnemyTypeToSpawn;
}