using TMPro;
using UnityEngine;

public class CoinsUI : MonoBehaviour
{
    public TextMeshProUGUI coinsText;

    void Start()
    {
        Debug.Log(" CoinsUI STARTED");

        if (coinsText == null)
        {
            Debug.LogError(" coinsText no asignado");
        }

        if (CurrencyManager.Instance == null)
        {
            Debug.LogError(" CurrencyManager.Instance es NULL");
        }
    }

    void Update()
    {
        if (coinsText == null) return;

        if (CurrencyManager.Instance == null)
        {
            coinsText.text = "NO MANAGER";
            return;
        }

        coinsText.text = "Coins: " + CurrencyManager.Instance.coins;
    }
}