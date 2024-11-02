using UnityEngine;

[CreateAssetMenu(fileName = "BuildingData", menuName = "ScriptableObjects/BuildingData", order = 1)]
public class BuildingData : ScriptableObject
{
    public string buildingName;
    public int length;
    public int width;
    public int height;
    public int cost;
    public string description;
    public Color color;

    public BuildingType buildingType;
    public int residents;
    public int jobs;
    public int shoppingSpace;
}
