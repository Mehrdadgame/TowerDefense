using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct ProjectileMovementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var deltaTime = SystemAPI.Time.DeltaTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

        foreach (var (projectile, transform, entity) in
                 SystemAPI.Query<RefRW<ProjectileComponent>, RefRW<LocalTransform>>().WithEntityAccess())
        {
            // Update lifetime
            projectile.ValueRW.LifeTime += deltaTime;

            if (projectile.ValueRO.LifeTime > projectile.ValueRO.MaxLifeTime)
            {
                ecb.DestroyEntity(entity);
                continue;
            }

            // Update target position if target still exists
            if (projectile.ValueRO.HasTarget && state.EntityManager.Exists(projectile.ValueRO.Target))
            {
                var targetTransform = state.EntityManager.GetComponentData<LocalTransform>(projectile.ValueRO.Target);
                projectile.ValueRW.TargetPosition = targetTransform.Position;
            }

            // Move towards target
            var currentPos = transform.ValueRO.Position;
            var targetPos = projectile.ValueRO.TargetPosition;
            var direction = math.normalize(targetPos - currentPos);
            var distance = math.distance(currentPos, targetPos);

            if (distance < 0.2f) // Hit target
            {
                // Deal damage to target
                if (state.EntityManager.Exists(projectile.ValueRO.Target))
                {
                    DealDamageToTarget(projectile.ValueRO.Target, projectile.ValueRO.Damage, ecb, ref state);
                }

                ecb.DestroyEntity(entity);
            }
            else
            {
                // Move projectile
                var newPosition = currentPos + direction * projectile.ValueRO.Speed * deltaTime;
                transform.ValueRW.Position = newPosition;

                // Rotate projectile to face direction
                if (math.lengthsq(direction) > 0.001f)
                {
                    transform.ValueRW.Rotation = quaternion.LookRotationSafe(direction, math.up());
                }
            }
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }

    private void DealDamageToTarget(Entity target, float damage, EntityCommandBuffer ecb, ref SystemState state)
    {
        if (state.EntityManager.HasComponent<EnemyComponent>(target))
        {
            var enemy = state.EntityManager.GetComponentData<EnemyComponent>(target);
            enemy.Health -= damage;

            if (enemy.Health <= 0)
            {
                // Enemy died, give reward
                var gameManager = UnityEngine.Object.FindObjectOfType<GameManager>();
                if (gameManager != null)
                {
                    gameManager.AddMoney(enemy.Reward);
                }

                ecb.DestroyEntity(target);
            }
            else
            {
                ecb.SetComponent(target, enemy);
            }
        }
    }
}