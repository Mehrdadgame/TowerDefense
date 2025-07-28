using Unity.Entities;

public struct TowerTargetComponent : IComponentData
{
    public Entity Target;
    public bool HasTarget;
    public float LastTargetCheckTime;
}