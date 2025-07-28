using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct EnemyMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Allocator.TempJob);

        // Get all waypoints
        var waypointQuery = SystemAPI.QueryBuilder()
            .WithAll<WaypointComponent, LocalTransform>()
            .Build();

        var waypoints = new NativeArray<float3>(10, Allocator.TempJob); // Max 10 waypoints
        var waypointCount = 0;

        foreach (var (waypoint, transform) in
                 SystemAPI.Query<RefRO<WaypointComponent>, RefRO<LocalTransform>>())
        {
            if (waypoint.ValueRO.Index < waypoints.Length)
            {
                waypoints[waypoint.ValueRO.Index] = transform.ValueRO.Position;
                waypointCount = math.max(waypointCount, waypoint.ValueRO.Index + 1);
            }
        }

        // Move enemies
        foreach (var (enemy, movement, transform, entity) in
                 SystemAPI.Query<RefRO<EnemyComponent>, RefRW<EnemyMovementComponent>, RefRW<LocalTransform>>()
                 .WithEntityAccess())
        {
            if (movement.ValueRO.HasReachedDestination)
            {
                // Enemy reached the end, damage player and destroy enemy
                var gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.TakeDamage(1);
                }
                ecb.DestroyEntity(entity);
                continue;
            }

            MoveEnemyTowardsTarget(ref movement.ValueRW, ref transform.ValueRW,
                                 enemy.ValueRO.Speed, deltaTime, waypoints, waypointCount);
        }

        waypoints.Dispose();
        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void MoveEnemyTowardsTarget(ref EnemyMovementComponent movement,
                                       ref LocalTransform transform,
                                       float speed, float deltaTime,
                                       NativeArray<float3> waypoints, int waypointCount)
    {
        if (movement.CurrentWaypointIndex >= waypointCount)
        {
            movement.HasReachedDestination = true;
            return;
        }

        var targetPosition = waypoints[movement.CurrentWaypointIndex];
        var currentPosition = transform.Position;
        var direction = math.normalize(targetPosition - currentPosition);
        var distance = math.distance(currentPosition, targetPosition);

        if (distance < 0.1f) // Reached waypoint
        {
            movement.CurrentWaypointIndex++;
            if (movement.CurrentWaypointIndex >= waypointCount)
            {
                movement.HasReachedDestination = true;
            }
        }
        else
        {
            // Move towards target
            var newPosition = currentPosition + direction * speed * deltaTime;
            transform.Position = newPosition;

            // Rotate to face movement direction
            if (math.lengthsq(direction) > 0.001f)
            {
                transform.Rotation = quaternion.LookRotationSafe(direction, math.up());
            }
        }
    }
}