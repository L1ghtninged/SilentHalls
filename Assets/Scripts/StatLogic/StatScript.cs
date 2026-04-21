using System;
using System.Collections.Generic;
using UnityEngine;

public class StatScript : MonoBehaviour
{
    public PlayerSkillTree skillTree;
    public SkillTreeScript skillTreeScript;
    [Header("Stamina Regeneration")]
    public float staminaRegenPerSecond = 5f;
    public float staminaRegenDelay = 2f;
    private float staminaRegenBuffer = 0f;

    private float lastStaminaUseTime;

    [Header("Base Attributes")]
    public int defence = 1;

    [Header("Resistances")]
    public int resistFire;
    public int resistIce;
    public int resistLightning;

    [Header("Progression")]
    public int healthBonus = 10;
    public int manaBonus = 10;
    public int staminaBonus = 10;
    public int level = 1;
    public int skillPoints = 0;

    [Header("Resources")]
    public StatResource Health;
    public StatResource Mana;
    public StatResource Stamina;
    public StatResource Experience;

    private InputManagerScript inputManager;

    void Awake()
    {
        inputManager = FindObjectOfType<InputManagerScript>();

        Health = new StatResource(100);
        Mana = new StatResource(100);
        Stamina = new StatResource(100);

        Experience = new StatResource(100);
        Experience.SetCurrentValue(0);

        Health.OnValueChanged += OnHealthChanged;
    }
    void Update()
    {
        RegenerateStamina();
    }
    private void RegenerateStamina()
    {
        if (Stamina.CurrentValue >= Stamina.MaxValue)
            return;

        if (Time.time < lastStaminaUseTime + staminaRegenDelay)
            return;

        staminaRegenBuffer += staminaRegenPerSecond * Time.deltaTime;

        if (staminaRegenBuffer >= 1f)
        {
            int amountToRestore = Mathf.FloorToInt(staminaRegenBuffer);
            staminaRegenBuffer -= amountToRestore;
            Stamina.Modify(amountToRestore);
        }
    }

    void OnHealthChanged(int current, int max)
    {
        if (current <= 0)
            Die();
    }

    public void TakeDamage(int damage)
    {
        int finalDamage = Mathf.Max(damage - defence, 1);
        Health.Modify(-finalDamage);
    }

    public void Heal(int amount) => Health.Modify(amount);
    public void UseMana(int amount) => Mana.Modify(-amount);
    public void UseStamina(int amount)
    {
        Stamina.Modify(-amount);
        lastStaminaUseTime = Time.time;
    }
    public void RestoreMana(int amount) => Mana.Modify(amount);

    void Die()
    {
        inputManager.GameOver();
    }

    public void GainExperience(int amount)
    {
        Experience.Modify(amount);

        while (Experience.CurrentValue >= Experience.MaxValue)
        {
            Experience.SetCurrentValue(Experience.CurrentValue - Experience.MaxValue);
            LevelUp();
        }
    }

    void LevelUp()
    {
        level++;
        skillPoints++;

        Health.SetMaxValue(Health.MaxValue + healthBonus);
        Mana.SetMaxValue(Mana.MaxValue + manaBonus);
        Stamina.SetMaxValue(Stamina.MaxValue + staminaBonus);

        skillTreeScript.UpdateUnAssignedPoints();
        SoundManager.Instance.PlayLevelUp();
    }

    public CombatStats GetCombatStats()
    {
        CombatStats finalStats = new CombatStats();

        foreach (var node in skillTree.skillLevels)
        {
            ApplyModifier(finalStats, node.Key.effects);
        }

        finalStats.Defence += defence;
        finalStats.ResistFire += resistFire;
        finalStats.ResistIce += resistIce;
        finalStats.ResistLightning += resistLightning;
        finalStats.MaxHealth += Health.MaxValue;
        finalStats.MaxMana += Mana.MaxValue;
        finalStats.MaxStamina += Stamina.MaxValue;

        return finalStats;
    }
    private void ApplyModifier(CombatStats stats, StatModifier mod)
    {
        if (mod == null) return;

        stats.BaseDamage += mod.flatDamage;
        stats.BaseDamage *= (1 + mod.percentDamage);

        stats.Defence += mod.flatDefence;
        stats.Defence = (int)(stats.Defence * (1 + mod.percentDefence));

        stats.CritChance += mod.critChance;
        stats.BlockChance += mod.blockChance;

        stats.ResistFire += mod.resistFire;
        stats.ResistIce += mod.resistIce;
        stats.ResistLightning += mod.resistLightning;

        stats.MaxHealth += mod.flatHealth;
        stats.MaxHealth = (int)(stats.MaxHealth * (1 + mod.percentHealth));

        stats.MaxMana += mod.flatMana;
        stats.MaxMana = (int)(stats.MaxMana * (1 + mod.percentMana));

        stats.MaxStamina += mod.flatStamina;
        stats.MaxStamina = (int)(stats.MaxStamina * (1 + mod.percentStamina));

        stats.DodgeChance += mod.dodgeChance;
        stats.MovementSpeed += mod.movementSpeed;
        stats.AttackSpeed += mod.attackSpeed;

        stats.SwordMastery += mod.swordMastery;
        stats.AxeMastery += mod.axeMastery;
        stats.MaceMastery += mod.maceMastery;
        stats.BowMastery += mod.bowMastery;
        stats.DaggerMastery += mod.daggerMastery;
        stats.CrossbowMastery += mod.crossbowMastery;

        stats.FireMastery += mod.fireMastery;
        stats.IceMastery += mod.iceMastery;
        stats.LightningMastery += mod.lightningMastery;
    }

    public void ApplyLevelModifiers()
    {
        Health.SetMaxValue(Health.MaxValue + healthBonus);
        Mana.SetMaxValue(Mana.MaxValue + manaBonus);
        Stamina.SetMaxValue(Stamina.MaxValue + staminaBonus);
    }

    internal void ModifyDefence(int defence)
    {
        this.defence += defence;
    }
}