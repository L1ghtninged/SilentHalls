using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapVisualScript : MonoBehaviour
{
    public MapGenerator generatorScript;
    public List<LevelPageInfo> levelPages = new List<LevelPageInfo>();
    public Canvas canvas;
    public Sprite baseTileSprite;
    public RectTransform mapContent;
    public GameObject mapRoot;

    public float tileSize = 32f;
    public float computedSize;
    public float playerSize = 32f;
    public float playerComputedSize;

    public int countOfTiles = 400;

    public int upperPadding = 10;
    public int rightPagePadding = 50;

    private int levelIndex = 0;
    private int playerPositionX = 0;
    private int playerPositionY = 0;

    private int pageIndex = 0;
    private int pageIndexSize = 0;
    private Dictionary<int, LevelVisuals> levelList = new();
    private List<GameObject> leftPageObjects = new();
    private List<GameObject> rightPageObjects = new();

    [Header("Player Icon")]
    public Sprite playerArrowSprite;

    private GameObject playerIcon;



    public bool isMapOpen = false;

    void Start()
    {

        computedSize = tileSize;
        playerComputedSize = playerSize;
        SetUpLevels();
        SetUpGameObjects();
        CreatePlayerIcon();
        DrawBothPages();
    }
    void CreatePlayerIcon()
    {
        playerIcon = CreateEmptyTile(
            0,
            0,
            mapContent,
            playerArrowSprite,
            playerComputedSize
        );

        RectTransform rt = playerIcon.GetComponent<RectTransform>();

        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchorMin = new Vector2(0, 1);
        rt.anchorMax = new Vector2(0, 1);

        playerIcon.name = "PlayerIcon";
        playerIcon.SetActive(false);
    }
    public void DrawPlayer(int levelId, int x, int y, int orientationIndex)
    {
        // hráè není na zobrazené stránce
        int leftId = pageIndex * 2;
        int rightId = pageIndex * 2 + 1;

        bool isLeftPage = levelId == leftId;
        bool isRightPage = levelId == rightId;

        if (!isLeftPage && !isRightPage)
        {
            playerIcon.SetActive(false);
            return;
        }

        int rightPadding = isLeftPage ? 0 : rightPagePadding;

        RectTransform rt = playerIcon.GetComponent<RectTransform>();


        rt.anchoredPosition = new Vector2(
    (y + rightPadding) * computedSize + computedSize * 0.5f,
    -(x + upperPadding) * computedSize - computedSize * 0.5f
);

        // rotace až po pozici
        rt.localRotation = Quaternion.Euler(0,0,-orientationIndex * 90 - 90);



        playerIcon.SetActive(isMapOpen);
    }

    public void SetUpLevels()
    {
        levelList.Clear();

        foreach (var level in generatorScript.savedLevels)
        {
            levelList[level.Key] = GetLevel(level.Value);
        }

        pageIndexSize = Mathf.CeilToInt(levelList.Count / 2f);
    }

    public void SetUpGameObjects()
    {
        for(int i = 0; i < countOfTiles; i++)
        {
            leftPageObjects.Add(CreateEmptyTile(0, 0, mapContent, null, computedSize));
        }
        for (int i = 0; i < countOfTiles; i++)
        {
            rightPageObjects.Add(CreateEmptyTile(0, 0, mapContent, null, computedSize));
        }
    }
    public void DrawBothPages()
    {
        HideLeftPage();
        HideRightPage();

        int leftId = pageIndex * 2;
        int rightId = pageIndex * 2 + 1;

        if (levelList.TryGetValue(leftId, out var leftLevel) && leftLevel.discovered)
        {
            DrawPage(leftLevel, true);
        }

        if (levelList.TryGetValue(rightId, out var rightLevel) && rightLevel.discovered)
        {
            DrawPage(rightLevel, false);
        }

    }

    public void HideLeftPage()
    {
        foreach(GameObject o in leftPageObjects)
        {
            o.SetActive(false);
        }
    }
    public void HideRightPage()
    {
        foreach (GameObject o in rightPageObjects)
        {
            o.SetActive(false);
        }
    }
    bool IsPageDiscovered(int page)
    {
        int leftId = page * 2;
        int rightId = page * 2 + 1;

        bool leftKnown =
            levelList.ContainsKey(leftId) && levelList[leftId].discovered;

        bool rightKnown =
            levelList.ContainsKey(rightId) && levelList[rightId].discovered;

        return leftKnown || rightKnown;
    }

    public GameObject CreateEmptyTile(int x, int y, RectTransform parent, Sprite sprite, float tileSize)
    {
        GameObject tileObj = new GameObject($"Tile_{x}_{y}", typeof(RectTransform), typeof(Image));
        tileObj.transform.SetParent(parent, false);

        RectTransform rt = tileObj.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 1);  // levý horní roh knihy
        rt.anchorMax = new Vector2(0, 1);
        rt.pivot = new Vector2(0, 1);      // pivot doleva nahoru

        rt.sizeDelta = new Vector2(tileSize, tileSize);

        rt.anchoredPosition = new Vector2(
            x * tileSize,
            -y * tileSize
        );
        Image img = tileObj.GetComponent<Image>();
        img.sprite = sprite;
        img.raycastTarget = false;

        tileObj.SetActive(false);
        return tileObj;
    }

    

    public void DrawPage(LevelVisuals level, bool leftPage = true)
    {
        var rightPadding = leftPage ? 0 : rightPagePadding;
        int index = 0;
        foreach(TileInfo tile in level.tiles)
        {
            if (tile.found && tile.tileType != TileType.Wall)
            {
                SetTile(index, tile.x + rightPadding, tile.y + upperPadding, baseTileSprite, leftPage);
                index++;
            }
            
        }
        var list = leftPage ? leftPageObjects : rightPageObjects;
        for (int i = index; i < list.Count; i++)
        { 
            list[i].SetActive(false);
        }
    }
    public void SetTile(int index, int x, int y, Sprite sprite, bool leftPage = true)
    {
        var list = leftPage ? leftPageObjects : rightPageObjects;

        var tile = list[index];
        RectTransform rt = tile.GetComponent<RectTransform>();

        rt.anchoredPosition = new Vector2(
            x * computedSize,
            -y * computedSize
        );

        tile.SetActive(true);
        tile.GetComponent<Image>().sprite = sprite;
    }



    private LevelVisuals GetLevel(Level level)
    {
        LevelVisuals levelVisuals = new LevelVisuals();
        var tiles = level.GenerateMap();

        int width = tiles.GetLength(1);
        int height = tiles.GetLength(0);

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var tileInfo = new TileInfo(
                    x,
                    y,
                    false,
                    tiles[y, x].tileType
                );

                levelVisuals.AddTile(tileInfo);
            }
        }

        return levelVisuals;
    }
    public void DiscoverTile(int levelId, int x, int y)
    {
        if (!levelList.TryGetValue(levelId, out var level))
            return;

        level.DiscoverTile(y, x);

    }
    public void DiscoverLevel(int levelId)
    {
        if (!levelList.TryGetValue(levelId, out var level))
            return;

        level.discovered = true;

    }
    public void UpdateInformation(int x, int y, int orientationIndex)
    {
        this.levelIndex = generatorScript.GetCurrentLevelIndex();
        this.playerPositionX = x;
        this.playerPositionY = y;

        DiscoverLevel(levelIndex);
        DiscoverTile(levelIndex, playerPositionX, playerPositionY);

        DrawPlayer(levelIndex, x, y, orientationIndex);
        DrawBothPages();
    }

    public void ToggleMap()
    {
        if (isMapOpen)
            CloseMap();
        else
            OpenMap();
    }

    public void OpenMap()
    {
        pageIndex = levelIndex - 1;
        isMapOpen = true;
        mapRoot.SetActive(true);
        playerIcon.SetActive(true);
        
    }

    public void CloseMap()
    {
        isMapOpen = false;
        mapRoot.SetActive(false);
        playerIcon.SetActive(false);
    }

    public void FlipLeft()
    {
        int nextPage = pageIndex - 1;
        if (nextPage >= 0 && IsPageDiscovered(nextPage))
        {
            pageIndex = nextPage;
            DrawBothPages();
        }
    }
    public void FlipRight()
    {
        int nextPage = pageIndex + 1;
        if (nextPage < pageIndexSize && IsPageDiscovered(nextPage))
        {
            pageIndex = nextPage;
            DrawBothPages();
        }
    }
    public void RevealAllTiles()
    {
        foreach (var levelPair in levelList)
        {
            var level = levelPair.Value;
            level.discovered = true;

            foreach (var tile in level.tiles)
            {
                tile.found = true; // oznaèí dlaždici jako nalezenou
            }
        }

        DrawBothPages(); // pøekreslí mapu
    }
    




}
