using Unity.Entities;
using Unity.Mathematics;

public struct ProjectileComponent : IComponentData
{
    public float Damage;
    public float Speed;
    public Entity Target;
    public float3 TargetPosition;
    public bool HasTarget;
    public float LifeTime;
    public float MaxLifeTime;
}