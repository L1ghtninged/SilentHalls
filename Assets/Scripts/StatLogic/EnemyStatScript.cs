using UnityEngine;
using static WeaponItem;

public class EnemyStatScript : MonoBehaviour
{
    [Header("Data")]
    public EnemyData enemyData;

    [Header("Progression")]
    public int level = 1;

    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }

    [Header("Combat Base Stats")]
    public int defence;
    public int attackLower;
    public int attackHigher;

    [Header("Resistances (0–100)")]
    public int resistFire;
    public int resistIce;
    public int resistLightning;

    [Header("Combat Settings")]
    public float attackDelay;
    public float moveDelay;

    [Header("Rewards")]
    public int experiencePoints;

    [SerializeField] private GameObject damageNumberPrefab;
    [SerializeField] private Transform damageCanvasParent;

    public Enemy enemy;

    void Awake()
    {
        
        InitializeFromData();
    }

    void InitializeFromData()
    {
        level = enemyData.level;

        MaxHealth = enemyData.health;
        defence = enemyData.defence;
        attackLower = enemyData.attackLower;
        attackHigher = enemyData.attackHigher;

        resistFire = enemyData.resistFire;
        resistIce = enemyData.resistIce;
        resistLightning = enemyData.resistLightning;

        attackDelay = enemyData.attackDelay;
        moveDelay = enemyData.moveDelay;

        experiencePoints = enemyData.experiencePoints;

        CurrentHealth = MaxHealth;
    }

    // =============================
    // COMBAT SNAPSHOT
    // =============================

    public CombatStats GetCombatStats()
    {
        CombatStats stats = new CombatStats();

        // Base stats
        stats.Defence = defence;
        stats.BaseDamage = Random.Range(attackLower, attackHigher + 1);

        // Elemental resistances (pøevedené na 0–1)
        stats.ResistFire = resistFire;
        stats.ResistIce = resistIce;
        stats.ResistLightning = resistLightning;

        // Fixní šance pro enemy, lze dále rozšiøovat pøes EnemyData
        stats.CritChance = 0.05f;
        stats.DodgeChance = 0f;
        stats.BlockChance = 0f;

        // Max health
        stats.MaxHealth = MaxHealth;

        return stats;
    }

    // =============================
    // DAMAGE / HEAL
    // =============================

    public void TakeDamage(int damage)
    {
        SoundManager.Instance.PlayHit();

        CurrentHealth -= damage;

        ShowDamageNumber(damage);

        if (CurrentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }

    void ShowDamageNumber(int damage)
    {
        if (damageNumberPrefab == null) return;

        if (damageCanvasParent == null)
        {
            GameObject canvasGO = GameObject.FindGameObjectWithTag("InGameUI");
            if (canvasGO != null)
                damageCanvasParent = canvasGO.transform;
        }

        GameObject dmg = Instantiate(
            damageNumberPrefab,
            damageCanvasParent.position,
            Quaternion.identity,
            damageCanvasParent);

        dmg.GetComponent<DamageNumber>().Initialize(damage);
    }

    // =============================
    // LEVEL SCALING
    // =============================

    public void ChangeLevel(int newLevel)
    {
        level = newLevel;

        float scale = Mathf.Pow(1.5f, level - 1);

        MaxHealth = Mathf.RoundToInt(enemyData.health * scale);
        defence = Mathf.RoundToInt(enemyData.defence * scale);
        attackLower = Mathf.RoundToInt(enemyData.attackLower * scale);
        attackHigher = Mathf.RoundToInt(enemyData.attackHigher * scale);

        resistFire = Mathf.RoundToInt(enemyData.resistFire * scale);
        resistIce = Mathf.RoundToInt(enemyData.resistIce * scale);
        resistLightning = Mathf.RoundToInt(enemyData.resistLightning * scale);

        CurrentHealth = MaxHealth;
    }

    // =============================
    // DEATH
    // =============================

    void Die()
    {
        if (enemy != null)
            enemy.RemoveEnemy();

        var player = GameObject.FindGameObjectWithTag("Player")
                               .GetComponent<StatScript>();

        player.GainExperience(experiencePoints);

        SoundManager.Instance.PlayDeath();

        Destroy(gameObject);
    }
}