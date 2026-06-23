using UnityEngine;
using TMPro;

public class DoorUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public GameObject panel;

    public void ShowMessage(string message)
    {
        panel.SetActive(true);
        messageText.text = message;
    }

    public void HideMessage()
    {
        panel.SetActive(false);
    }
}