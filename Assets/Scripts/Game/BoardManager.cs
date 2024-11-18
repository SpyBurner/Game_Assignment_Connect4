using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Slot> slotDictionary = new Dictionary<Vector2Int, Slot>();
    // Start is called before the first frame update
    void Start()
    {

    }

    void Update()
    {
        if (slotDictionary.Count < 6 * 7)
        {
            foreach (Slot slot in FindObjectsOfType<Slot>())
            {
                Vector2Int slotPosition = new Vector2Int(slot.x, slot.y);
                if (!slotDictionary.ContainsKey(slotPosition))
                {
                    slotDictionary.Add(slotPosition, slot);
                }
            }
            Debug.Log("BoardManager: " + slotDictionary.Count);
        }
    }

    public Slot GetSlotAt(Vector2Int position)
    {
        if (slotDictionary.ContainsKey(position))
        {
            return slotDictionary[position];
        }
        return null;
    }
}
