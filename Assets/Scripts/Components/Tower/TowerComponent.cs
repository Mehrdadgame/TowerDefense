using Helpers;
using Unity.Entities;

public struct TowerComponent : IComponentData
{
    public float Range;
    public float Damage;
    public float FireRate;
    public float LastFireTime;
    public TowerType Type;
    public int Cost;
}