using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Item[] allItems;

    public InventorySlot[] inventorySlots;

    public InventorySlot[] toolSlots;
    public InventorySlot[] armorSlots;
    public InventorySlot[] weaponSlots;

    public GameObject inventoryItemPrefab;

    public bool isFull = false;

    private void Start()
    {
        
    }

    public bool AddItem(Item item, bool isDropped = false)
    {
        for(int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>(true);
            if(itemInSlot == null)
            {
                Debug.Log("index: "+i);
                SpawnNewItem(item, slot, isDropped);
                return true;
            }
            else
            {
                Debug.Log("At index " + i + itemInSlot.item.itemName);
            }
        }
        return false;


    }



    public void SpawnNewItem(Item item, InventorySlot slot, bool isDropped = false)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
        inventoryItem.InitialiseItem(item);
        inventoryItem.isDropped = isDropped;
    }
    public void NewRandomItem(bool isDropped = false)
    {
        if (allItems.Length == 0) return;
        Item item = allItems[Random.Range(0, allItems.Length)];
        AddItem(item, isDropped);
    }

    public void FillInventory(List<Item> items, bool isDropped = false)
    {
        foreach(var item in items)
        {
            if(!AddItem(item, isDropped)) return;
        }
    }
    public bool IsFull()
    {
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>(true);
            if (itemInSlot == null)
            {
                return false;
            }
            
        }
        return true;
    }
    public void ClearInventory()
    {
        void ClearSlots(InventorySlot[] slots)
        {
            foreach (InventorySlot slot in slots)
            {
                foreach (Transform child in slot.transform)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        ClearSlots(inventorySlots);
        ClearSlots(toolSlots);
        ClearSlots(armorSlots);
    }
    public List<DroppedItemInstance> GetAllItemsAsDroppedInstances(Position dropPosition)
    {
        List<DroppedItemInstance> result = new List<DroppedItemInstance>();

        // Projdeme pouze inventorySlots
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            InventoryItem itemInSlot = slot.GetComponentInChildren<InventoryItem>();
            if (itemInSlot != null)
            {
                result.Add(new DroppedItemInstance(itemInSlot, dropPosition));
            }
        }
        return result;
    }


}
