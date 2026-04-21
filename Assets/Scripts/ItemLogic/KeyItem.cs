using UnityEngine;

[CreateAssetMenu(fileName = "New Key", menuName = "Inventory/Key")]
public class KeyItem : Item
{
    public KeyType keyType;
    public enum KeyType {Gold, Silver}

    private void OnEnable()
    {
        itemType = ItemType.Key;
    }

    public override void Use(ScriptManager player)
    {
        // Implementace použití klíče
    }
}