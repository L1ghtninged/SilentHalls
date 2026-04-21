using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManagerScript : MonoBehaviour
{
    public GameObject inventoryGroup;
    private bool isGamePaused = false;
    [HideInInspector] public bool isInventoryOpened = false;
    private bool isGameOver = false;
    private bool isSkillTreeOpen = false;
    public GameObject InGameUI;
    public GameObject pauseMenu;
    public GameObject gameOverMenu;
    public GameObject droppedInventory;
    public GameObject darkScreen;
    public GameObject skillTree;
    public TextMeshProUGUI stairsText;
    public MapVisualScript mapVisualScript;
    public InventoryStatsUI inventoryStatsUI;
    // Start is called before the first frame update
    private string GetText(int number)
    {
        if(number == 0)
        {
            return "You escaped!";
        }
        string t;
        switch (number)
        {
            case 1:
                t = "st";
                break;
            case 2:
                t = "nd";
                break;
            case 3:
                t = "rd";
                break;
            default:
                t = "th";
                break;
        }
        string txt = "You entered the " + number + t + " floor";



        return txt;
    }
    public void ShowScreen(int floorNumber)
    {
        StartCoroutine(ShowDarkScreenCoroutine(floorNumber));
    }

    private IEnumerator ShowDarkScreenCoroutine(int floorNumber)
    {
        darkScreen.SetActive(true);
        stairsText.text = GetText(floorNumber);
        yield return new WaitForSeconds(2f);
        darkScreen.SetActive(false);
    }

    // Update is called once per frame
    void Update() 
    {
        if (Input.GetKeyUp(KeyCode.Escape) && !isGameOver)
        {
            PauseGame();

        }
        if(isGamePaused) return;
        else if(Input.GetKeyDown(KeyCode.I) && !isGameOver)
        {
            OpenInventory();
            mapVisualScript.CloseMap();
            CloseSkillTree();
            
        }
        
        if (Input.GetKeyUp(KeyCode.T))
        {
            mapVisualScript.CloseMap();
            CloseInventory();
            OpenSkillTree();
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapVisualScript.ToggleMap();
            CloseInventory();
            CloseSkillTree();
        }
        if (!mapVisualScript.isMapOpen) return;
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            mapVisualScript.FlipLeft();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            mapVisualScript.FlipRight();
        }


    }
    public void PauseGame()
    {
        
        isGamePaused = !isGamePaused;

        if (isGamePaused)
        {
            SoundManager.Instance.PauseSounds();
            ShowPauseMenu();
        }
        else
        {
            SoundManager.Instance.UnPauseSounds();
            Time.timeScale = 1.0f;
            InGameUI.SetActive(true);
            pauseMenu.SetActive(false);
        }
        TooltipUI.instance.HideTooltip();
    }

    public void ShowPauseMenu()
    {
        Time.timeScale = 0f;
        InGameUI.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void GameOver()
    {
        isGameOver = true;
        ShowGameOverMenu();
    }
    public void ShowGameOverMenu()
    {
        Time.timeScale = 0f;
        InGameUI.SetActive(false);
        gameOverMenu.SetActive(true);
        TooltipUI.instance.HideTooltip();
    }
    public void OpenInventory()
    {
        if (isGamePaused)
        {
            return;
        }
        inventoryStatsUI.Refresh();
        inventoryGroup.SetActive(!inventoryGroup.activeInHierarchy);
        isInventoryOpened = !isInventoryOpened;
        TooltipUI.instance.HideTooltip();

    }
    public void CloseInventory()
    {
        if (isGamePaused) return;
        inventoryGroup.SetActive(false);
        isInventoryOpened = false;
        TooltipUI.instance.HideTooltip();
    }
    public void OpenDroppedInventory()
    {
        if(isGamePaused) return; 
        droppedInventory.SetActive(!droppedInventory.activeInHierarchy);

        DropItemScript dropSlotScript = GameObject.FindGameObjectWithTag("DropSlot").GetComponent<DropItemScript>();
        dropSlotScript.UpdateUI();
    }
    public void OpenSkillTree()
    {
        if (isGamePaused) { return; }
        CloseInventory();
        skillTree.SetActive(!skillTree.activeInHierarchy);
        isSkillTreeOpen = !skillTree.activeInHierarchy;

    }
    public void CloseSkillTree()
    {
        skillTree.SetActive(false);
        isSkillTreeOpen = false;
    }


    public void MainMenu()
    {
        PauseGame();
        SceneManager.LoadScene(0);
    }
    public void MainMenuFromGameOver()
    {
        SoundManager.Instance.UnPauseSounds();
        //InGameUI.SetActive(true);
        //pauseMenu.SetActive(false);
        Time.timeScale = 1.0f;
        SceneManager.LoadScene(0);
    }
    public void PlaySounds()
    {
        SoundManager.Instance.EnableSounds();
    }
    public void DisableSounds()
    {
        SoundManager.Instance.DisableSounds();
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void ToggleMap()
    {
        if (isGamePaused) return;
        mapVisualScript.ToggleMap();
    }


}
