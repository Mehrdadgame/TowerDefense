using Helpers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemySpawnerSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var time = (float)SystemAPI.Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

        foreach (var (spawner, spawnerTransform) in
                 SystemAPI.Query<RefRW<EnemySpawnerComponent>, RefRO<LocalTransform>>())
        {
            if (spawner.ValueRO.EnemiesSpawned >= spawner.ValueRO.EnemiesToSpawn)
                continue;

            if (time >= spawner.ValueRO.LastSpawnTime + spawner.ValueRO.SpawnInterval)
            {
                SpawnEnemy(spawnerTransform.ValueRO.Position, spawner.ValueRO.EnemyTypeToSpawn, ecb);

                spawner.ValueRW.LastSpawnTime = time;
                spawner.ValueRW.EnemiesSpawned++;
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void SpawnEnemy(float3 spawnPosition, EnemyType enemyType, EntityCommandBuffer ecb)
    {
        var enemyEntity = ecb.CreateEntity();

        var enemyData = GetEnemyData(enemyType);

        ecb.AddComponent(enemyEntity, new EnemyComponent
        {
            Health = enemyData.Health,
            MaxHealth = enemyData.Health,
            Speed = enemyData.Speed,
            Type = enemyType,
            Reward = enemyData.Reward
        });

        ecb.AddComponent(enemyEntity, new EnemyMovementComponent
        {
            CurrentWaypointIndex = 0,
            HasReachedDestination = false,
            IsMoving = true
        });

        ecb.AddComponent(enemyEntity, LocalTransform.FromPosition(spawnPosition));
    }

    private EnemyData GetEnemyData(EnemyType type)
    {
        return type switch
        {
            EnemyType.Basic => new EnemyData { Health = 100f, Speed = 2f, Reward = 5 },
            EnemyType.Fast => new EnemyData { Health = 50f, Speed = 4f, Reward = 8 },
            EnemyType.Heavy => new EnemyData { Health = 200f, Speed = 1f, Reward = 15 },
            _ => new EnemyData { Health = 100f, Speed = 2f, Reward = 5 }
        };
    }
}

public struct EnemyData
{
    public float Health;
    public float Speed;
    public int Reward;
}