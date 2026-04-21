using UnityEngine;

[CreateAssetMenu(fileName = "New Potion", menuName = "Inventory/Potion")]
public class PotionItem : Item
{
    public float effectDuration = 5f;
    public bool isConsumable = true;

    private void OnEnable()
    {
        itemType = ItemType.Potion;
    }

    

    public override void Use(ScriptManager player)
    {
        if(isConsumable)
        {
            player.playerStats.Heal(healthEffect);
            player.playerStats.RestoreMana(manaEffect);
            SoundManager.Instance.PlayHeal();
        }
        
    }
}