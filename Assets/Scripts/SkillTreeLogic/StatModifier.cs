using UnityEngine;

[System.Serializable]
public class StatModifier
{
    // --- Útok / damage ---
    public int flatDamage;           // Přidává fixní damage
    public float percentDamage;      // Přidává % k damage
    public float critChance;         // Šance na kritický zásah (%)
    public float critMultiplier;     // Multiplikátor kritického zásahu

    // --- Obrana / resistence ---
    public int flatDefence;          // Přidává fixní obranu
    public float percentDefence;     // Přidává % k obranným statům
    public float blockChance;        // Šance na zablokování útoku (%)

    // --- Odolnosti proti elementům ---
    public int resistFire;           // Přidává bonus k fire resist
    public int resistIce;            // Přidává bonus k ice resist
    public int resistLightning;      // Přidává bonus k lightning resist

    // --- Zdraví / Mana / Stamina ---
    public int flatHealth;           // Přidává fixní HP
    public float percentHealth;      // Přidává % k max HP
    public int flatMana;             // Přidává fixní MANA
    public float percentMana;        // Přidává % k max MANA
    public int flatStamina;          // Přidává fixní Staminu
    public float percentStamina;     // Přidává % k max Stamině

    // --- Pohyblivost / utility ---
    public float dodgeChance;        // Šance vyhnutí útoku (%)
    public float movementSpeed;      // Bonus k rychlosti pohybu
    public float attackSpeed;        // Bonus k rychlosti útoku (např. %)

    // --- Skill / class specifické ---
    public float swordMastery;       // Bonus k sword damage
    public float axeMastery;         // Bonus k axe damage
    public float maceMastery;        // Bonus k mace damage
    public float bowMastery;         // Bonus k lukům
    public float daggerMastery;      // Bonus k dýkám
    public float crossbowMastery;    // Bonus k kuším
    public float fireMastery;
    public float iceMastery;
    public float lightningMastery;
}