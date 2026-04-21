using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptManager : MonoBehaviour
{
    public InputManagerScript gameUIManager;
    public GameObject playerObject;
    [HideInInspector]public StatScript playerStats;
    [HideInInspector]public AttackScript playerAttack;
    [HideInInspector]public GridMovement playerGridMovement;
    [HideInInspector]public PositionScript playerPosition;
    [HideInInspector]public PlayerController playerController;
    [HideInInspector]public Inventory playerInventory;
    public bool showDescriptions = true;

    private void Start()
    {
        playerStats = playerObject.GetComponent<StatScript>();
        playerAttack = playerObject.GetComponent<AttackScript>();
        playerGridMovement = playerObject.GetComponent<GridMovement>();
        playerPosition = playerObject.GetComponent<PositionScript>();
        playerController = playerObject.GetComponent<PlayerController>();
        playerInventory = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<Inventory>();
    }
    public void ToggleDescriptions()
    {
        showDescriptions = !showDescriptions;
    }




}
