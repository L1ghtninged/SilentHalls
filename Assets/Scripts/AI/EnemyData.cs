using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static WeaponItem;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "Enemies/Enemy Data")]
public class EnemyData : ScriptableObject
{
    public string enemyName;
    public int health;
    public int attackLower;
    public int attackHigher;
    public float attackDelay;
    public float moveDelay;
    public int defence;
    public int resistFire;
    public int resistIce;
    public int resistLightning;
    public int experiencePoints;
    public int level;

    public DamageType damageType;
    public MagicType magicType;



}
