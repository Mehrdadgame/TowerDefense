using Unity.Entities;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct TowerPlacementEventSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var ecb = new EntityCommandBuffer(Unity.Collections.Allocator.TempJob);

        foreach (var (placementEvent, entity) in
                 SystemAPI.Query<RefRO<TowerPlacedEventComponent>>().WithEntityAccess())
        {
            // Notify hybrid game manager
            var gameManager = UnityEngine.Object.FindFirstObjectByType<GameManager>();
            if (gameManager != null)
            {
                gameManager.OnTowerPlaced(placementEvent.ValueRO.TowerType, placementEvent.ValueRO.Cost);
            }

            // Destroy the event entity
            ecb.DestroyEntity(entity);
        }

        ecb.Playback(state.EntityManager);
        ecb.Dispose();
    }
}