using UnityEngine;
using static WeaponItem;

public class AttackScript : MonoBehaviour
{
    [Header("Combat")]
    public float attackRange = 2f;
    public Transform castPoint;

    private PositionScript positionScript;
    private StatScript statScript;

    public GameObject slashPrefab;
    

    void Awake()
    {
        positionScript = GetComponent<PositionScript>();
        statScript = GetComponent<StatScript>();
    }

    // =========================
    // PLAYER ATTACK
    // =========================

    public void PerformAttack(WeaponItem weapon)
    {
        if (!HasEnoughResource(weapon))
            return;

        if (weapon.damageType == DamageType.Magical && weapon.spellPrefab != null)
        {
            CastSpell(weapon);
        }
        else
        {
            PerformMeleeAttack(weapon);
        }
    }

    void CastSpell(WeaponItem weapon)
    {
        Vector3 direction = GetAttackDirection();

        GameObject spellGO = Instantiate(
            weapon.spellPrefab,
            transform.position,
            Quaternion.LookRotation(direction)
        );

        ProjectileSpell spell = spellGO.GetComponent<ProjectileSpell>();

        if (spell != null)
        {
            spell.Initialize(statScript, weapon);
        }

        ConsumeResource(weapon);
    }

    void PerformMeleeAttack(WeaponItem weapon)
    {
        Vector3 direction = GetAttackDirection();

        if (Physics.Raycast(transform.position, direction, out RaycastHit hit, attackRange))
        {
            EnemyStatScript enemy = hit.collider.GetComponent<EnemyStatScript>();
            if (enemy != null)
            {
                ResolvePlayerAttack(statScript, enemy, weapon);
                SpawnSlashEffect(hit.transform.position);
            }
        }

        ConsumeResource(weapon);
    }

    // =========================
    // RESOURCE CHECK
    // =========================

    public bool HasEnoughResource(WeaponItem weapon)
    {
        if (weapon.damageType == DamageType.Magical)
            return statScript.Mana.CurrentValue >= weapon.manaCost;

        return statScript.Stamina.CurrentValue >= weapon.manaCost;
    }

    public void ConsumeResource(WeaponItem weapon)
    {
        if (weapon.damageType == DamageType.Magical)
            statScript.UseMana(weapon.manaCost);
        else
            statScript.UseStamina(weapon.manaCost);
    }

    // =========================
    // DAMAGE CALCULATION
    // =========================
    void SpawnSlashEffect(Vector3 position)
    {
        Vector3 spawnPos = position + Vector3.up * 1f;

        GameObject effect = Instantiate(
            slashPrefab,
            spawnPos,
            Quaternion.identity
        );

        // náhodná rotace → vypadá líp
        effect.transform.rotation = Quaternion.Euler(0, Random.Range(0, 360), 0);
    }
    void ResolvePlayerAttack(StatScript attacker, EnemyStatScript defender, WeaponItem weapon)
    {
        CombatStats atk = attacker.GetCombatStats();
        CombatStats def = defender.GetCombatStats();

        // Dodge check
        if (Random.value < def.DodgeChance)
            return;

        // Base weapon damage
        float damage = Random.Range(weapon.lowValue, weapon.highValue + 1);

        // Přidáme BaseDamage z CombatStats hráče
        damage += atk.BaseDamage;

        // Přidáme bonus z mastery zbraně
        switch (weapon.weaponType)
        {
            case WeaponType.Sword: damage += atk.SwordMastery; break;
            case WeaponType.Axe: damage += atk.AxeMastery; break;
            case WeaponType.Mace: damage += atk.MaceMastery; break;
            case WeaponType.Bow: damage += atk.BowMastery; break;
            case WeaponType.Dagger: damage += atk.DaggerMastery; break;
            case WeaponType.Crossbow: damage += atk.CrossbowMastery; break;
        }

        // Apply defence reduction
        float reduction = def.Defence / (def.Defence + 100f);
        damage = damage * (1f - reduction);

        // Apply elemental resist
        float resist = GetElementResistance(def, weapon.magicType);
        damage = damage * (1f - resist);

        // Critical hit
        if (Random.value < atk.CritChance)
            damage = Mathf.Round(damage * 2f); // prozatím krit jen x2, můžeš přidat multiplikátor

        defender.TakeDamage(Mathf.RoundToInt(damage));
    }

    float GetElementResistance(CombatStats def, MagicType type)
    {
        switch (type)
        {
            case MagicType.Fire: return def.ResistFire * 0.01f;
            case MagicType.Ice: return def.ResistIce * 0.01f;
            case MagicType.Lightning: return def.ResistLightning * 0.01f;
            default: return 0f;
        }
    }

    Vector3 GetAttackDirection()
    {
        switch (positionScript.orientationIndex)
        {
            case 0: return Vector3.forward;
            case 1: return Vector3.right;
            case 2: return Vector3.back;
            case 3: return Vector3.left;
            default: return Vector3.forward;
        }
    }

    // =========================
    // ENEMY ATTACK
    // =========================

    public void EnemyAttack(EnemyStatScript enemy, StatScript player)
    {
        CombatStats atk = enemy.GetCombatStats();
        CombatStats def = player.GetCombatStats();

        if (Random.value < def.DodgeChance)
            return;

        float damage = Random.Range(enemy.attackLower, enemy.attackHigher + 1);

        // Add BaseDamage z enemy CombatStats (pokud by se používalo)
        damage += atk.BaseDamage;

        // Apply defence reduction
        float reduction = def.Defence / (def.Defence + 100f);
        damage = damage * (1f - reduction);

        // TODO: Přidat elemental resist a crit pro enemy, pokud bude implementováno
        player.TakeDamage(Mathf.RoundToInt(damage));
    }
}