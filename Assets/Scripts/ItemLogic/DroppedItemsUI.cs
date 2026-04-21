using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DroppedItemsUI : MonoBehaviour
{
    public TextMeshProUGUI floorText;
    public Inventory inventory;
    private void Start()
    {
        inventory = GetComponent<Inventory>();        
    }

    public void UnsetFloorText()
    {
        bool isFloorTextActive = floorText.gameObject.activeInHierarchy;
        floorText.gameObject.SetActive(!isFloorTextActive);
    }

    public void LoadInventory(List<Item> items)
    {
        inventory.ClearInventory();
        inventory.FillInventory(items, true);
    }

}
