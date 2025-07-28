using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;

public class GridAuthoring : MonoBehaviour
{
    [SerializeField] private int2 gridSize = new int2(10, 8);
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private Vector3 gridOrigin = Vector3.zero;

    class GridBaker : Baker<GridAuthoring>
    {
        public override void Bake(GridAuthoring authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new GridManagerComponent
            {
                GridSize = authoring.gridSize,
                CellSize = authoring.cellSize,
                GridOrigin = authoring.gridOrigin
            });
        }
    }
}