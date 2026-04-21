using Assembly_CSharp;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PositionScript : MonoBehaviour
{
    public int x = 0;
    public int y = 0;
    public int z = 0;
    public readonly string[] orientations = { "north", "east", "south", "west" };
    public int orientationIndex = 0;

    private float gridSize = 5;
    [HideInInspector]public GridMovement gridMovement;
    public DropItemScript dropItemScript;

    void Start()
    {
        gridMovement = GetComponent<GridMovement>();
        if (gridMovement == null)
        {
            Debug.LogError("GridMovement component not found!");
        }
        else
        {
            gridMovement.UpdatePos += UpdatePosition;
        }
    }
    public void SetPosition(int x, int y)
    {
        EntityPositions.UnregisterEntity(this.x, this.y);
        this.x = x;
        this.y = y;
        Vector3 newPosition = new Vector3(x * gridSize, 1.5f, y * gridSize);
        EntityPositions.RegisterEntity(this, x, y);
        transform.position = newPosition;
        UpdatePosition(newPosition);
    }

    void OnDestroy()
    {
        if (gridMovement != null)
        {
            gridMovement.UpdatePos -= UpdatePosition;
        }
    }
    private void UpdatePosition(Vector3 newPosition)
    {
        EntityPositions.UnregisterEntity(x, y);
        x = Mathf.RoundToInt(newPosition.x / gridSize);
        y = Mathf.RoundToInt(newPosition.z / gridSize);
        z = Mathf.RoundToInt(newPosition.y / gridSize);
        EntityPositions.RegisterEntity(this, x, y);
        if(dropItemScript!= null)
        {
            dropItemScript.UpdateUI(false);
            if (EntityPositions.IsStairsDown(x, y))
            {
                Pos pos = new Pos();
                pos.x = x;
                pos.y = y;
                GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>().GetObjectByPos(pos).GetComponent<StairsDown>().OnPlayerEnter();
            }
            else if(EntityPositions.IsStairsUp(x, y))
            {
                Pos pos = new Pos();
                pos.x = x;
                pos.y = y;
                GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>().GetObjectByPos(pos).GetComponent<StairsUp>().OnPlayerEnter();
            }
        }
        
         
    }

    public void TurnLeft()
    {
        if (orientationIndex == 0)
        {
            orientationIndex = orientations.Length - 1;
        }
        else
        {
            orientationIndex--;
        }
    }

    public void TurnRight()
    {
        if (orientationIndex == orientations.Length - 1)
        {
            orientationIndex = 0;
        }
        else
        {
            orientationIndex++;
        }
    }
    public Vector2Int GetGridPosition()
    {
        return new Vector2Int(x, y);
    }
    public int GetMoveOrientation(int direction) // Tohle pohne, pokud zadáme napø: Chci jít rovnì. 0
    {
        int finalOrientation = orientationIndex + direction;
        if (finalOrientation > orientations.Length - 1) finalOrientation -= orientations.Length;

        return finalOrientation;
    }
    public Position GetPosition()
    {
        return new Position(x, y, z);
    }

    public void MovePlayer(int orientation)
    {
        Vector3 moveDirection = Vector3.zero;

        switch (orientation)
        {
            case 0: // north
                moveDirection = Vector3.forward;
                break;
            case 1: // east
                moveDirection = Vector3.right;
                break;
            case 2: // south
                moveDirection = Vector3.back;
                break;
            case 3: // west
                moveDirection = Vector3.left;
                break;
            default:
                Debug.LogError("Orientation is not 0-3, it is: " + orientation);
                break;
        }


        if (gridMovement != null)
        {
            gridMovement.StartMovement(moveDirection);
        }
    }

    public void Move(int direction)
    {
        int targetOrientation = GetMoveOrientation(direction);
        Vector2Int newPos = GetNewPosition(targetOrientation);

        if (EntityPositions.IsPositionValid(newPos.x, newPos.y) &&
           EntityPositions.IsWalkable(newPos.x, newPos.y))
        {
            MovePlayer(targetOrientation);
        }
    }
    public Vector2Int GetNewPosition(int direction)
    {
        return GetNewPositionFromDirection(direction, this.x, this.y);
    }

    public static Vector2Int GetNewPositionFromDirection(int direction, int currentX, int currentY)
    {
        int x = currentX;
        int y = currentY;

        switch (direction)
        {
            case 0: // North
                y++;
                break;
            case 1: // East
                x++;
                break;
            case 2: // South
                y--;
                break;
            case 3: // West
                x--;
                break;
            default:
                Debug.LogError("Invalid direction: " + direction);
                break;
        }

        return new Vector2Int(x, y);
    }
}
public struct Position
{
    public int x, y, z;

    public Position(int x, int y, int z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public override bool Equals(object obj)
    {
        if (!(obj is Position)) return false;
        Position other = (Position)obj;
        return this == other;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            return (x * 73856093) ^ (y * 19349663) ^ (z * 83492791);
        }
    }

    public static bool operator ==(Position a, Position b)
    {
        return a.x == b.x && a.y == b.y && a.z == b.z;
    }

    public static bool operator !=(Position a, Position b)
    {
        return !(a == b);
    }
}

