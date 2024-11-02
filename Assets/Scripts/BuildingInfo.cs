using UnityEngine;

public enum BuildingType { Residential, Commercial, Industrial }

public class BuildingInfo : MonoBehaviour
{
    public BuildingType buildingType;
    public int residents;         // Only for Residential
    public int happyResidents;    // Only for Residential
    public int jobs;              // For Commercial and Industrial
    public int shoppingSpace;     // Only for Commercial
}
