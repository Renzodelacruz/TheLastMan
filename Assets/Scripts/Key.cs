using UnityEngine;

public class Key : MonoBehaviour
{
    public int keyID;
    public DoorUI doorUI; // referencia a la UI

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory inventory = other.GetComponent<PlayerInventory>();

            if (inventory != null)
            {
                inventory.AddKey(keyID);

                // 👉 MENSAJE SIMPLE
                if (doorUI != null)
                {
                    doorUI.ShowMessage("Llave recogida");
                    Invoke(nameof(HideUI), 2f);
                }

                Destroy(gameObject);
            }
        }
    }

    void HideUI()
    {
        doorUI.HideMessage();
    }
}