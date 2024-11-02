using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public bool isOwned = false; // To track if the tile is owned
    public string ownerID = ""; // To track the player who owns the tile
    public int cost = 100; // Set a default cost for tiles
    public GameObject outlinePrefab; // The outline prefab to be instantiated
    private GameObject instantiatedOutline; // To track the instantiated outline
    public Color hoverOutlineColor; // Color when hovering
    public Color ownedOutlineColor; // Color for owned tiles
    public Color clickedOutlineColor; // Color for clicked tile
    public GameObject roadPrefab;  // Assign the road prefab in the Inspector
    bool tileClicked = false; // To track if the player clicks on the tile
    bool isHovering = false; // To track if the player is hovering over the tile

    void Start()
    {
        // Place roads at the start
        PlaceRoads();
    }

    void Update()
    {
        // Create a ray from the camera to the mouse position
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        // Layer mask to only hit the Tile Layer
        int tileLayerMask = LayerMask.GetMask("Tile Layer");

        // Check if the ray hits a tile
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tileLayerMask))
        {
            // If the ray hits this tile
            if (hit.collider.GetComponent<Tile>() == this)
            {
                    // Apply hover outline if the mouse is over this tile
                ApplyOutline(hoverOutlineColor);

                // Check for mouse click
                if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                {
                    // Notify the CanvasHandler to show the buy button
                    CanvasHandler canvasHandler = FindObjectOfType<CanvasHandler>();
                    if (canvasHandler != null)
                    {
                        canvasHandler.ShowBuyButtonForTile(this);
                        // Apply the hover outline when the tile is clicked
                        ApplyOutline(hoverOutlineColor);

                        // Reset tile clicked state for all tiles
                        Tile[] allTiles = FindObjectsOfType<Tile>();
                        foreach (Tile tile in allTiles)
                        {
                            tile.tileClicked = false;
                        }

                        tileClicked = true; // Set this tile as clicked
                    }
                }
            }
            else
            {
                // Reset outline if the mouse is not over this tile
                ResetOutline();
            }
        }
        else
        {
            // Reset outline if raycast does not hit a tile
            ResetOutline();
        }
        if (tileClicked)
        {
            ApplyOutline(clickedOutlineColor);
        }
        // Apply owned outline for owned tiles
        if (isOwned)
        {
            ApplyOutline(ownedOutlineColor);
        }
    }
    public void BuyTile()
    {
        isOwned = true;
        ownerID = "Player1"; // This would dynamically be set based on the player
        ApplyOutline(ownedOutlineColor); // Keep the owned outline
        ResetOutline(); // Make sure to reset any hover outlines if the tile is owned
    }

    void ApplyOutline(Color outlineColor)
    {
        if (instantiatedOutline != null)
        {
            // If the outline already exists, just update its color
            Renderer[] childRenderers = instantiatedOutline.GetComponentsInChildren<Renderer>();
            foreach (Renderer childRenderer in childRenderers)
            {
                childRenderer.material.color = outlineColor; // Change the existing outline color
            }
        }
        else if (outlinePrefab != null)
        {
            // If the outline does not exist, instantiate it
            Vector3 offsetPosition = transform.position + new Vector3(-5, 0.05f, 0); // Move 5 units to the left
            instantiatedOutline = Instantiate(outlinePrefab, offsetPosition, Quaternion.identity, transform);

            // Get all child renderers of the instantiated outline object and apply the initial color
            Renderer[] childRenderers = instantiatedOutline.GetComponentsInChildren<Renderer>();
            foreach (Renderer childRenderer in childRenderers)
            {
                childRenderer.material.color = outlineColor; // Set the initial outline color
            }
        }
    }

    void ResetOutline()
    {
        if (instantiatedOutline != null && !isOwned) // Only reset if it's not owned
        {
            // Destroy the instantiated outline when it's no longer needed
            Destroy(instantiatedOutline);
            instantiatedOutline = null;
        }
    }

    public int GetCost()
    {
        return cost;
    }

    void PlaceRoads()
    {
        // Create vertical road (along Z-axis, in the middle of the plane)
        Instantiate(roadPrefab, transform);

        // Create horizontal road (along X-axis, rotate by 90 degrees around Y-axis to lay flat horizontally)
        GameObject horizontalRoad = Instantiate(roadPrefab, transform);
        horizontalRoad.transform.rotation = Quaternion.Euler(0, 90, 0); // Rotate 90 degrees around Y-axis
    }

}
