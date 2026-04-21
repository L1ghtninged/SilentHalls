using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

namespace Assembly_CSharp
{
    public static class EntityPositions
    {
        public static int width = 100;
        public static int height = 100;
        public static PositionScript[,] positions = new PositionScript[width, height];
        public static Tile[,] mapTiles = new Tile[width, height];

        public static void SetUp(Tile[,] tiles)
        {
            width = tiles.GetLength(0);
            height = tiles.GetLength(1);
            positions = new PositionScript[width, height];
            mapTiles = tiles;
        }

        public static bool IsOccupied(int x, int y)
        {
            return positions[x, y] != null;
        }

        public static bool IsWalkable(int x, int y)
        {

            if (x < 0 || y < 0 || x >= mapTiles.GetLength(0) || y >= mapTiles.GetLength(1))
                return false;
            Debug.Log(mapTiles[x, y].walkable);
            Debug.Log(mapTiles.GetLength(0));
            Debug.Log(mapTiles.GetLength(1));
            return mapTiles[x, y].walkable && !IsOccupied(x, y);
        }
        public static bool IsStairsDown(int x, int y)
        {
            return mapTiles[x, y].tileType == TileType.StairsDOWN_NS
                || mapTiles[x, y].tileType == TileType.StairsDOWN_WE;
        }
        public static bool IsStairsUp(int x, int y)
        {
            return mapTiles[x, y].tileType == TileType.StairsUP_NS
                || mapTiles[x, y].tileType == TileType.StairsUP_WE;
        }
        public static void UpdateTileWalkable(int x, int y, bool walkable)
        {
            if (x >= 0 && y >= 0 && x < mapTiles.GetLength(0) && y < mapTiles.GetLength(1))
            {
                var tile = mapTiles[x, y];
                tile.walkable = walkable;
                mapTiles[x, y] = tile;
            }
        }
        public static void RegisterEntity(PositionScript entity, int x, int y)
        {
            try
            {

                positions[x, y] = entity;
            }
            catch(Exception ex)
            {
                Debug.Log(x + ", "+y);
            }
        }
        public static void UnregisterEntity(int x, int y)
        {
            try
            {
                positions[x, y] = null;
            }
            catch (Exception ex)
            {
                Debug.Log(x + ", " + y);
            }
        }
        // Oprava v EntityPositions.cs
        public static bool IsPositionValid(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }


    }
}
