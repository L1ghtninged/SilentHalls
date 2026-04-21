using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DropItemScript : InventorySlot, IDropHandler
{
    public List<DroppedItemInstance> droppedItems = new List<DroppedItemInstance>();
    private GameObject playerObject;
    private PositionScript positionScript;
    public DroppedItemsUI droppedItemsUI;
    public Inventory inventory;
    public GroundItemsVisual visuals;
    public int maxCount = 24;
    void Start()
    {
        playerObject = GameObject.FindGameObjectWithTag("Player");
        
        if (playerObject == null)
        {
            Debug.LogError("Player object not found! Make sure it's tagged 'Player'.");
            return;
        }

        positionScript = playerObject.GetComponent<PositionScript>();
        if (positionScript == null)
        {
            Debug.LogError("PositionScript not found on Player object!");
            return;
        }
    }

    public new void OnDrop(PointerEventData eventData)
    {
        if (inventory.IsFull()) return;

        GameObject dropped = eventData.pointerDrag;
        InventoryItem draggedItem = dropped.GetComponent<InventoryItem>();
        if (draggedItem.isDropped) return; 

        if(draggedItem.type == ItemType.Armor)
        {
            ArmorItem armorItem = draggedItem.item as ArmorItem;
            if (draggedItem.isEquipped) armorItem.UnEquip(draggedItem);
        }

        draggedItem.parentAfterDrag = transform;
        draggedItem.transform.localPosition = Vector3.zero;
        

        Position playerPosition = new Position();
        playerPosition.x = positionScript.x;
        playerPosition.y = positionScript.y;
        playerPosition.z = positionScript.z;
        AddDroppedItem(draggedItem, playerPosition);
        visuals.UpdateBagsVisibility();
        UpdateUI(true);
        Destroy(dropped);
    }
    public void AddDroppedItem(InventoryItem item, Position position)
    {
        droppedItems.Add(new DroppedItemInstance(item, position));
        item.isDropped = true;
        visuals.OnItemsChangedAtPosition(position);
        GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>().AddItem(item.item, position.x, position.y);
    }
    public void AddDroppedItemOnStart(InventoryItem item, Position position)
    {
        droppedItems.Add(new DroppedItemInstance(item, position));
        item.isDropped = true;
        visuals.OnItemsChangedAtPosition(position);
        visuals.UpdateBagsVisibility();
    }
    public int CountItemsOnFloor()
    {
        Position playerPosition = new Position();
        if(positionScript == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
            positionScript = playerObject.GetComponent<PositionScript>();
        }
        playerPosition.x = positionScript.x;
        playerPosition.y = positionScript.y;
        playerPosition.z = positionScript.z;
        

        return DroppedItemsFiltered(playerPosition).Count;
    }
    
    public List<DroppedItemInstance> DroppedItemsFiltered(Position position)
    {
        List<DroppedItemInstance> droppedItemInstances = new List<DroppedItemInstance>();

        foreach(DroppedItemInstance item in droppedItems)
        {
            if(item.position == position) droppedItemInstances.Add(item);
        }


        return droppedItemInstances;
    }

    public void UpdateUI(bool clearInventory = true)
    {
        if (inventory == null)
        {
            inventory = droppedItemsUI.inventory;
        }

        Position playerPosition = GetPlayerPosition();
        List<DroppedItemInstance> itemsOnFloor = DroppedItemsFiltered(playerPosition);
        int count = CountItemsOnFloor();
        droppedItemsUI.floorText.gameObject.SetActive(count > 0);
        droppedItemsUI.floorText.text = "You found " + count + " item(s)";
        if (clearInventory)
        {
            inventory.ClearInventory();
            // Poèkejte na dokonèení destruktivní operace (napø. pomocí Coroutine)
            StartCoroutine(WaitAndFill(itemsOnFloor));
        }
        Debug.Log("Total number of items" + droppedItems.Count);

    }

    private IEnumerator WaitAndFill(List<DroppedItemInstance> items)
    {
        yield return new WaitForEndOfFrame(); // Poèká na dokonèení Destroy
        inventory.FillInventory(GetListOfItems(items), true);
    }
    private List<Item> GetListOfItems(List<InventoryItem> inventoryItems)
    {
        var list = new List<Item>();
        foreach(InventoryItem inventoryItem in inventoryItems)
        {
            list.Add(inventoryItem.item);
        }

        return list;
    }
    private List<Item> GetListOfItems(List<DroppedItemInstance> instances)
    {
        var list = new List<Item>();
        foreach (DroppedItemInstance instance in instances)
        {
            list.Add(instance.item.item);
        }

        return list;
    }

    public Position GetPlayerPosition()
    {
        if(playerObject == null) playerObject = GameObject.FindGameObjectWithTag("Player");
        if (positionScript == null) positionScript = playerObject.GetComponent<PositionScript>();
        return new Position
        {
            x = positionScript.x,
            y = positionScript.y,
            z = positionScript.z
        };
    }



}

public class DroppedItemInstance
{
    public InventoryItem item;
    public Position position;
    public DroppedItemInstance(InventoryItem item, Position position)
    {
        this.item = item;
        this.position = position;
    }
    public override string ToString()
    {
        return $"DroppedItemInstance (Item: {item?.item?.itemName})";
    }
}