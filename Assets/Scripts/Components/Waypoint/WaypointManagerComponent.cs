using Unity.Collections;
using Unity.Entities;

public struct WaypointManagerComponent : IComponentData
{
    public Entity WaypointContainer;
    public int WaypointCount;
}