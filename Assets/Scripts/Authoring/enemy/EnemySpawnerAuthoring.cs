using UnityEngine;
using Unity.Entities;
using Helpers;

public class EnemySpawnerAuthoring : MonoBehaviour
{
    [SerializeField] private float spawnInterval = 2f;
    [SerializeField] private int enemiesToSpawn = 10;
    [SerializeField] private EnemyType enemyTypeToSpawn = EnemyType.Basic;

    class EnemySpawnerBaker : Baker<EnemySpawnerAuthoring>
    {
        public override void Bake(EnemySpawnerAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EnemySpawnerComponent
            {
                SpawnInterval = authoring.spawnInterval,
                EnemiesToSpawn = authoring.enemiesToSpawn,
                EnemyTypeToSpawn = authoring.enemyTypeToSpawn,
                LastSpawnTime = 0f,
                EnemiesSpawned = 0
            });
        }
    }
}