using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tilePrefab; // The tile prefab to be instantiated
    public int gridWidth = 100; // Number of tiles horizontally
    public int gridHeight = 100; // Number of tiles vertically
    public float tileSpacing = 1.1f; // Spacing between tiles

    void Start()
    {
        // Instantiate tiles in a grid layout
        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 tilePosition = new Vector3(x * tileSpacing, 0, y * tileSpacing); // Position tiles
                Instantiate(tilePrefab, tilePosition, Quaternion.identity); // Instantiate tile from prefab
            }
        }
    }
}
