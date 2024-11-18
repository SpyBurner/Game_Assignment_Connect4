using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [SerializeField] public Dictionary<Vector2Int, Slot> slotDictionary = new Dictionary<Vector2Int, Slot>();

    
    void Update()
    {
        foreach (Slot slot in FindObjectsOfType<Slot>())
        {
            Vector2Int coords = new Vector2Int(slot.x, slot.y);
            if (!slotDictionary.ContainsKey(coords))
            {
                slotDictionary.Add(coords, slot);
                Debug.Log(slot);
            }
        }
    }

}
