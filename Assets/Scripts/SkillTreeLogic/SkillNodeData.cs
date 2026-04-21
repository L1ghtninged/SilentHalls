using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Skill Tree/Skill Node")]
public class SkillNodeData : ScriptableObject
{
    [Header("Basic Info")]
    public string skillID; // unikátní ID
    public string skillName;
    [TextArea] public string description;
    public Sprite icon;

    [Header("Tree Placement")]
    public SkillBranch branch;
    public int tier; // 1,2,3...

    [Header("Leveling")]
    public int maxLevel = 1;

    [Header("Requirements")]
    public List<SkillNodeData> prerequisites;
    public int requiredPointsInBranch;
    public int requiredPlayerLevel;

    [Header("Effects")]
    public StatModifier effects;
}