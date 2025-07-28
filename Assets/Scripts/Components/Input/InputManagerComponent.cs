using Unity.Entities;
using Unity.Mathematics;
using Helpers;

public struct InputManagerComponent : IComponentData
{
    public bool PlaceTowerRequested;
    public float3 MouseWorldPosition;
    public TowerType SelectedTowerType;
    public bool HasSelectedTower;
}
