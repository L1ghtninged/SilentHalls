using UnityEngine;

public class ProjectileSpell : MonoBehaviour
{
    public float speed = 10f;

    private StatScript attacker;
    private WeaponItem weapon;

    public void Initialize(StatScript attackerStats, WeaponItem weaponItem)
    {
        attacker = attackerStats;
        weapon = weaponItem;
    }

    void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        EnemyStatScript enemy = other.GetComponent<EnemyStatScript>();
        if (enemy != null)
        {
            ResolveSpellHit(enemy);
            Destroy(gameObject);
        }
    }

    void ResolveSpellHit(EnemyStatScript defender)
    {
        CombatStats atk = attacker.GetCombatStats();
        CombatStats def = defender.GetCombatStats();

        // Základní damage ze zbranì
        float damage = Random.Range(weapon.lowValue, weapon.highValue + 1);

        // Pøièteme BaseDamage hráèe
        damage += atk.BaseDamage;

        // Pøièteme mastery bonus podle typu zbranì
        switch (weapon.weaponType)
        {
            case WeaponType.Sword: damage += atk.SwordMastery; break;
            case WeaponType.Axe: damage += atk.AxeMastery; break;
            case WeaponType.Mace: damage += atk.MaceMastery; break;
            case WeaponType.Bow: damage += atk.BowMastery; break;
            case WeaponType.Dagger: damage += atk.DaggerMastery; break;
            case WeaponType.Crossbow: damage += atk.CrossbowMastery; break;
        }

        // Aplikace redukce obrany
        float reduction = def.Defence / (def.Defence + 100f);
        damage *= (1f - reduction);

        // Aplikace elementární rezistence
        float resist = GetElementResistance(def, weapon.magicType);
        damage *= (1f - resist);

        // Kritický zásah
        if (Random.value < atk.CritChance)
            damage = Mathf.Round(damage * 2f); // x2, mùžeš pozdìji použít multiplikátor

        defender.TakeDamage(Mathf.RoundToInt(damage));
    }

    float GetElementResistance(CombatStats def, WeaponItem.MagicType type)
    {
        switch (type)
        {
            case WeaponItem.MagicType.Fire: return def.ResistFire * 0.01f;
            case WeaponItem.MagicType.Ice: return def.ResistIce * 0.01f;
            case WeaponItem.MagicType.Lightning: return def.ResistLightning * 0.01f;
            default: return 0f;
        }
    }
}