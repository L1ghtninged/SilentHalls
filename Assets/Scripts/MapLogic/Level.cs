using static KeyItem;
using System.Collections.Generic;
using UnityEngine;

public class Level
{
    public int[,] levelMap;
    public int levelNumber = 1;
    public List<Lock> locks = new List<Lock>();
    public List<ButtonObject> ButtonObjects = new List<ButtonObject>();
    public List<Enemy> enemies = new List<Enemy>();
    public Dictionary<Position, List<Item>> items = new Dictionary<Position, List<Item>>();
    public int startX = 1;
    public int startY = 1;
    public int endX = 1;
    public int endY = 1;
    public Level(int[,] levelMap)
    {
        this.levelMap = levelMap;
    }

    public void AddLock(int x, int y, int doorX, int doorY, int direction, KeyType keyType, bool isOpened = false)
    {
        locks.Add(new Lock(x, y, direction, keyType, doorX, doorY, isOpened));
    }
    public void AddButtonObject(int x, int y, int doorX, int doorY, int direction, bool isOpened = false)
    {
        ButtonObjects.Add(new ButtonObject(x, y, direction, doorX, doorY, isOpened));
    }
    public void AddEnemy(int x, int y, Enemy.EnemyType type)
    {
        enemies.Add(new Enemy(x, y, type));
    }

    public void AddItem(int x, int y, Item item)
    {
        Position position = new Position(x, y, 0);
        if (!items.ContainsKey(position)) items.Add(position, new List<Item>());
        items[position].Add(item);
    }

    public Tile[,] GenerateMap()
    {
        int[,] mapBlueprint = levelMap;
        int width = mapBlueprint.GetLength(0);
        int height = mapBlueprint.GetLength(1);

        var mapTiles = new Tile[width, height];

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // PŘEVRÁCENÍ Y PŘI GENEROVÁNÍ LOGICKÉ MAPY
                TileType type = (TileType)mapBlueprint[x, y];
                if (type == TileType.StartPlayerPosition)
                {
                    startX = x;
                    startY = y;
                }
                if (type == TileType.EndPlayerPosition)
                {
                    endX = x;
                    endY = y;
                }
                mapTiles[x, y] = new Tile
                {
                    tileType = type,
                    walkable = type == TileType.Floor
                    || type == TileType.StairsDOWN_NS
                    || type == TileType.Indicator
                    || type == TileType.StairsDOWN_WE
                    || type == TileType.StairsUP_NS
                    || type == TileType.StairsUP_WE
                    || type == TileType.StartPlayerPosition
                    || type == TileType.EndPlayerPosition

                };
            }
        }
        return mapTiles;
    }

}
public enum TileType
{
    Indicator = 0,
    Wall = 1,      // Zeď (neprůchozí)
    Floor = 2,     // Podlaha (průchozí)
    Door_NS = 3,
    Door_WE = 4,
    StairsDOWN_NS = 5,
    StairsDOWN_WE = 6,
    StairsUP_NS = 7,
    StairsUP_WE = 8,
    StartPlayerPosition = 9,
    EndPlayerPosition = 10
}
public struct Tile
{
    public TileType tileType;
    public bool walkable;

}
public struct Pos
{
    public int x;
    public int y;
}
public class Lock
{
    public int x;
    public int y;
    public int direction;
    public KeyType keyType;

    public int doorX;
    public int doorY;
    public bool isOpened;

    public Lock(int x, int y, int direction, KeyType keyType, int doorX, int doorY, bool isOpened = false)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.keyType = keyType;
        this.doorX = doorX;
        this.doorY = doorY;
        this.isOpened = isOpened;
    }
}
public class ButtonObject
{
    public int x;
    public int y;
    public int direction;


    public int doorX;
    public int doorY;
    public bool isOpened;

    public ButtonObject(int x, int y, int direction, int doorX, int doorY, bool isOpened = false)
    {
        this.x = x;
        this.y = y;
        this.direction = direction;
        this.doorX = doorX;
        this.doorY = doorY;
        this.isOpened = isOpened;
    }
}
public class Enemy
{

    public int x;
    public int y;
    public enum EnemyType { Skeleton, Goblin, Golem };
    public EnemyType enemyType;

    public Enemy(int x, int y, EnemyType enemyType)
    {
        this.x = x;
        this.y = y;
        this.enemyType = enemyType;
    }
    public void RemoveEnemy()
    {
        GameObject.FindGameObjectWithTag("Map").GetComponent<MapGenerator>().RemoveEnemy(this);
    }

}