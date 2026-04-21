using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class SkillScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Data")]
    public SkillNodeData skill;

    [Header("UI References")]
    public Image iconImage;
    public Image backgroundImage;
    public TMP_Text levelText;
    public SkillTreeScript skillTreeScript;

    [Header("Visual Settings")]
    public Color normalColor = Color.white;
    public Color hoverColor = new Color(1f, 1f, 0.8f);
    public Color lockedColor = new Color(0.5f, 0.5f, 0.5f);

    private PlayerSkillTree playerSkillTree;

    void Start()
    {
        playerSkillTree = FindObjectOfType<PlayerSkillTree>();

        SetupFromData();
        RefreshState();
    }

    void SetupFromData()
    {
        if (skill == null) return;

        iconImage.sprite = skill.icon;
        levelText.text = "";
    }

    public void RefreshState()
    {
        if (skill == null || playerSkillTree == null) return;

        int level = playerSkillTree.GetSkillLevel(skill);
        bool unlocked = level > 0;
        bool canUnlock = playerSkillTree.CanUnlock(skill);

        // Level text
        if (skill.maxLevel > 1)
            levelText.text = level + "/" + skill.maxLevel;
        else
            levelText.text = unlocked ? level.ToString() : "";

        // Barva backgroundu
        if (unlocked)
            backgroundImage.color = normalColor;
        else if (!canUnlock)
            backgroundImage.color = lockedColor;
        else
            backgroundImage.color = normalColor;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        backgroundImage.color = hoverColor;

        skillTreeScript.ShowSkillInfo(skill);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        RefreshState();

        skillTreeScript.HideSkillInfo();
    }

    public void OnClick()
    {
        if (playerSkillTree.CanUnlock(skill))
        {
            playerSkillTree.Add(skill);
            RefreshState();
        }
    }
}