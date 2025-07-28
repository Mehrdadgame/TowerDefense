using UnityEngine;
using Unity.Entities;
using Helpers;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int playerLives = 20;
    [SerializeField] private TowerSelectionUI towerSelectionUI;

    private EntityManager entityManager;
    private Entity inputEntity;
    private int playerMoney;

    void Start()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        // Create input entity
        inputEntity = entityManager.CreateEntity();
        entityManager.AddComponentData(inputEntity, new InputManagerComponent());

        UpdateUI();
    }

    public void SelectTower(TowerType towerType)
    {
        if (CanAffordTowerType(towerType))
        {
            var inputComponent = entityManager.GetComponentData<InputManagerComponent>(inputEntity);
            inputComponent.SelectedTowerType = towerType;
            inputComponent.HasSelectedTower = true;
            entityManager.SetComponentData(inputEntity, inputComponent);
        }
    }

    public void OnTowerPlaced(TowerType towerType, int cost)
    {
        playerMoney -= cost;

        // Deselect tower
        var inputComponent = entityManager.GetComponentData<InputManagerComponent>(inputEntity);
        inputComponent.HasSelectedTower = false;
        entityManager.SetComponentData(inputEntity, inputComponent);

        towerSelectionUI.DeselectTower();
        UpdateUI();
    }

    public bool CanAffordTowerType(TowerType towerType)
    {
        var cost = GetTowerCost(towerType);
        return playerMoney >= cost;
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateUI();
    }

    public void TakeDamage(int damage)
    {
        playerLives -= damage;
        UpdateUI();

        if (playerLives <= 0)
        {
            GameOver();
        }
    }

    private void GameOver()
    {
        Debug.Log("Game Over!");
        // Implement game over logic
    }

    private int GetTowerCost(TowerType towerType)
    {
        return towerType switch
        {
            TowerType.Basic => 10,
            TowerType.Heavy => 25,
            TowerType.Fast => 15,
            _ => 10
        };
    }

    private void UpdateUI()
    {
        towerSelectionUI.UpdateMoneyDisplay(playerMoney);
        towerSelectionUI.UpdateTowerButtons(this);
    }
}