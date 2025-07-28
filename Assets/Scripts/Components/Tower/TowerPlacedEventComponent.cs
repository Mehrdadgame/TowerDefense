using Helpers;
using Unity.Entities;

public struct TowerPlacedEventComponent : IComponentData
{
    public TowerType TowerType;
    public int Cost;
}