using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial struct GridInitializationSystem : ISystem
{
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<GridManagerComponent>();
    }

    public void OnUpdate(ref SystemState state)
    {
        var gridManager = SystemAPI.GetSingleton<GridManagerComponent>();

        // Create grid cells
        for (int x = 0; x < gridManager.GridSize.x; x++)
        {
            for (int y = 0; y < gridManager.GridSize.y; y++)
            {
                var gridEntity = state.EntityManager.CreateEntity();
                var worldPos = GridToWorldPosition(new int2(x, y), gridManager);

                state.EntityManager.AddComponentData(gridEntity, new GridCellComponent
                {
                    GridPosition = new int2(x, y),
                    IsOccupied = false,
                    IsPath = IsPathCell(x, y), // Define your path logic here
                    WorldPosition = worldPos
                });

                state.EntityManager.AddComponentData(gridEntity, LocalTransform.FromPosition(worldPos));
            }
        }

        state.Enabled = false; // Run only once
    }

    private float3 GridToWorldPosition(int2 gridPos, GridManagerComponent gridManager)
    {
        return gridManager.GridOrigin + new float3(
            gridPos.x * gridManager.CellSize,
            0,
            gridPos.y * gridManager.CellSize
        );
    }

    private bool IsPathCell(int x, int y)
    {
        // Simple path logic - customize based on your level design
        return y == 2; // Horizontal path at row 2
    }
}