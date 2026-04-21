using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class LevelPageInfo
{
    public int levelId;
    public int pageIndex;
    public bool found;
    public PageSide side;
        
    public LevelPageInfo(int levelId, int pageIndex, bool found, PageSide side)
    {
        this.levelId = levelId;
        this.pageIndex = pageIndex;
        this.found = found;
        this.side = side;
    }
}
public enum PageSide
{
    Left,
    Right

}

