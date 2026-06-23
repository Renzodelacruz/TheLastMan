using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    private List<int> keys = new List<int>();

    public void AddKey(int keyID)
    {
        keys.Add(keyID);
    }

    public int GetKeyCount()
    {
        return keys.Count;
    }
}