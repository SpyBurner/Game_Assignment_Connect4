using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
public class BoardManager : MonoBehaviour
{
    public Dictionary<Vector2Int, Slot> slotDictionary = new Dictionary<Vector2Int, Slot>();
    // Start is called before the first frame update
    void Update()
    {
        if (slotDictionary.Count != 42)
        {
            foreach (Slot slot in FindObjectsOfType<Slot>())
            {
                Vector2Int slotPosition = new Vector2Int(slot.x, slot.y);
                if (!slotDictionary.ContainsKey(slotPosition))
                {
                    slotDictionary.Add(slotPosition, slot);

                }
            }
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

    public Player CheckForFourInARow(Slot checkSlot)
    {
        if (checkSlot == null || checkSlot.occupyingPlayer == null)
            return null;

        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(1, 0),   
        new Vector2Int(0, 1),   
        new Vector2Int(1, 1),  
        new Vector2Int(1, -1)   
        };

        Vector2Int checkSlotPosition = new Vector2Int(checkSlot.x, checkSlot.y);
        Player occupier = checkSlot.occupyingPlayer;

        foreach (Vector2Int direction in directions)
        {
            int count = 1; 

            count += CountSlotsInDirection(checkSlotPosition, direction, occupier);
            count += CountSlotsInDirection(checkSlotPosition, -direction, occupier);
            if (count >= 4)
            {
                Debug.Log(occupier);
                return occupier; 
            }
        }

        return null;
    }

    private int CountSlotsInDirection(Vector2Int startPosition, Vector2Int direction, Player occupier)
    {
        Vector2Int nextPos = startPosition + direction;
        if (slotDictionary.ContainsKey(nextPos))
        {
            if (slotDictionary[nextPos].occupyingPlayer == occupier) return 1 + CountSlotsInDirection(nextPos, direction,occupier);
        }
        return 0;
    }
}
