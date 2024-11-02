using UnityEngine;

public class Player : MonoBehaviour
{
    public float money = 1000; // Initial amount of money the player has

    public void SubtractMoney(float amount)
    {
        money -= amount;
    }

    public void AddMoney(float amount)
    {
        money += amount;
    }

    public float GetMoney()
    {
        return money;
    }
}
