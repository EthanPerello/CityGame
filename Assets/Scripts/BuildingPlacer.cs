using UnityEngine;

public class BuildingPlacer : MonoBehaviour
{
    public GameObject buildingPrefab; // Assign your cube prefab in the Inspector
    private GameObject currentBuilding; // Reference to the currently placed building
    public LayerMask roadLayer; // Assign the "Road" layer in the Inspector

    private bool isPlacementValid = false;

    private bool placingBuilding = false;

    public CanvasHandler canvasHandler;

    private Tile currentTile;

    public Color currentBuildingColor;

    void Update()
    {
        if (placingBuilding == true)
        {
            // Move the building to the mouse position
            UpdateBuildingPosition();
        }

        currentTile = canvasHandler.currentTile;
    }

    // Public method to be called with a BuildingData object
    public void PlaceBuilding(BuildingData buildingData)
    {
        if (currentBuilding != null)
        {
            Destroy(currentBuilding); // Destroy any existing building before placing a new one
        }

        // Instantiate the building prefab and set the current tile as its parent
        currentBuilding = Instantiate(buildingPrefab, canvasHandler.currentTile.transform);

        // Set the size of the building based on the BuildingData
        Vector3 newSize = new Vector3(buildingData.width, buildingData.height, buildingData.length);
        currentBuilding.transform.localScale = newSize;

        // Optionally, reset the building's local position to place it at the center of the tile
        currentBuilding.transform.localPosition = Vector3.zero;

        placingBuilding = true;
    }


    void UpdateBuildingPosition()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        int tileLayerMask = LayerMask.GetMask("Tile Layer"); // Get the layer mask for the tile layer

        // Check if the ray hits something (such as a ground plane)
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
        {
            // Get the point where the ray hits and round the x and z positions to nearest integers
            float xPos = Mathf.Round(hit.point.x);
            float zPos = Mathf.Round(hit.point.z);

            // Update the building's position
            currentBuilding.transform.position = new Vector3(xPos, currentBuilding.transform.position.y, zPos);

            // Check if the placement is valid
            CheckValidPlacement();

            if (Input.GetMouseButtonDown(0) && isPlacementValid)
            {
                canvasHandler.CompleteBuildingPlacement();
            }
        }

        // Optionally, destroy the building if right mouse button is clicked
        if (Input.GetMouseButtonDown(1) && currentBuilding != null)
        {
            placingBuilding = false;
            Destroy(currentBuilding);
        }
    }

    void CheckValidPlacement()
    {
        bool roadAdjacent = CheckRoadProximity();
        bool roadUnderBuilding = CheckOverlapWithRoads();
        bool isOverlapping = CheckOverlapWithBuildings(currentBuilding);
        bool aboveCurrentTile = CheckAboveCurrentTile();

        // Update the building's color based on the results of the checks
        if (roadAdjacent && !roadUnderBuilding && aboveCurrentTile && !isOverlapping)
        {
            isPlacementValid = true;
            currentBuilding.GetComponent<Renderer>().material.color = Color.green; // Optional visual feedback
        }
        else
        {
            isPlacementValid = false;
            currentBuilding.GetComponent<Renderer>().material.color = Color.red; // Optional visual feedback
        }
    }

    bool CheckRoadProximity()
    {
        Vector3 buildingPosition = currentBuilding.transform.position;
        Vector3[] adjacentPositions = new Vector3[]
        {
            buildingPosition + new Vector3(currentBuilding.transform.localScale.x/2 + 1, 0, 0), // Right
            buildingPosition + new Vector3(-currentBuilding.transform.localScale.x/2 - 1, 0, 0), // Left
            buildingPosition + new Vector3(0, 0, currentBuilding.transform.localScale.z/2 + 1), // Forward
            buildingPosition + new Vector3(0, 0, -currentBuilding.transform.localScale.z/2 - 1) // Backward
        };

        foreach (Vector3 adjacentPosition in adjacentPositions)
        {
            Collider[] colliders = Physics.OverlapSphere(adjacentPosition, 0.1f, roadLayer);
            if (colliders.Length > 0)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckOverlapWithRoads()
    {
        Vector3 buildingPosition = currentBuilding.transform.position;
        Vector3[] overlappingPositions = new Vector3[]
        {
            buildingPosition + new Vector3(currentBuilding.transform.localScale.x/2, 0, 0), // Right
            buildingPosition + new Vector3(-currentBuilding.transform.localScale.x/2, 0, 0), // Left
            buildingPosition + new Vector3(0, 0, currentBuilding.transform.localScale.z/2), // Forward
            buildingPosition + new Vector3(0, 0, -currentBuilding.transform.localScale.z/2) // Backward
        };

        foreach (Vector3 overlappingPosition in overlappingPositions)
        {
            Collider[] collidersUnder = Physics.OverlapSphere(overlappingPosition, 0.1f, roadLayer);
            if (collidersUnder.Length > 0)
            {
                return true;
            }
        }
        return false;
    }

    bool CheckAboveCurrentTile()
    {
        float distanceX = Mathf.Abs(currentBuilding.transform.position.x + currentBuilding.transform.localScale.x/2 - currentTile.transform.position.x - currentBuilding.transform.localScale.x/2);
        float distanceZ = Mathf.Abs(currentBuilding.transform.position.z + currentBuilding.transform.localScale.z/2 - currentTile.transform.position.z - currentBuilding.transform.localScale.z/2);
        
        return distanceX <= 5 - currentBuilding.transform.localScale.x / 2 && distanceZ <= 5 - currentBuilding.transform.localScale.z / 2;
    }

    bool CheckOverlapWithBuildings(GameObject objectToCheck)
    {
        GameObject[] buildingObjects = GameObject.FindGameObjectsWithTag("building");
        Vector3 objectPosition = objectToCheck.transform.position;
        Vector3 objectScale = objectToCheck.transform.localScale;

        foreach (GameObject building in buildingObjects)
        {
            if (building == objectToCheck) continue;

            Vector3 buildingPosition = building.transform.position;
            Vector3 buildingScale = building.transform.localScale;

            float overlapDistanceX = (objectScale.x + buildingScale.x) / 2 - 0.1f;
            float overlapDistanceZ = (objectScale.z + buildingScale.z) / 2 - 0.1f;

            float distanceX = Mathf.Abs(objectPosition.x - buildingPosition.x);
            float distanceZ = Mathf.Abs(objectPosition.z - buildingPosition.z);

            if (distanceX <= overlapDistanceX && distanceZ <= overlapDistanceZ)
            {
                return true;
            }
        }
        return false;
    }


    public void MakeBuildingPermanent()
    {
        currentBuilding.GetComponent<Renderer>().material.color = currentBuildingColor;
        // Disable movement and make the building permanent by removing this script from the object
        currentBuilding.GetComponent<Collider>().enabled = true; // Optionally enable the collider
        
        
        BuildingInfo buildingInfo = currentBuilding.GetComponent<BuildingInfo>();
        canvasHandler.UpdateBuildingInfo(buildingInfo);
        currentBuilding = null; // Clear the current building so it no longer follows the mouse
        placingBuilding = false;
    }
}
