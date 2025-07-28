using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
[UpdateAfter(typeof(TowerTargetingSystem))]
public partial struct TowerShootingSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var time = (float)SystemAPI.Time.ElapsedTime;
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

        foreach (var (tower, towerTarget, towerTransform) in
                 SystemAPI.Query<RefRW<TowerComponent>, RefRO<TowerTargetComponent>, RefRO<LocalTransform>>())
        {
            if (!towerTarget.ValueRO.HasTarget ||
                time < tower.ValueRO.LastFireTime + (1f / tower.ValueRO.FireRate))
                continue;

            // Validate target still exists
            if (!state.EntityManager.Exists(towerTarget.ValueRO.Target))
                continue;

            var targetTransform = state.EntityManager.GetComponentData<LocalTransform>(towerTarget.ValueRO.Target);

            // Create projectile
            var projectileEntity = ecb.CreateEntity();

            ecb.AddComponent(projectileEntity, new ProjectileComponent
            {
                Damage = tower.ValueRO.Damage,
                Speed = 10f, // Projectile speed
                Target = towerTarget.ValueRO.Target,
                TargetPosition = targetTransform.Position,
                HasTarget = true,
                LifeTime = 0f,
                MaxLifeTime = 5f
            });

            ecb.AddComponent(projectileEntity, LocalTransform.FromPosition(towerTransform.ValueRO.Position));

            // Update tower fire time
            tower.ValueRW.LastFireTime = time;

            // Rotate tower to face target
            var direction = math.normalize(targetTransform.Position - towerTransform.ValueRO.Position);
            // You can add tower rotation logic here if needed
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}