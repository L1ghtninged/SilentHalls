using System.Collections.Generic;
using UnityEngine;

public class GroundItemsVisual : MonoBehaviour
{
    public GameObject bagPrefab; // Prefab sáèku/bedny
    public float spawnOffset = 0f;
    public float cellSize = 5f;

    private Dictionary<Position, GameObject> activeBags = new Dictionary<Position, GameObject>();
    public DropItemScript dropItemScript;
    public InputManagerScript inputManager;
    private void Start()
    {
        dropItemScript = GetComponent<DropItemScript>();
        if (dropItemScript == null)
        {
            Debug.LogError("DropItemScript not found!");
            return;
        }
    }

    public void UpdateBagsVisibility()
    {
        // Získat všechny unikátní pozice s pøedmìty
        HashSet<Position> positionsWithItems = GetPositionsWithItems();

        // Vytvoøit sáèky na nových pozicích
        foreach (var position in positionsWithItems)
        {
            if (!activeBags.ContainsKey(position))
            {
                CreateBagAtPosition(position);
            }
        }

        // Odstranit sáèky na pozicích bez pøedmìtù
        RemoveEmptyBags(positionsWithItems);
    }

    private HashSet<Position> GetPositionsWithItems()
    {
        var positions = new HashSet<Position>();
        foreach (var item in dropItemScript.droppedItems)
        {
            positions.Add(item.position);
        }
        return positions;
    }

    private void CreateBagAtPosition(Position position)
    {
        Vector3 worldPosition = PositionToWorld(position);
        GameObject bag = Instantiate(bagPrefab, worldPosition, Quaternion.identity);
        activeBags[position] = bag;
    }

    private void RemoveEmptyBags(HashSet<Position> validPositions)
    {
        List<Position> positionsToRemove = new List<Position>();

        foreach (var kvp in activeBags)
        {
            if (!validPositions.Contains(kvp.Key))
            {
                Destroy(kvp.Value);
                positionsToRemove.Add(kvp.Key);
            }
        }

        foreach (var position in positionsToRemove)
        {
            activeBags.Remove(position);
        }
    }

    private Vector3 PositionToWorld(Position position)
    {
        return new Vector3(
            position.x * cellSize,
            position.z + spawnOffset,
            position.y * cellSize
        );
    }

    // Volá se pøi zmìnì poètu pøedmìtù na pozici
    public void OnItemsChangedAtPosition(Position position)
    {
        int itemCount = dropItemScript.DroppedItemsFiltered(position).Count;

        if (itemCount == 0 && activeBags.ContainsKey(position))
        {
            Destroy(activeBags[position]);
            activeBags.Remove(position);
        }
        else if (itemCount > 0 && !activeBags.ContainsKey(position))
        {
            CreateBagAtPosition(position);
        }
    }
}