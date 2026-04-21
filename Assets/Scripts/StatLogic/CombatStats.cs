using UnityEngine;

[System.Serializable]
public class CombatStats
{
    // --- Damage & Defence ---
    public float BaseDamage = 1f;
    public int Defence = 0;

    // --- Critical / Block / Dodge ---
    public float CritChance = 0f;      // šance na kritický úder v procentech
    public float BlockChance = 0f;     // šance na zablokování útoku
    public float DodgeChance = 0f;     // šance na únik před útokem

    // --- Elemental Resistances ---
    public int ResistFire = 0;
    public int ResistIce = 0;
    public int ResistLightning = 0;

    // --- Health / Mana / Stamina ---
    public int MaxHealth = 0;
    public int MaxMana = 0;
    public int MaxStamina = 0;

    // --- Movement / Attack ---
    public float MovementSpeed = 0f;
    public float AttackSpeed = 0f;

    // --- Weapon Masteries (Fighter / Rogue / Mage) ---
    public float SwordMastery = 0f;
    public float AxeMastery = 0f;
    public float MaceMastery = 0f;
    public float BowMastery = 0f;
    public float DaggerMastery = 0f;
    public float CrossbowMastery = 0f;
    public float FireMastery = 0f;
    public float IceMastery = 0f;
    public float LightningMastery = 0f;

    // --- Helper method pro debugging ---
    public override string ToString()
    {
        return $"Damage: {BaseDamage}, Defence: {Defence}, Crit: {CritChance}, Block: {BlockChance}, Dodge: {DodgeChance}";
    }
}