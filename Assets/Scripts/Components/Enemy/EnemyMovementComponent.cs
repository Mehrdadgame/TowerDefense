using Unity.Entities;
using Unity.Mathematics;

public struct EnemyMovementComponent : IComponentData
{
    public int CurrentWaypointIndex;
    public float3 TargetPosition;
    public bool HasReachedDestination;
    public bool IsMoving;
}