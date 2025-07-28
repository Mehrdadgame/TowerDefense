using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[UpdateInGroup(typeof(InitializationSystemGroup))]
public partial class InputSystem : SystemBase
{
    private Camera mainCamera;

    protected override void OnCreate()
    {
        mainCamera = Camera.main;
        RequireForUpdate<InputManagerComponent>();
    }

    protected override void OnUpdate()
    {
        var inputManager = SystemAPI.GetSingletonRW<InputManagerComponent>();

        // Update mouse position
        var mouseScreenPos = Input.mousePosition;
        var ray = mainCamera.ScreenPointToRay(mouseScreenPos);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            inputManager.ValueRW.MouseWorldPosition = hit.point;
        }

        // Check for placement input
        if (Input.GetMouseButtonDown(0) && inputManager.ValueRO.HasSelectedTower)
        {
            inputManager.ValueRW.PlaceTowerRequested = true;
        }
        else
        {
            inputManager.ValueRW.PlaceTowerRequested = false;
        }
    }
}