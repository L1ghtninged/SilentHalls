using UnityEngine;
using TMPro;

public class InventoryStatsUI : MonoBehaviour
{
    [Header("References")]
    public StatScript stats;

    [Header("Base")]
    public TextMeshProUGUI levelText;

    [Header("Resources")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI staminaText;

    [Header("Combat")]
    public TextMeshProUGUI defenceText;
    public TextMeshProUGUI movementSpeedText;

    [Header("Resistances")]
    public TextMeshProUGUI fireResText;
    public TextMeshProUGUI iceResText;
    public TextMeshProUGUI lightningResText;

    void Start()
    {
        if (stats == null)
            stats = GameObject.FindGameObjectWithTag("Player").GetComponent<StatScript>();

        Refresh();
    }

    public void Refresh()
    {
        CombatStats finalStats = stats.GetCombatStats();

        // BASE
        levelText.text = $"Level: {stats.level}";

        // RESOURCES
        healthText.text = $"Health: {stats.Health.CurrentValue}/{stats.Health.MaxValue}";
        manaText.text = $"Mana: {stats.Mana.CurrentValue}/{stats.Mana.MaxValue}";
        staminaText.text = $"Stamina: {stats.Stamina.CurrentValue}/{stats.Stamina.MaxValue}";

        // COMBAT
        defenceText.text = $"Defence: {finalStats.Defence}";
        movementSpeedText.text = $"Move Speed: {finalStats.MovementSpeed:F2}";

        // RESIST
        fireResText.text = $"Fire Resist: {finalStats.ResistFire}%";
        iceResText.text = $"Ice Resist: {finalStats.ResistIce}%";
        lightningResText.text = $"Lightning Resist: {finalStats.ResistLightning}%";
    }
}