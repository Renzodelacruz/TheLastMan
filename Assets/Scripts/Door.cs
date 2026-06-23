using UnityEngine;

public class Door : MonoBehaviour
{
    public int requiredKeys = 4;
    public DoorUI doorUI;

    private bool isOpen = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isOpen)
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                int currentKeys = inventory.GetKeyCount();
                int missingKeys = requiredKeys - currentKeys;

                Debug.Log("Llaves actuales: " + currentKeys);

                if (currentKeys >= requiredKeys)
                {
                    OpenDoor();
                    doorUI.ShowMessage("Puerta abierta");
                }
                else
                {
                    doorUI.ShowMessage("Te faltan " + missingKeys + " llaves");
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            doorUI.HideMessage();
        }
    }

    void OpenDoor()
    {
        isOpen = true;
        Debug.Log("Puerta abierta");
        gameObject.SetActive(false);
    }
}