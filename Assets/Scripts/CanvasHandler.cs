using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.ComponentModel.Design.Serialization;

public class CanvasHandler : MonoBehaviour
{
    // UI element
    public GameObject buyTileButton; // Buy Tile Button
    public GameObject buyBuildingButton; // Buy Building Button (for owned tiles)
    public GameObject buildingMenu; // The building selection menu
    public TMP_Text buildingName; // TextMeshPro for building description
    public TMP_Text buildingDescription; // TextMeshPro for building description
    public TMP_Text moneyDisplay; // TextMeshPro for money display
    public Button nextBuildingButton; // Button to view next building
    public Button selectBuildingButton; // Button to select the building
    public Button cancelBuildingButton; // Cancel button to close the building menu

    private BuildingData[] availableBuildings; // Array of buildings to choose from
    private int currentBuildingIndex = 0; // Index to track the current building
    public Tile currentTile; // Track selected tile
    public Player player; // Reference to player
    public BuildingPlacer buildingPlacer;

    void Start()
    {
        // Ensure buttons are initially hidden
        HideAllButtons();

        // Set up button events
        if (buyTileButton != null) buyTileButton.GetComponent<Button>().onClick.AddListener(BuyTile);
        if (buyBuildingButton != null) buyBuildingButton.GetComponent<Button>().onClick.AddListener(ShowBuildingMenu);
        if (nextBuildingButton != null) nextBuildingButton.onClick.AddListener(NextBuilding);
        if (selectBuildingButton != null) selectBuildingButton.onClick.AddListener(SelectBuilding);
        if (cancelBuildingButton != null) cancelBuildingButton.onClick.AddListener(CancelBuildingSelection);

        // Find the Player instance
        player = FindObjectOfType<Player>();
        UpdateMoneyDisplay();

        // Load BuildingData from the "Buildings" folder
        LoadBuildingData();
    }

    private void LoadBuildingData()
    {
        // Load all BuildingData assets from the "Buildings" folder
        availableBuildings = Resources.LoadAll<BuildingData>("Buildings");

        // Check if any buildings were loaded
        if (availableBuildings.Length == 0)
        {
            Debug.LogWarning("No BuildingData found in Resources/Buildings!");
        }
    }

    public void ShowBuyButtonForTile(Tile tile)
    {
        currentTile = tile;
        if (!currentTile.isOwned) // Show Buy Tile button if tile is not owned
        {
            buyTileButton.SetActive(true);
            buyBuildingButton.SetActive(false);
        }
        else // Show Buy Building button if tile is owned
        {
            buyBuildingButton.SetActive(true);
            buyTileButton.SetActive(false);
        }
    }

    public void HideAllButtons()
    {
        buyTileButton.SetActive(false);
        buyBuildingButton.SetActive(false);
        buildingMenu.SetActive(false);
    }

    public void BuyTile()
    {
        if (currentTile != null && player != null)
        {
            int cost = currentTile.GetCost();
            if (player.GetMoney() >= cost)
            {
                player.SubtractMoney(cost);
                currentTile.BuyTile();
                HideAllButtons();
                UpdateMoneyDisplay();
            }
            else
            {
                Debug.Log("Not enough money to buy tile.");
            }
        }
    }

    public void ShowBuildingMenu()
    {
        if (currentTile != null && currentTile.isOwned)
        {
            currentBuildingIndex = 0; // Reset to the first building
            UpdateBuildingDisplay(); // Show first building
            buildingMenu.SetActive(true); // Show the building selection menu
        }
    }

    public void NextBuilding()
    {
        // Cycle through buildings
        currentBuildingIndex = (currentBuildingIndex + 1) % availableBuildings.Length;
        UpdateBuildingDisplay();
    }

    public void SelectBuilding()
    {
        BuildingData selectedBuilding = availableBuildings[currentBuildingIndex];
        buildingPlacer.PlaceBuilding(selectedBuilding);
        buildingMenu.SetActive(false); // Hide the menu after selection
    }

    public void CompleteBuildingPlacement()
    {
        BuildingData selectedBuilding = availableBuildings[currentBuildingIndex];
        int cost = selectedBuilding.cost;
        if (player.GetMoney() >= cost)
        {
            player.SubtractMoney(cost);
            buildingPlacer.currentBuildingColor = selectedBuilding.color;
            buildingPlacer.MakeBuildingPermanent();
        }
    }


    public void CancelBuildingSelection()
    {
        buildingMenu.SetActive(false);
    }

    void UpdateBuildingDisplay()
    {
        BuildingData currentBuilding = availableBuildings[currentBuildingIndex];

        buildingName.text = $"{currentBuilding.buildingName}";
        buildingDescription.text = $"Size: {currentBuilding.length}x{currentBuilding.width}x{currentBuilding.height}\n" +
                                   $"Cost: {currentBuilding.cost}\n" +
                                   $"{currentBuilding.description}";
    }

    void UpdateMoneyDisplay()
    {
        if (moneyDisplay != null && player != null)
        {
            moneyDisplay.text = "Money: $" + player.GetMoney();
        }
    }

    public void UpdateBuildingInfo(BuildingInfo building) 
    {
        BuildingData selectedBuilding = availableBuildings[currentBuildingIndex];
        building.buildingType = selectedBuilding.buildingType;
        building.residents = selectedBuilding.residents;
        building.jobs = selectedBuilding.jobs;
        building.shoppingSpace = selectedBuilding.shoppingSpace;

    }

    void Update() 
    {
        UpdateMoneyDisplay();
    }
}
