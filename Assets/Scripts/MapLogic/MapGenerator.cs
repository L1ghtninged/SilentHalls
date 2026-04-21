using Assembly_CSharp;
using System.Collections.Generic;
using UnityEngine;
using static KeyItem;

public class MapGenerator : MonoBehaviour
{
    public DropItemScript dropItemScript;
    public GameObject playerObject;
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public GameObject doorPrefab;
    public GameObject stairsUpPrefab;
    public GameObject stairsDownPrefab;

    public GameObject goldLockPrefab;
    public GameObject silverLockPrefab;
    public GameObject buttonPrefab;

    public GameObject skeletonPrefab;
    public GameObject goblinPrefab;
    public GameObject golemPrefab;

    public GameObject inventoryItemPrefab;
    public Item[] possibleItems;
    public float cellSize = 5;

    public Dictionary<GameObject, Pos> spawnedObjects = new Dictionary<GameObject, Pos>();
    public Dictionary<int, Level> savedLevels = new(); // Uchováme levely
    public int currentLevelIndex = 1;

    private void Start()
    {
        SetUpLevels();
        GenerateLevel(savedLevels[1]);
        SetPlayerPosition(savedLevels[1].startX, savedLevels[1].startY);
        //Level testingLevel = new Level(Levels.testingLevel);
        //SetPlayerPosition(testingLevel.startX, testingLevel.startY);
        //GenerateLevel(testingLevel);

    }
    public void SetUpLevels()
    {
        Level level1 = new Level(Levels.level1);
        Level level2 = new Level(Levels.level2);
        Level level3 = new Level(Levels.level3);
        Level level4 = new Level(Levels.level4);
        Level level5 = new Level(Levels.level5);

        level1.levelNumber = 1;
        level2.levelNumber = 2;
        level3.levelNumber = 3;
        level4.levelNumber = 4;
        level5.levelNumber = 5;

        Generations1(level1);
        Generations2(level2);
        Generations3(level3);
        Generations4(level4);
        Generations5(level5);

        savedLevels.Add(1, level1);
        savedLevels.Add(2, level2);
        savedLevels.Add(3, level3);
        savedLevels.Add(4, level4);
        savedLevels.Add(5, level5);
        


    }
    public GameObject GetObjectByPos(Pos pos)
    {
        GameObject obj = null;
        foreach (var pair in spawnedObjects)
        {
            if (pair.Value.x == pos.x && pair.Value.y == pos.y)
            {
                obj = pair.Key;
                break;
            }
        }
        return obj;

    }
    public void GenerateLevel(Level level)
    {
        foreach (var tile in spawnedObjects.Keys)
        {
            Destroy(tile);
        }
        spawnedObjects.Clear();
        dropItemScript.droppedItems.Clear();
        dropItemScript.visuals.UpdateBagsVisibility();
        var tiles = level.GenerateMap();
        EntityPositions.SetUp(tiles);
        GenerateMapVisuals(tiles, level.levelMap.GetLength(0), level.levelMap.GetLength(1));
        GenerateLocks(level); 
        GenerateButtonObjects(level);
        GenerateEnemies(level);
        GenerateItems(level);
    }
    public void GenerateLocks(Level level)
    {
        foreach (var _lock in level.locks)
        {
            Vector3 position = new Vector3(_lock.x * cellSize, cellSize/2, _lock.y * cellSize);
            Pos lockPos = new();
            lockPos.x = _lock.x;
            lockPos.y = _lock.y;

            Pos doorPos = new();
            doorPos.x = _lock.doorX;
            doorPos.y = _lock.doorY;

            GameObject doorObject = GetObjectByPos(doorPos);

            if (_lock.keyType == KeyType.Gold)
            {
                var l = Instantiate(goldLockPrefab, position, Quaternion.Euler(0, 90 * (_lock.direction -1), 0), transform);
                var lockScript = l.GetComponent<DoorLock>();
                if (lockScript != null && doorObject != null)
                {
                    lockScript.door = doorObject.GetComponent<DoorController>();
                }
                lockScript._lock = _lock;
                if (_lock.isOpened) lockScript.OnKeyUsed();
                spawnedObjects.Add(l, lockPos);
            }
            else if (_lock.keyType == KeyType.Silver)
            {
                var l = Instantiate(silverLockPrefab, position, Quaternion.Euler(0, 90 * (_lock.direction -1), 0), transform);
                var lockScript = l.GetComponent<DoorLock>();
                if (lockScript != null && doorObject != null)
                {
                    lockScript.door = doorObject.GetComponent<DoorController>();
                }
                lockScript._lock = _lock;
                if (_lock.isOpened) lockScript.OnKeyUsed();
                spawnedObjects.Add(l, lockPos);
            }
        }
    }
    public void GenerateButtonObjects(Level level)
    {
        foreach(var button in level.ButtonObjects)
        {
            Vector3 position = new Vector3(button.x * cellSize, cellSize / 2, button.y * cellSize);
            Pos ButtonObjectPos = new Pos();
            ButtonObjectPos.x = button.x;
            ButtonObjectPos.y = button.y;

            Pos doorPos = new Pos();
            doorPos.x = button.doorX;
            doorPos.y = button.doorY;

            GameObject doorObject = GetObjectByPos(doorPos);
            var l = Instantiate(buttonPrefab, position, Quaternion.Euler(0, 90 * button.direction, 0), transform);
            var buttonObjectScript = l.GetComponent<ButtonTrigger>();
            buttonObjectScript.button = button;
            if (buttonObjectScript != null && doorObject != null)
            {
                buttonObjectScript.door = doorObject.GetComponent<DoorController>();
                if (button.isOpened) 
                {
                    buttonObjectScript.OnClick();
                    button.isOpened = !button.isOpened;
                }
            }
            spawnedObjects.Add(l, ButtonObjectPos);
        }
    }
    public void GenerateEnemies(Level level)
    {
        foreach (var enemy in level.enemies)
        {
            Pos pos = new();
            pos.x = enemy.x;
            pos.y = enemy.y;
            GameObject enemyPrefab = this.skeletonPrefab;
            switch (enemy.enemyType)
            {
                case Enemy.EnemyType.Skeleton: enemyPrefab = skeletonPrefab;
                    break;
                case Enemy.EnemyType.Goblin: enemyPrefab = goblinPrefab;
                    break;
                case Enemy.EnemyType.Golem: enemyPrefab = golemPrefab; 
                    break;

            }
            var l = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, transform);
            spawnedObjects.Add(l, pos);
            var enemyStats = l.GetComponent<EnemyStatScript>();
            enemyStats.enemy = enemy;
            enemyStats.level = level.levelNumber;
            SetEnemyPosition(l, enemy.x, enemy.y);
        }
    }
    public void GenerateItems(Level level)
    {
        if (dropItemScript == null) return;
        foreach(var pair in level.items)
        {
            foreach(var item in pair.Value)
            {
                GameObject newItem = Instantiate(inventoryItemPrefab);
                InventoryItem inventoryItem = newItem.GetComponent<InventoryItem>();
                inventoryItem.InitialiseItem(item);
                dropItemScript.AddDroppedItemOnStart(inventoryItem, pair.Key);
                Destroy(newItem);
            }
            
        }
    }

    public void GenerateMapVisuals(Tile[,] mapTiles, int width, int height)
    {
        

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {

                Tile tile = mapTiles[x, y];
                Vector3 position = new Vector3(x * cellSize, 0, y * cellSize);
                Pos pos;
                pos.x = x; 
                pos.y = y;
                if(tile.tileType == TileType.Indicator)
                {
                    Debug.LogWarning(pos.x + ":" + pos.y);
                }
                switch (tile.tileType)
                {
                    case TileType.Indicator:
                    case TileType.Floor:
                        spawnedObjects.Add(Instantiate(floorPrefab, position, Quaternion.identity, transform), pos);
                        break;
                    case TileType.Wall:
                        position.y = 3;
                        spawnedObjects.Add(Instantiate(wallPrefab, position, Quaternion.identity, transform), pos);
                        break;
                    case TileType.Door_NS:
                    case TileType.Door_WE:
                        var door = Instantiate(doorPrefab, position,
                            tile.tileType == TileType.Door_WE ? Quaternion.identity : Quaternion.Euler(0, -90, 0),
                            transform);
                        door.GetComponent<DoorController>()?.Initialize(x, y, tile.tileType == TileType.Door_NS, false);
                        spawnedObjects.Add(door, pos);
                        break;
                    case TileType.StairsDOWN_NS:
                    case TileType.StairsDOWN_WE:

                        var stairs = Instantiate(stairsDownPrefab, position, tile.tileType == TileType.StairsDOWN_NS ? Quaternion.Euler(0, 90, 0) :
                            Quaternion.Euler(0, 180, 0), transform);
                        spawnedObjects.Add(stairs, pos);
                        break;
                    case TileType.StairsUP_NS:
                    case TileType.StairsUP_WE:
                        var stairsUp = Instantiate(stairsUpPrefab, position, tile.tileType == TileType.StairsUP_WE ? Quaternion.identity:
                            Quaternion.Euler(0, -90, 0), transform);
                        spawnedObjects.Add(stairsUp, pos);
                        break;
                    case TileType.StartPlayerPosition:
                    case TileType.EndPlayerPosition:
                        spawnedObjects.Add(Instantiate(floorPrefab, position, Quaternion.identity, transform), pos);
                        break;

                }
                Vector3 positionCeiling = new Vector3(x * cellSize, cellSize + 3, y * cellSize);
                spawnedObjects.Add(Instantiate(wallPrefab, positionCeiling, Quaternion.identity, transform), pos);
            }
        }
    }
    

    public void SetPlayerPosition(int logicalX, int logicalY)
    {

        if (playerObject == null)
        {
            playerObject = GameObject.FindGameObjectWithTag("Player");
        }
        // Převod logické pozice na vizuální (převrácení Y)
        int visualY = EntityPositions.height - 1 - logicalY;
        Vector3 worldPos = new Vector3(logicalX * cellSize, 1.5f, visualY * cellSize);
        playerObject.GetComponent<GridMovement>().ForceStopAt(worldPos);

        playerObject.transform.position = worldPos;
        playerObject.GetComponent<PositionScript>().SetPosition(logicalX, logicalY);
    }
    public void SetEnemyPosition(GameObject enemyObject, int logicalX, int logicalY)
    {
        if (enemyObject == null)
        {
            Debug.LogError("Enemy object is null");
            return;
        }

        var posScript = enemyObject.GetComponent<PositionScript>();
        if (posScript == null)
        {
            Debug.LogError("Enemy prefab is missing PositionScript!");
            return;
        }

        int visualY = EntityPositions.height - 1 - logicalY;
        Vector3 worldPos = new Vector3(logicalX * cellSize, 0, visualY * cellSize);

        enemyObject.transform.position = worldPos;
        posScript.SetPosition(logicalX, logicalY);
    }
    private void AddItem(Level level, int x, int y, string itemName)
    {
        Item item = null;
        foreach (var i in possibleItems)
        {
            if (i.itemName == itemName)
            {
                item = i;
            }
        }
        if (item == null) return;
        level.AddItem(x, y, item);
        
    }
    public void AddItem(Item item, int x, int y)
    {
        Level level = GetLevel(GetCurrentLevelIndex());

        if (item == null) return;
        level.AddItem(x, y, item);

    }
    public void RemoveEnemy(Enemy enemy)
    {
        savedLevels[currentLevelIndex].enemies.Remove(enemy);
    }
    public void RemoveItem(Item item, Position position)
    {
        savedLevels[currentLevelIndex].items[position].Remove(item);
    }
    public int GetCurrentLevelIndex()
    {
        return currentLevelIndex;
    }

    public void SetCurrentLevelIndex(int index)
    {
        currentLevelIndex = index;
    }

    public bool HasLevel(int index)
    {
        return savedLevels.ContainsKey(index);
    }

    public Level GetLevel(int index)
    {
        return savedLevels[index];
    }
    public void Generations1(Level level)
    {
        AddItem(level, 1, 23, "Dagger");
        AddItem(level, 1, 23, "SilverKey");
        AddItem(level, 1, 23, "IceWand");
        level.AddLock(3, 23, 4, 24, 2, KeyType.Silver);

        AddItem(level, 1, 25, "HealthPotion");
        AddItem(level, 4, 31, "Sword");
        AddItem(level, 4, 33, "ManaPotion");
        AddItem(level, 22, 31, "HealthPotion");
        level.AddEnemy(20, 31, Enemy.EnemyType.Skeleton);
        AddItem(level, 10, 22, "HealthPotion");
        level.AddEnemy(15, 26, Enemy.EnemyType.Goblin);
        level.AddEnemy(12, 19, Enemy.EnemyType.Goblin);
        AddItem(level, 6, 19, "GoldKey");
        level.AddLock(10, 7, 10, 6, 0, KeyType.Gold);
        level.AddEnemy(5, 10, Enemy.EnemyType.Skeleton);
        level.AddEnemy(5, 16, Enemy.EnemyType.Skeleton);
        AddItem(level, 2, 13, "LeatherBoots");
        level.AddEnemy(5, 13, Enemy.EnemyType.Goblin);
        AddItem(level, 7, 14, "ManaPotion");
        AddItem(level, 7, 12, "ManaPotion");
        level.AddEnemy(13, 13, Enemy.EnemyType.Goblin);
        AddItem(level, 19, 13, "HealthPotion");
        level.AddEnemy(23, 13, Enemy.EnemyType.Skeleton);
        level.AddEnemy(19, 16, Enemy.EnemyType.Skeleton);
        AddItem(level, 2, 5, "LeatherArmor");
        level.AddEnemy(4, 3, Enemy.EnemyType.Goblin);
        AddItem(level, 5, 5, "ManaPotion");
        level.AddEnemy(8, 4, Enemy.EnemyType.Goblin);
        level.AddEnemy(18, 1, Enemy.EnemyType.Goblin);
        level.AddEnemy(18, 7, Enemy.EnemyType.Goblin);
        AddItem(level, 23, 4, "HealthPotion");
    }
    public void Generations2(Level level)
    {
        level.AddLock(10, 5, 10, 6, 0, KeyType.Gold);

        AddItem(level, 1, 25, "HealthPotion");
        AddItem(level, 13, 30, "HealthPotion");
        AddItem(level, 15, 29, "HealthPotion");
        AddItem(level, 13, 18, "HealthPotion");
        AddItem(level, 22, 11, "HealthPotion");
        AddItem(level, 15, 13, "HealthPotion");
        AddItem(level, 13, 11, "HealthPotion");

        AddItem(level, 6, 30, "ManaPotion");
        AddItem(level, 6, 18, "ManaPotion");
        AddItem(level, 26, 28, "ManaPotion");
        AddItem(level, 22, 24, "ManaPotion");
        AddItem(level, 16, 7, "ManaPotion");
        AddItem(level, 8, 11, "ManaPotion");

        AddItem(level, 15, 12, "IronHelmet");
        AddItem(level, 9, 24, "IronArmor");
        AddItem(level, 2, 5, "IronLeggins");

        AddItem(level, 13, 16, "LongSword");
        AddItem(level, 24, 21, "LongSword");
        AddItem(level, 6, 14, "FireWand");

        AddItem(level, 5, 21, "LeatherHelmet");
        AddItem(level, 5, 27, "IronBoots");
        AddItem(level, 2, 3, "GoldKey");

        level.AddEnemy(4, 28, Enemy.EnemyType.Skeleton);
        level.AddEnemy(4, 20, Enemy.EnemyType.Skeleton);
        level.AddEnemy(6, 24, Enemy.EnemyType.Skeleton);
        level.AddEnemy(7, 23, Enemy.EnemyType.Skeleton);
        level.AddEnemy(10, 18, Enemy.EnemyType.Skeleton);
        level.AddEnemy(10, 30, Enemy.EnemyType.Skeleton);
        level.AddEnemy(12, 22, Enemy.EnemyType.Skeleton);
        level.AddEnemy(13, 28, Enemy.EnemyType.Skeleton);
        level.AddEnemy(17, 19, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 30, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 21, Enemy.EnemyType.Skeleton);
        level.AddEnemy(25, 28, Enemy.EnemyType.Skeleton);
        level.AddEnemy(9, 11, Enemy.EnemyType.Skeleton);
        level.AddEnemy(8, 10, Enemy.EnemyType.Skeleton);
        level.AddEnemy(12, 7, Enemy.EnemyType.Skeleton);

        level.AddEnemy(8, 26, Enemy.EnemyType.Goblin);
        level.AddEnemy(11, 25, Enemy.EnemyType.Goblin);
        level.AddEnemy(15, 30, Enemy.EnemyType.Goblin);
        level.AddEnemy(15, 27, Enemy.EnemyType.Goblin);
        level.AddEnemy(17, 27, Enemy.EnemyType.Goblin);
        level.AddEnemy(22, 27, Enemy.EnemyType.Goblin);
        level.AddEnemy(21, 24, Enemy.EnemyType.Goblin);
        level.AddEnemy(6, 16, Enemy.EnemyType.Goblin);
        level.AddEnemy(13, 13, Enemy.EnemyType.Goblin);
        level.AddEnemy(8, 13, Enemy.EnemyType.Goblin);
        level.AddEnemy(6, 9, Enemy.EnemyType.Goblin);
        level.AddEnemy(6, 7, Enemy.EnemyType.Goblin);
        level.AddEnemy(15, 9, Enemy.EnemyType.Goblin);

    }
    public void Generations3(Level level)
    {
        level.AddButtonObject(4, 24, 5, 24, 0);
        level.AddButtonObject(10, 7, 10, 6, 3);

        AddItem(level, 1, 25, "HealthPotion");
        AddItem(level, 2, 5, "HealthPotion");
        AddItem(level, 8, 8, "HealthPotion");
        AddItem(level, 8, 20, "HealthPotion");
        AddItem(level, 7, 26, "HealthPotion");
        AddItem(level, 19, 32, "HealthPotion");
        AddItem(level, 26, 13, "HealthPotion");
        AddItem(level, 12, 14, "HealthPotion");

        AddItem(level, 3, 25, "ManaPotion");
        AddItem(level, 6, 34, "ManaPotion");
        AddItem(level, 10, 16, "ManaPotion");
        AddItem(level, 17, 34, "ManaPotion");
        AddItem(level, 26, 30, "ManaPotion");
        AddItem(level, 26, 11, "ManaPotion");

        AddItem(level, 20, 32, "GoldArmor");
        AddItem(level, 26, 12, "GreenSword");
        AddItem(level, 22, 23, "IceWand");

        level.AddEnemy(6, 22, Enemy.EnemyType.Skeleton);
        level.AddEnemy(6, 26, Enemy.EnemyType.Skeleton);
        level.AddEnemy(6, 30, Enemy.EnemyType.Skeleton);
        level.AddEnemy(11, 22, Enemy.EnemyType.Skeleton);
        level.AddEnemy(13, 34, Enemy.EnemyType.Skeleton);
        level.AddEnemy(17, 33, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 21, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 24, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 26, Enemy.EnemyType.Skeleton);
        level.AddEnemy(23, 30, Enemy.EnemyType.Skeleton);
        level.AddEnemy(23, 34, Enemy.EnemyType.Skeleton);
        level.AddEnemy(26, 20, Enemy.EnemyType.Skeleton);
        level.AddEnemy(26, 21, Enemy.EnemyType.Skeleton);
        level.AddEnemy(8, 7, Enemy.EnemyType.Skeleton);
        level.AddEnemy(16, 7, Enemy.EnemyType.Skeleton);
        level.AddEnemy(11, 9, Enemy.EnemyType.Skeleton);
        level.AddEnemy(10, 10, Enemy.EnemyType.Skeleton);

        level.AddEnemy(6, 4, Enemy.EnemyType.Goblin);
        level.AddEnemy(7, 4, Enemy.EnemyType.Goblin);
        level.AddEnemy(11, 12, Enemy.EnemyType.Goblin);
        level.AddEnemy(14, 12, Enemy.EnemyType.Goblin);
        level.AddEnemy(13, 18, Enemy.EnemyType.Goblin);
        level.AddEnemy(8, 18, Enemy.EnemyType.Goblin);
        level.AddEnemy(11, 19, Enemy.EnemyType.Goblin);
        level.AddEnemy(6, 32, Enemy.EnemyType.Goblin);
        level.AddEnemy(9, 28, Enemy.EnemyType.Goblin);
        level.AddEnemy(15, 23, Enemy.EnemyType.Goblin);
        level.AddEnemy(15, 24, Enemy.EnemyType.Goblin);
        level.AddEnemy(20, 15, Enemy.EnemyType.Goblin);
        level.AddEnemy(23, 32, Enemy.EnemyType.Goblin);
        level.AddEnemy(26, 34, Enemy.EnemyType.Goblin);
        level.AddEnemy(24, 23, Enemy.EnemyType.Goblin);
        level.AddEnemy(26, 17, Enemy.EnemyType.Goblin);
    }
    public void Generations4(Level level)
    {
        level.AddButtonObject(3, 23, 2, 22, 2);
        level.AddLock(3, 25, 2, 26, 1, KeyType.Gold);

        AddItem(level, 2, 3, "HealthPotion");
        AddItem(level, 8, 19, "HealthPotion");
        AddItem(level, 8, 29, "HealthPotion");
        AddItem(level, 8, 35, "HealthPotion");
        AddItem(level, 15, 11, "HealthPotion");
        AddItem(level, 21, 11, "HealthPotion");
        AddItem(level, 22, 32, "HealthPotion");

        AddItem(level, 2, 5, "ManaPotion");
        AddItem(level, 9, 15, "ManaPotion");
        AddItem(level, 6, 21, "ManaPotion");
        AddItem(level, 12, 27, "ManaPotion");
        AddItem(level, 9, 35, "ManaPotion");

        AddItem(level, 6, 33, "GreenBoots");
        AddItem(level, 9, 33, "Axe");

        level.AddEnemy(9, 21, Enemy.EnemyType.Golem);
        level.AddEnemy(9, 27, Enemy.EnemyType.Golem);

        level.AddEnemy(9, 11, Enemy.EnemyType.Skeleton);
        level.AddEnemy(12, 12, Enemy.EnemyType.Skeleton);
        level.AddEnemy(15, 15, Enemy.EnemyType.Skeleton);
        level.AddEnemy(12, 21, Enemy.EnemyType.Skeleton);
        level.AddEnemy(6, 27, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 21, Enemy.EnemyType.Skeleton);
        level.AddEnemy(26, 21, Enemy.EnemyType.Skeleton);
        level.AddEnemy(20, 27, Enemy.EnemyType.Skeleton);
        level.AddEnemy(26, 27, Enemy.EnemyType.Skeleton);

        level.AddEnemy(18, 1, Enemy.EnemyType.Goblin);
        level.AddEnemy(16, 4, Enemy.EnemyType.Goblin);
        level.AddEnemy(18, 7, Enemy.EnemyType.Goblin);
        level.AddEnemy(21, 12, Enemy.EnemyType.Goblin);
        level.AddEnemy(24, 12, Enemy.EnemyType.Goblin);
    }
    public void Generations5(Level level)
    {
        AddItem(level, 6, 2, "HealthPotion");
        AddItem(level, 18, 1, "HealthPotion");
        AddItem(level, 25, 2, "HealthPotion");
        AddItem(level, 6, 13, "HealthPotion");
        AddItem(level, 8, 21, "HealthPotion");
        AddItem(level, 14, 20, "HealthPotion");
        AddItem(level, 25, 19, "HealthPotion");

        AddItem(level, 6, 5, "ManaPotion");
        AddItem(level, 6, 8, "ManaPotion");
        AddItem(level, 6, 19, "ManaPotion");
        AddItem(level, 14, 1, "ManaPotion");
        AddItem(level, 18, 9, "ManaPotion");
        AddItem(level, 18, 12, "ManaPotion");
        AddItem(level, 26, 8, "ManaPotion");
        AddItem(level, 26, 13, "ManaPotion");

        AddItem(level, 26, 2, "IceWand");
        AddItem(level, 26, 19, "FireWand");

        level.AddEnemy(10, 5, Enemy.EnemyType.Golem);
        level.AddEnemy(16, 5, Enemy.EnemyType.Golem);
        level.AddEnemy(23, 5, Enemy.EnemyType.Golem);
        level.AddEnemy(23, 16, Enemy.EnemyType.Golem);
        level.AddEnemy(16, 16, Enemy.EnemyType.Golem);
        level.AddEnemy(9, 16, Enemy.EnemyType.Golem);
    }

}





