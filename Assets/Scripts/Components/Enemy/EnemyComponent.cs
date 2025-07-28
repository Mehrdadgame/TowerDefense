using Helpers;
using Unity.Entities;

public struct EnemyComponent : IComponentData
{
    public float Health;
    public float MaxHealth;
    public float Speed;
    public EnemyType Type;
    public int Reward;
}