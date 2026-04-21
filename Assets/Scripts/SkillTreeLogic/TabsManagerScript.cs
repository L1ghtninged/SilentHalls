using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabsManagerScript : MonoBehaviour
{
    public GameObject[] Tabs;
    public Image[] TabButtons;
    public Sprite[] InactiveTabButtons, ActiveTabButtons;
    public Vector2 InactiveTabButtonSize, ActiveTabButtonSize;
    public SkillTreeScript skillScript;

    public void SwitchToTab(int tabID)
    {
        foreach(GameObject go in Tabs)
        {
            go.SetActive(false);
        }
        Tabs[tabID].SetActive(true);
        for(int i = 0; i < TabButtons.Length; i++)
        {
            var im = TabButtons[i];
            im.sprite = InactiveTabButtons[i];
            im.rectTransform.sizeDelta = InactiveTabButtonSize;
        }
        TabButtons[tabID].sprite = ActiveTabButtons[tabID];
        TabButtons[tabID].rectTransform.sizeDelta = ActiveTabButtonSize;
        skillScript.UpdateTexts(tabID);
    }
}
