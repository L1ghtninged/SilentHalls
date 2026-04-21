using UnityEngine;
using static PotionItem;

[CreateAssetMenu(fileName = "New Armor", menuName = "Inventory/Armor")]
public class ArmorItem : Item
{
    public enum ArmorSlot { Helmet, ChestPlate, Leggins, Boots};
    [Header("Armor Specific Stats")]
    public ArmorSlot slot;
    public int defence = 5;
    public int resistIce;
    public int resistFire;

    private StatScript stats;
    private void OnEnable()
    {
        itemType = ItemType.Armor;
    }

    public override void Use(ScriptManager manager)
    {
        /*
        if (!isEquipped) 
        {

            Equip();
        }
        else {

            UnEquip();
        }
        */
    }

    public override string GetTooltip()
    {
        string tooltip = base.GetTooltip();
        tooltip += $"Defense: {defence}\n";
        if(resistFire != 0)tooltip += $"Resist fire: {resistFire}\n";
        if(resistIce != 0) tooltip += $"Resist ice: {resistIce}\n";
        
        return tooltip;
    }
    public void Equip(InventoryItem inventoryItem)
    {
        if (stats == null) stats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatScript>();
        stats.resistFire += resistFire;
        stats.resistIce += resistIce;
        stats.ModifyDefence(defence);
        inventoryItem.isEquipped = true;

    }
    public void UnEquip(InventoryItem inventoryItem)
    {
        if (stats == null) stats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatScript>();
        stats.resistFire -= resistFire;
        stats.resistIce -= resistIce;
        stats.ModifyDefence(-defence);
        inventoryItem.isEquipped = false;
    }
}
