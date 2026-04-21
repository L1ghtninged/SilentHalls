using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using static ArmorItem;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public ItemType[] allowedItemTypes;
    public ArmorSlot[] allowedArmorTypes;
    public SlotType slotType = SlotType.None;
    public void OnDrop(PointerEventData eventData)
    {
        if (slotType == SlotType.Dropped) return;
        GameObject dropped = eventData.pointerDrag;
        InventoryItem draggedItem = dropped.GetComponent<InventoryItem>();
        if (!draggedItem.isBeingDragged) return;
        if (!AllowedTypeDrop(draggedItem.type)) return;
        if (!IsCorrectEquipment(draggedItem.item)) return;

        if (transform.childCount > 0)
        {
            Transform existingItem = transform.GetChild(0);
            InventoryItem existingInventoryItem = existingItem.GetComponent<InventoryItem>();
            
            // Pokud je to ArmorItem, zavoláme UnEquip
            if (existingInventoryItem.type == ItemType.Armor)
            {
                ArmorItem existingArmor = existingInventoryItem.item as ArmorItem;
                if (draggedItem.isEquipped)
                {
                    if (draggedItem.type != ItemType.Armor) return;
                    ArmorItem draggedArmor = draggedItem.item as ArmorItem;
                    
                    if (!AreSameEquipment(existingArmor, draggedArmor)) return;
                    existingArmor.Equip(existingInventoryItem);
                    draggedArmor.UnEquip(draggedItem);
                }
                else if(existingInventoryItem.isEquipped)
                {
                    ArmorItem draggedArmor = draggedItem.item as ArmorItem;
                    existingArmor.UnEquip(existingInventoryItem);
                    draggedArmor.Equip(draggedItem);
                }
                
                
            }
            else
            {
                if (draggedItem.type == ItemType.Armor)
                {
                    if (draggedItem.isEquipped) return;
                }
            }
            
            existingItem.SetParent(draggedItem.parentAfterDrag);
            existingItem.localPosition = Vector3.zero;
            
        }
        else if (SpecificType())
        {
            if(draggedItem.type == ItemType.Armor)
            {
                ArmorItem draggedArmor = draggedItem.item as ArmorItem;
                if(!draggedItem.isEquipped)draggedArmor.Equip(draggedItem);
            }
        }
        else
        {
            if (draggedItem.type == ItemType.Armor)
            {
                ArmorItem draggedArmor = draggedItem.item as ArmorItem;
                if (draggedItem.isEquipped) draggedArmor.UnEquip(draggedItem);
            }
        }
        draggedItem.parentAfterDrag = transform;
        draggedItem.transform.localPosition = Vector3.zero;

    }

    public bool AllowedTypeDrop(ItemType type)
    {
        if (allowedItemTypes.Length == 0) return true;
        foreach (ItemType item in allowedItemTypes)
        {
            if (type == item) return true;
        }
        return false;
    }
    public bool AllowedArmorType(ArmorSlot type)
    {
        if (allowedArmorTypes.Length == 0) return true;
        foreach (ArmorSlot item in allowedArmorTypes)
        {
            if (type == item) return true;
        }
        return false;
    }
    public bool SpecificType()
    {
        return allowedItemTypes.Length > 0;
    }
    public bool IsCorrectEquipment(Item item)
    {
        if(SpecificType() && item.itemType == ItemType.Armor)
        {
            ArmorItem draggedArmor = item as ArmorItem;
            return AllowedTypeDrop(item.itemType) && AllowedArmorType((item as ArmorItem).slot);
        }
        return AllowedTypeDrop(item.itemType);
    }
    public bool AreSameEquipment(ArmorItem item1, ArmorItem item2)
    {
        return item1.slot == item2.slot;
    }



}
public enum SlotType
{
    None,
    Dropped,
}