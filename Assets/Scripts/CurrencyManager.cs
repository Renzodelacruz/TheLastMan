using UnityEngine;

public class CurrencyManager : MonoBehaviour
{
    public static CurrencyManager Instance;

    public int coins = 0;

    void Awake()
    {
        Instance = this;
    }

    public void AddCoins(int amount)
    {
        coins += amount;
        Debug.Log("Coins: " + coins);
    }

    public bool SpendCoins(int amount)
    {
        if (coins >= amount)
        {
            coins -= amount;
            Debug.Log("Spent: " + amount + " | Remaining: " + coins);
            return true;
        }

        Debug.Log("Not enough coins");
        return false;
    }
}