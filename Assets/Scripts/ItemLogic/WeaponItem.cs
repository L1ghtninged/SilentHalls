using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Inventory/Weapon")]
public class WeaponItem : Item
{
    [Header("Weapon Specific Stats")]
    public const int probabilityConstant = 25;
    public int lowValue;
    public int highValue;
    public float attackSpeed = 1f;
    public DamageType damageType;
    public MagicType magicType;
    public int critChance = 2;
    public WeaponType weaponType;
    public GameObject spellPrefab;

    public enum DamageType {Physical, Ranged, Magical}
    public enum MagicType {None, Fire, Ice, Lightning}

    private void OnEnable()
    {
        itemType = ItemType.Weapon;
    }

    public override void Use(ScriptManager manager)
    {
        if (manager.gameUIManager.isInventoryOpened)
        {
            Equip();
            return;
        }

        var attackScript = GameObject
            .FindGameObjectWithTag("Player")
            .GetComponent<AttackScript>();

        attackScript.PerformAttack(this);
    }

    public override string GetTooltip()
    {
        string tooltip = base.GetTooltip();
        tooltip += $"Attack: {lowValue}-{highValue}\n";
        tooltip += $"Attack Speed: {attackSpeed}\n";
        return tooltip;
    }
    public void Equip()
    {

    }
    

}
public enum WeaponType
{
    None,
    Sword,
    Axe,
    Mace,
    Bow,
    Dagger,
    Staff,
    Crossbow
}

