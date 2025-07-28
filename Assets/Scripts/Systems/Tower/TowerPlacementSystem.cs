using Helpers;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

[UpdateInGroup(typeof(SimulationSystemGroup))]
public partial struct TowerPlacementSystem : ISystem
{
    public void OnUpdate(ref SystemState state)
    {
        var inputManager = SystemAPI.GetSingleton<InputManagerComponent>();

        if (!inputManager.PlaceTowerRequested || !inputManager.HasSelectedTower)
            return;

        var gridManager = SystemAPI.GetSingleton<GridManagerComponent>();
        var mouseWorldPos = inputManager.MouseWorldPosition;
        var gridPosition = WorldToGridPosition(mouseWorldPos, gridManager);

        // Check if placement is valid
        if (CanPlaceTowerAt(gridPosition, ref state))
        {
            PlaceTower(gridPosition, inputManager.SelectedTowerType, ref state);

            // Update grid cell
            UpdateGridCell(gridPosition, true, ref state);

            // Notify game manager to deduct money
            NotifyTowerPlaced(inputManager.SelectedTowerType, ref state);
        }
    }

    private int2 WorldToGridPosition(float3 worldPos, GridManagerComponent gridManager)
    {
        var relativePos = worldPos - gridManager.GridOrigin;
        return new int2(
            (int)math.round(relativePos.x / gridManager.CellSize),
            (int)math.round(relativePos.z / gridManager.CellSize)
        );
    }

    private bool CanPlaceTowerAt(int2 gridPosition, ref SystemState state)
    {
        var gridManager = SystemAPI.GetSingleton<GridManagerComponent>();

        // Check bounds
        if (gridPosition.x < 0 || gridPosition.x >= gridManager.GridSize.x ||
            gridPosition.y < 0 || gridPosition.y >= gridManager.GridSize.y)
        {
            return false;
        }

        // Check if cell is available
        foreach (var gridCell in SystemAPI.Query<RefRO<GridCellComponent>>())
        {
            if (math.all(gridCell.ValueRO.GridPosition == gridPosition))
            {
                return !gridCell.ValueRO.IsOccupied && !gridCell.ValueRO.IsPath;
            }
        }

        return false;
    }

    private void PlaceTower(int2 gridPosition, TowerType towerType, ref SystemState state)
    {
        var gridManager = SystemAPI.GetSingleton<GridManagerComponent>();
        var worldPosition = GridToWorldPosition(gridPosition, gridManager);

        // Create tower entity
        var towerEntity = state.EntityManager.CreateEntity();

        // Get tower data based on type
        var towerData = GetTowerData(towerType);

        state.EntityManager.AddComponentData(towerEntity, new TowerComponent
        {
            Range = towerData.Range,
            Damage = towerData.Damage,
            FireRate = towerData.FireRate,
            Type = towerType,
            Cost = towerData.Cost
        });

        state.EntityManager.AddComponentData(towerEntity, LocalTransform.FromPosition(worldPosition));

        // Add visual representation (you'll need to implement this based on your rendering approach)
        // For ECS Graphics or GameObject conversion
    }

    private void UpdateGridCell(int2 gridPosition, bool occupied, ref SystemState state)
    {
        foreach (var (gridCell, entity) in SystemAPI.Query<RefRW<GridCellComponent>>().WithEntityAccess())
        {
            if (math.all(gridCell.ValueRO.GridPosition == gridPosition))
            {
                gridCell.ValueRW.IsOccupied = occupied;
                break;
            }
        }
    }

    private void NotifyTowerPlaced(TowerType towerType, ref SystemState state)
    {
        // Create an event entity for the game manager to process
        var eventEntity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponentData(eventEntity, new TowerPlacedEventComponent
        {
            TowerType = towerType,
            Cost = GetTowerData(towerType).Cost
        });
    }

    private float3 GridToWorldPosition(int2 gridPos, GridManagerComponent gridManager)
    {
        return gridManager.GridOrigin + new float3(
            gridPos.x * gridManager.CellSize,
            0,
            gridPos.y * gridManager.CellSize
        );
    }

    private TowerData GetTowerData(TowerType type)
    {
        // This would typically come from a ScriptableObject or data table
        return type switch
        {
            TowerType.Basic => new TowerData { Range = 3f, Damage = 10f, FireRate = 1f, Cost = 10 },
            TowerType.Heavy => new TowerData { Range = 2f, Damage = 25f, FireRate = 0.5f, Cost = 25 },
            TowerType.Fast => new TowerData { Range = 4f, Damage = 5f, FireRate = 3f, Cost = 15 },
            _ => new TowerData { Range = 3f, Damage = 10f, FireRate = 1f, Cost = 10 }
        };
    }
}
