using UnityEngine;
using Unity.Entities;

public class WaypointAuthoring : MonoBehaviour
{
    [SerializeField] private int waypointIndex;

    class WaypointBaker : Baker<WaypointAuthoring>
    {
        public override void Bake(WaypointAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new WaypointComponent
            {
                Index = authoring.waypointIndex,
                Position = authoring.transform.position
            });
        }
    }
}