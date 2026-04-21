using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private PositionScript positionScript;
    private GridMovement gridMovement;
    public DropItemScript droppedItems;
    public InputManagerScript inputManager;
    public MapVisualScript mapVisualScript;
    void Start() // To do - znemožnit pohyb pøi otevøení inventáøe
    {
        positionScript = GetComponent<PositionScript>();
        gridMovement= GetComponent<GridMovement>();
        if (positionScript == null)
        {
            Debug.LogError("PositionScript component not found!");
        }
        if (gridMovement == null)
        {
            Debug.LogError("GridMovement component not found!");
        }
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    void Update()
    {
        if (Time.timeScale == 0f) return;
        
        if (!gridMovement.isMoving)
        {
            if (inputManager != null && inputManager.isInventoryOpened) return;
            HandleRotationInput();
            HandleMovementInput();
            
        }

        else
        {
            gridMovement.MovePlayer();
            HandleRotationInput();
        }

    }

    void HandleMovementInput() // Pozor toto je na držení tlaèítka!!! Opravit pozici
    {
        if (Input.GetKey(KeyCode.W))
        {
            
            positionScript.Move(0);
            //droppedItems.UpdateUI(false);
            bool isUIActive = droppedItems.droppedItemsUI.gameObject.activeInHierarchy;
            droppedItems.droppedItemsUI.gameObject.SetActive(!isUIActive);
            droppedItems.droppedItemsUI.gameObject.SetActive(isUIActive);
            mapVisualScript.UpdateInformation(positionScript.GetPosition().x, positionScript.GetPosition().y, positionScript.orientationIndex);

        }
        else if (Input.GetKey(KeyCode.S))
        {
            positionScript.Move(2);
            //droppedItems.UpdateUI(false);
            bool isUIActive = droppedItems.droppedItemsUI.gameObject.activeInHierarchy;
            droppedItems.droppedItemsUI.gameObject.SetActive(!isUIActive);
            droppedItems.droppedItemsUI.gameObject.SetActive(isUIActive);
            mapVisualScript.UpdateInformation(positionScript.GetPosition().x, positionScript.GetPosition().y, positionScript.orientationIndex);
        }
        else if (Input.GetKey(KeyCode.A))
        {
            positionScript.Move(3);
            //droppedItems.UpdateUI(false);
            bool isUIActive = droppedItems.droppedItemsUI.gameObject.activeInHierarchy;
            droppedItems.droppedItemsUI.gameObject.SetActive(!isUIActive);
            droppedItems.droppedItemsUI.gameObject.SetActive(isUIActive);
            mapVisualScript.UpdateInformation(positionScript.GetPosition().x, positionScript.GetPosition().y, positionScript.orientationIndex);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            positionScript.Move(1);
            //droppedItems.UpdateUI(false);
            bool isUIActive = droppedItems.droppedItemsUI.gameObject.activeInHierarchy;
            droppedItems.droppedItemsUI.gameObject.SetActive(!isUIActive);
            droppedItems.droppedItemsUI.gameObject.SetActive(isUIActive);
            mapVisualScript.UpdateInformation(positionScript.GetPosition().x, positionScript.GetPosition().y, positionScript.orientationIndex);
        }

        
    }

    private bool isRotating = false;
    private float currentRotationY = 0f;

    void HandleRotationInput()
    {
        if (isRotating) return;

        if (Input.GetKeyDown(KeyCode.Q)) // Otoèení doleva
        {
            positionScript.TurnLeft();
            RotateCamera(-90f);
            mapVisualScript.UpdateInformation(positionScript.GetPosition().x, positionScript.GetPosition().y, positionScript.orientationIndex);
        }
        if (Input.GetKeyDown(KeyCode.E)) // Otoèení doprava
        {
            positionScript.TurnRight();
            RotateCamera(90f);
            mapVisualScript.UpdateInformation(positionScript.GetPosition().x, positionScript.GetPosition().y, positionScript.orientationIndex);
        }
    }

    void RotateCamera(float angle)
    {
        currentRotationY += angle;
        StartCoroutine(SmoothRotate(Camera.main.transform, Quaternion.Euler(0, currentRotationY, 0), 0.2f));
    }

    private IEnumerator SmoothRotate(Transform target, Quaternion targetRotation, float duration)
    {
        isRotating = true;
        Quaternion startRotation = target.rotation;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            target.rotation = Quaternion.Lerp(startRotation, targetRotation, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        target.rotation = targetRotation;
        isRotating = false;
    }


}
