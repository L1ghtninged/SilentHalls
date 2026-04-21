using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkillBranch
{
    Fighter,
    Mage,
    Rogue
}

public class PlayerSkillTree : MonoBehaviour
{
    public Dictionary<SkillNodeData, int> skillLevels = new();

    public int GetSkillLevel(SkillNodeData skill)
    {
        return skillLevels.ContainsKey(skill) ? skillLevels[skill] : 0;
    }

    public bool IsUnlocked(SkillNodeData skill)
    {
        return GetSkillLevel(skill) > 0;
    }
    public int GetPointsInBranch(SkillBranch branch)
    {
        int count = 0;
        foreach(var skill in skillLevels)
        {
            if (skill.Key.branch == branch) count += skill.Value;
        }
        return count;
    }
    public bool CanUnlock(SkillNodeData skill)
    {
        // body ve vìtvi
        if (GetPointsInBranch(skill.branch) < skill.requiredPointsInBranch)
            return false;

        // prerequisite
        foreach (var prereq in skill.prerequisites)
        {
            if (!IsUnlocked(prereq))
                return false;
        }

        return true;
    }
    public void Add(SkillNodeData skill)
    {
        skillLevels[skill] += 1;
    }
}
