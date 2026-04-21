using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class LevelVisuals
{
    public int levelId;
    public bool discovered;
    public List<TileInfo> tiles;

    private Dictionary<(int x, int y), TileInfo> tileLookup;

    public LevelVisuals()
    {
        discovered = false;
        tiles = new List<TileInfo>();
        tileLookup = new Dictionary<(int, int), TileInfo>();
    }

    public void AddTile(TileInfo tile)
    {
        tiles.Add(tile);
        tileLookup[(tile.x, tile.y)] = tile;
    }

    public void DiscoverTile(int x, int y)
    {
        if (tileLookup.TryGetValue((x, y), out var tile))
        {
            tile.found = true;
        }
    }
}



public class TileInfo
{
    public int x;
    public int y;
    public bool found;
    public TileType tileType;

    public TileInfo(int x, int y, bool found, TileType tileType)
    {
        this.x = x;
        this.y = y;
        this.found = found;
        this.tileType = tileType;
    }
}
