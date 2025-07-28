using Unity.Entities;
using Unity.Mathematics;

public struct WaypointComponent : IComponentData
{
    public int Index;
    public float3 Position;
}