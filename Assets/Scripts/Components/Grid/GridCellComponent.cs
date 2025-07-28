using Unity.Entities;
using Unity.Mathematics;

public struct GridCellComponent : IComponentData
{
    public int2 GridPosition;
    public bool IsOccupied;
    public bool IsPath;
    public Entity OccupiedBy;
    public float3 WorldPosition;
}