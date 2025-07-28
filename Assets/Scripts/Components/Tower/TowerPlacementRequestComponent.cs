using Helpers;
using Unity.Entities;
using Unity.Mathematics;

public struct TowerPlacementRequestComponent : IComponentData
{
    public int2 GridPosition;
    public TowerType TowerType;
    public Entity RequestedBy;
}