using System.Collections;
using UnityEngine;

public class IncomeManager : MonoBehaviour
{
    public float moneyPerSecond = 0.1f;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        StartCoroutine(CalculateIncome());
    }

    private IEnumerator CalculateIncome()
    {
        while (true)
        {
            int totalHappyResidents = CalculateHappyResidents();
            int totalResidents = CountTotalResidents(); // New method to count total residents
            int unhappyResidents = totalResidents - totalHappyResidents; // Calculate unhappy residents

            // Calculate income: happy residents earn full money, unhappy residents earn half
            float moneyIncrease = (totalHappyResidents * moneyPerSecond) + (unhappyResidents * moneyPerSecond * 0.5f);
            player.AddMoney(moneyIncrease);
            yield return new WaitForSeconds(1f);
        }
    }

    private int CalculateHappyResidents()
    {
        int happyResidentsCount = 0;
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");

        foreach (GameObject buildingObj in buildings)
        {
            BuildingInfo building = buildingObj.GetComponent<BuildingInfo>();

            if (building != null && building.buildingType == BuildingType.Residential)
            {
                // Calculate happy residents based on available jobs and shopping space
                int jobsAvailable = CountAvailableJobs();
                int shoppingAvailable = CountAvailableShopping();

                // Residents are happy if they have both a job and shopping space
                building.happyResidents = Mathf.Min(building.residents, Mathf.Min(jobsAvailable, shoppingAvailable));
                happyResidentsCount += building.happyResidents;
            }
        }
        return happyResidentsCount;
    }

    private int CountTotalResidents() // New method to count total residents
    {
        int totalResidentsCount = 0;
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");

        foreach (GameObject buildingObj in buildings)
        {
            BuildingInfo building = buildingObj.GetComponent<BuildingInfo>();

            if (building != null && building.buildingType == BuildingType.Residential)
            {
                totalResidentsCount += building.residents; // Count all residents in residential buildings
            }
        }
        return totalResidentsCount;
    }

    private int CountAvailableJobs()
    {
        int totalJobs = 0;
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");

        foreach (GameObject buildingObj in buildings)
        {
            BuildingInfo building = buildingObj.GetComponent<BuildingInfo>();

            if (building != null)
            {
                if (building.buildingType == BuildingType.Commercial || building.buildingType == BuildingType.Industrial)
                {
                    totalJobs += building.jobs;
                }
            }
        }
        return totalJobs;
    }

    private int CountAvailableShopping()
    {
        int totalShopping = 0;
        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");

        foreach (GameObject buildingObj in buildings)
        {
            BuildingInfo building = buildingObj.GetComponent<BuildingInfo>();

            if (building != null && building.buildingType == BuildingType.Commercial)
            {
                totalShopping += building.shoppingSpace;
            }
        }
        return totalShopping;
    }
}
