using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image image;
    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public bool enableDragging = true;
    [HideInInspector] public bool isBeingDragged = false;
    //public TooltipUI tooltip;
    public bool isEquipped = false;
    public bool isDropped = false;
    public ItemType type;
    public Item item;
    public ScriptManager manager;

    [Header("Cooldown Settings")]
    public Image cooldownOverlayPrefab;

    private Image cooldownOverlay;
    private float cooldownTimer;
    private bool isOnCooldown;
    float cooldownDuration;
    public int inventoryIndex = -1;
    
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("Logic").GetComponent<ScriptManager>();
        if(type == ItemType.Weapon)
        {
            InitialiseCooldownOverlay();

        }
        image.sprite = item.sprite;
        
    }
    public void InitialiseItem(Item newItem)
    {
        item = newItem;
        type = item.itemType;
        image.sprite = item.sprite;
    }
    void Update()
    {
        if (isOnCooldown && cooldownOverlay != null)
        {
            cooldownTimer -= Time.deltaTime;
            cooldownOverlay.fillAmount = cooldownTimer / cooldownDuration;

            if (cooldownTimer <= 0)
            {
                isOnCooldown = false;
                cooldownOverlay.fillAmount = 0;
                enableDragging = true;
            }
        }
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && !isBeingDragged && enableDragging && !isDropped)
        {
            isBeingDragged = true;
            image.raycastTarget = false;
            parentAfterDrag = transform.parent;
            transform.SetParent(transform.root);
            transform.SetAsLastSibling();
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!enableDragging) return;
        if (eventData.button == PointerEventData.InputButton.Left) transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isBeingDragged)
        {
            isBeingDragged = false;
            image.raycastTarget = true;

            bool wasDroppedOnLock = false;

            // Raycast do svìta z pozice myši
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 5f))
            {
                DoorLock doorLock = hit.collider.GetComponent<DoorLock>();

                if (doorLock != null && type == ItemType.Key && !doorLock._lock.isOpened)
                {
                    KeyItem key = (KeyItem)item;
                    if(key.keyType != doorLock.keyType)
                    {
                        wasDroppedOnLock = false;
                    }
                    else
                    {
                        Debug.Log("Klíè puštìn na zámek: " + hit.collider.name);
                        doorLock.OnKeyUsed(); // nebo doorLock.Unlock()
                        wasDroppedOnLock = true;

                        // Klíè mùže být znièen nebo pøesunut jinam:
                        Destroy(gameObject);
                    }
                    
                }
            }

            // Pokud nebyl klíè puštìn na zámek, vra ho zpìt
            if (!wasDroppedOnLock)
            {
                transform.SetParent(parentAfterDrag);
                transform.localPosition = Vector3.zero;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        TooltipUI.instance.HideTooltip();
        if (eventData.button == PointerEventData.InputButton.Left && isDropped)
        {
            if(inventoryIndex >= 0)
            {

            }
            else
            {
                PickupFromFloor();

                return; 
            }
        }

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (type == ItemType.Weapon && !manager.gameUIManager.isInventoryOpened)
            {
                if (!isOnCooldown)
                {
                    WeaponItem weaponItem = (WeaponItem)item;
                    if (manager.playerAttack.HasEnoughResource(weaponItem))
                    {
                        StartCooldown(weaponItem.attackSpeed);
                        enableDragging = false;
                        item.Use(manager);
                    }
                }
                
                
            }
            else
            {
                item.Use(manager);
            }
            
            if(type == ItemType.Potion)
            {
                Destroy(gameObject);
            }
        }
    }
    private void PickupFromFloor()
    {
        DropItemScript dropScript = GameObject.FindGameObjectWithTag("DropSlot").GetComponent<DropItemScript>();
        if (manager == null) manager = GameObject.FindGameObjectWithTag("Logic").GetComponent<ScriptManager>();

        if (manager.playerInventory.AddItem(item))
        {
            // Uložíme si potøebná data pøed destrukcí
            Position playerPosition = dropScript.GetPlayerPosition();
            GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>().RemoveItem(this.item, playerPosition);
            Destroy(gameObject);
            dropScript.visuals.UpdateBagsVisibility();
            // Spustíme korutinu pro správné naèasování
            StartCoroutine(PickupCoroutine(dropScript, playerPosition));
            
        }
    }

    private IEnumerator PickupCoroutine(DropItemScript dropScript, Position position)
    {
        // 1. Odstraníme položky
        foreach (var i in dropScript.DroppedItemsFiltered(position))
        {
            Debug.Log(i + "+ABCD");
        }
        Debug.Log("(0)");
        dropScript.droppedItems.RemoveAll(i => i.position == position);
        foreach(var i in dropScript.DroppedItemsFiltered(position))
        {
            Debug.Log(i + "+ABCD");
        }
        Debug.Log("(1)");
        // 2. Poèkáme na frame
        foreach (var i in dropScript.DroppedItemsFiltered(position))
        {
            Debug.Log(i + "+ABCD");
        }
        Debug.Log("(2)");
        // 3. Získáme aktuální pøedmìty
        List<DroppedItemInstance> currentItems = dropScript.inventory.GetAllItemsAsDroppedInstances(position);
        currentItems.RemoveAll(i => i.item.Equals(this));
        Debug.Log("Num of items"+currentItems.Count);
        foreach (var i in dropScript.DroppedItemsFiltered(position))
        {
            Debug.Log(i + "+ABCD");
        }
        Debug.Log("(3)");

        dropScript.droppedItems.AddRange(currentItems);
        foreach (var i in dropScript.DroppedItemsFiltered(position))
        {
            Debug.Log(i + "+ABCD");
        }
        Debug.Log("(4)");
        // 6. Aktualizace UI
        dropScript.UpdateUI(false);
        dropScript.visuals.OnItemsChangedAtPosition(position);

        Debug.Log($"Pickup completed. Items on floor: {dropScript.DroppedItemsFiltered(position).Count}");
        yield return new WaitForEndOfFrame();
    }


    void InitialiseCooldownOverlay()
    {
        Debug.Log("This came through (4)");
        if (cooldownOverlayPrefab != null)
        {
            cooldownOverlay = Instantiate(cooldownOverlayPrefab, transform);
            cooldownOverlay.transform.localPosition = Vector3.zero;
            cooldownOverlay.transform.localScale = Vector3.one;
            cooldownOverlay.transform.SetAsFirstSibling();

            cooldownOverlay.fillAmount = 0;
            cooldownOverlay.raycastTarget = false;


        }
        else
        {
            Debug.LogError("Prefab is null");
        }
    }
    void StartCooldown(float attackSpeed)
    {
        if (cooldownOverlay == null) return;
        cooldownDuration = attackSpeed;
        isOnCooldown = true;
        cooldownTimer = attackSpeed;
        cooldownOverlay.fillAmount = 1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(manager.gameUIManager.isInventoryOpened && manager.showDescriptions) TooltipUI.instance.SetAndShowTooltip(this.item.GetTooltip());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        TooltipUI.instance.HideTooltip();
    }
}

