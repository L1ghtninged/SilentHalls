using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public abstract class Item : ScriptableObject
{
    [Header("Basic Item Settings")]
    public string itemName = "New Item";
    public ItemType itemType;
    [TextArea] public string description;
    public Sprite sprite; 
    /*public bool isStackable = false;
    public int maxStack = 1;
    */
    [Header("Common Stats")]
    public int manaCost = 0;
    public int healthEffect = 0;
    public int manaEffect = 0;


    public abstract void Use(ScriptManager player);

    public virtual string GetTooltip()
    {
        string tooltip = $"<b>{itemName}</b>\n{description}\n";

        if (manaCost > 0) tooltip += $"Mana Cost: {manaCost}\n";
        if (healthEffect != 0) tooltip += $"Health: {healthEffect}\n";
        if (manaEffect != 0) tooltip += $"Mana: {manaEffect}\n";

        return tooltip;
    }

}
public enum ItemType
{
    None,
    Weapon,
    Potion,
    Armor,
    Key,
    Food



}