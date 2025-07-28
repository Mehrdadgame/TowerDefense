using Unity.Entities;
using Unity.Mathematics;

public struct GridManagerComponent : IComponentData
{
    public int2 GridSize;
    public float CellSize;
    public float3 GridOrigin;
}