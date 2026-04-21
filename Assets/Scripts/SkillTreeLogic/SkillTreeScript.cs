using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeScript : MonoBehaviour
{
    public string classString = "";
    public string[] classNames = { "Fighter class", "Mage class", "Rogue class" };
    public GameObject descriptionObject;
    public PlayerSkillTree playerSkillTree;
    public StatScript statScript;

    public TextMeshProUGUI classText;
    public TextMeshProUGUI skillName;
    public TextMeshProUGUI warningText;
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI assignedPointsText;
    public TextMeshProUGUI unAssignedPointsText;
    private void Start()
    {
        classString = classNames[0];
        HideSkillInfo();
        UpdateUnAssignedPoints();
    }
    public void UpdateUnAssignedPoints()
    {
        unAssignedPointsText.text = "Unassigned skill points: " + statScript.skillPoints.ToString();
    }
    public void AssignSkill(SkillNodeData skillNode)
    {
        if (statScript.skillPoints <= 0)
        {
            Debug.Log("Not enough skill points!");
            return;
        }

        if (!playerSkillTree.skillLevels.ContainsKey(skillNode))
        {
            playerSkillTree.skillLevels.Add(skillNode, 1);
        }
        else
        {
            playerSkillTree.skillLevels[skillNode]++;
        }

        statScript.skillPoints--;
        UpdateUnAssignedPoints();
        statScript.GetCombatStats();
        Debug.Log($"{skillNode.name} unlocked!");
    }

    public void UpdateTexts(int tabID)
    {
        classString = classNames[tabID];
        HideSkillInfo();
        unAssignedPointsText.text = "Unassigned skill points: " + statScript.skillPoints;
    }
    public void ShowSkillInfo(SkillNodeData skill)
    {
        classText.text = classString;
        skillName.text = skill.name;
        if (!playerSkillTree.CanUnlock(skill) || true)
        {
            int points = skill.requiredPointsInBranch;
            int requiredLevel = skill.requiredPlayerLevel;
            string requiredSkills = "";
            foreach(var requiredSkill in skill.prerequisites)
            {
                requiredSkills += requiredSkill.name + ", ";
            }
            if (requiredSkills.Length > 0)
            {
                requiredSkills = requiredSkills.Substring(requiredSkills.Length - 2);
            }
            
            warningText.text = " Required skills are: " + requiredSkills + "\n Required player level: " + requiredLevel + "\n Required points in this branch: " + points;
        }
        assignedPointsText.text = "Assigned points: " + playerSkillTree.GetSkillLevel(skill);
        descriptionText.text = skill.description;
        descriptionObject.SetActive(true);

    }
    public void HideSkillInfo()
    {
        classText.text = "";
        skillName.text = "";
        warningText.text = "";
        descriptionText.text = "";
        assignedPointsText.text = "";
        descriptionObject.SetActive(false);
    }

}

