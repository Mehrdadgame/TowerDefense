using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Helpers;

public class TowerSelectionUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Button basicTowerButton;
    [SerializeField] private Button heavyTowerButton;
    [SerializeField] private Button fastTowerButton;
    [SerializeField] private TextMeshProUGUI moneyText;
    [SerializeField] private GameObject selectedTowerIndicator;

    [Header("Tower Costs")]
    [SerializeField] private TextMeshProUGUI basicTowerCostText;
    [SerializeField] private TextMeshProUGUI heavyTowerCostText;
    [SerializeField] private TextMeshProUGUI fastTowerCostText;

    private GameManager gameManager;
    private TowerType? selectedTowerType = null;

    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        SetupButtons();
        UpdateCostTexts();
    }

    private void SetupButtons()
    {
        basicTowerButton.onClick.AddListener(() => SelectTower(TowerType.Basic));
        heavyTowerButton.onClick.AddListener(() => SelectTower(TowerType.Heavy));
        fastTowerButton.onClick.AddListener(() => SelectTower(TowerType.Fast));
    }

    private void UpdateCostTexts()
    {
        basicTowerCostText.text = "$10";
        heavyTowerCostText.text = "$25";
        fastTowerCostText.text = "$15";
    }

    public void SelectTower(TowerType towerType)
    {
        selectedTowerType = towerType;
        gameManager.SelectTower(towerType);
        UpdateSelectedIndicator();
    }

    public void UpdateMoneyDisplay(int money)
    {
        moneyText.text = $"Money: ${money}";
    }

    public void UpdateTowerButtons(GameManager manager)
    {
        basicTowerButton.interactable = manager.CanAffordTowerType(TowerType.Basic);
        heavyTowerButton.interactable = manager.CanAffordTowerType(TowerType.Heavy);
        fastTowerButton.interactable = manager.CanAffordTowerType(TowerType.Fast);
    }

    private void UpdateSelectedIndicator()
    {
        // Move indicator to selected button
        if (selectedTowerIndicator != null && selectedTowerType.HasValue)
        {
            Transform targetButton = selectedTowerType.Value switch
            {
                TowerType.Basic => basicTowerButton.transform,
                TowerType.Heavy => heavyTowerButton.transform,
                TowerType.Fast => fastTowerButton.transform,
                _ => null
            };

            if (targetButton != null)
            {
                selectedTowerIndicator.transform.SetParent(targetButton);
                selectedTowerIndicator.transform.localPosition = Vector3.zero;
                selectedTowerIndicator.SetActive(true);
            }
        }
    }

    public void DeselectTower()
    {
        selectedTowerType = null;
        if (selectedTowerIndicator != null)
        {
            selectedTowerIndicator.SetActive(false);
        }
    }
}