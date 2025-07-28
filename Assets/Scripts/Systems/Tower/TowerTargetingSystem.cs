using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using Unity.Collections;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(EnemyMovementSystem))]
public partial struct TowerTargetingSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var time = (float)SystemAPI.Time.ElapsedTime;
        var deltaTime = SystemAPI.Time.DeltaTime;

        foreach (var (tower, towerTarget, towerTransform) in
                 SystemAPI.Query<RefRO<TowerComponent>, RefRW<TowerTargetComponent>, RefRO<LocalTransform>>())
        {
            // Check for target every 0.1 seconds for performance
            if (time - towerTarget.ValueRO.LastTargetCheckTime < 0.1f)
                continue;

            var towerPosition = towerTransform.ValueRO.Position;
            var bestTarget = Entity.Null;
            var closestDistance = float.MaxValue;

            // Find closest enemy in range
            foreach (var (enemy, enemyTransform, enemyEntity) in
                     SystemAPI.Query<RefRO<EnemyComponent>, RefRO<LocalTransform>>().WithEntityAccess())
            {
                var distance = math.distance(towerPosition, enemyTransform.ValueRO.Position);

                if (distance <= tower.ValueRO.Range && distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = enemyEntity;
                }
            }

            // Update target
            towerTarget.ValueRW.Target = bestTarget;
            towerTarget.ValueRW.HasTarget = bestTarget != Entity.Null;
            towerTarget.ValueRW.LastTargetCheckTime = time;

            // Validate current target is still in range
            if (towerTarget.ValueRO.HasTarget && state.EntityManager.Exists(towerTarget.ValueRO.Target))
            {
                var targetTransform = state.EntityManager.GetComponentData<LocalTransform>(towerTarget.ValueRO.Target);
                var targetDistance = math.distance(towerPosition, targetTransform.Position);

                if (targetDistance > tower.ValueRO.Range)
                {
                    towerTarget.ValueRW.HasTarget = false;
                    towerTarget.ValueRW.Target = Entity.Null;
                }
            }
        }
    }
}