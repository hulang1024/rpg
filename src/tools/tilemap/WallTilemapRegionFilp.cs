using Godot;
using static Godot.GD;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public class WallTilemapRegionFilp : TileMap
{
    public enum FilpDirEnum { None, Copy, H, V }
    
    // 图块的位置 -> 水平翻转方向版本的图块的位置
    // 注意有些图块其实是一样的图形，但是设置了不同的碰撞体
    private static Dictionary<Vector2, Vector2> filpHDict = new Dictionary<Vector2, Vector2>
    {
        { new Vector2(0, 0), new Vector2(0, 3) },
        { new Vector2(1, 0), new Vector2(6, 0) },
        { new Vector2(2, 0), new Vector2(5, 0) },
        { new Vector2(3, 0), new Vector2(4, 0) },
        { new Vector2(3, 1), new Vector2(4, 1) },
        { new Vector2(3, 2), new Vector2(4, 2) },
        { new Vector2(2, 2), new Vector2(5, 2) },
        { new Vector2(2, 3), new Vector2(5, 3) },
        { new Vector2(0, 3), new Vector2(0, 0) },
        { new Vector2(3, 3), new Vector2(3, 3) },
        { new Vector2(4, 3), new Vector2(4, 3) },
        { new Vector2(0, 4), new Vector2(0, 1) },
        { new Vector2(2, 4), new Vector2(5, 4) },
        { new Vector2(1, 4), new Vector2(6, 4) },
        { new Vector2(0, 5), new Vector2(0, 2) },
        { new Vector2(1, 5), new Vector2(6, 5) },
        { new Vector2(2, 5), new Vector2(5, 5) },
        { new Vector2(3, 5), new Vector2(3, 5) },
        { new Vector2(4, 5), new Vector2(4, 5) },
    };

    [Export]
    public Rect2 Src;

    [Export]
    public Rect2 Dest;

    public FilpDirEnum filpDir = FilpDirEnum.None;
    [Export]
    public FilpDirEnum FilpDir
    {
        get { return filpDir; }
        set
        {
            filpDir = value;
            if (filpDir != FilpDirEnum.None)
                Filp(filpDir);
        }
    }

    private void Filp(FilpDirEnum filpDir)
    {
        if (Src.Size < Vector2.One || Dest.Size < Vector2.One)
            return;

        Vector2[] keys = new Vector2[filpHDict.Count];
        filpHDict.Keys.CopyTo(keys, 0);
        foreach (var k in keys)
        {
            filpHDict[filpHDict[k]] = k;
        }

        Print($"在TileMap {Name}中，复制区域{Src}的横向翻转版本到区域{Dest}");
        
        foreach (Vector2 cellV in GetUsedCells())
        {
            if ((Src.Position.x <= cellV.x && cellV.x < Src.Position.x + Src.Size.x) &&
                (Src.Position.y <= cellV.y && cellV.y < Src.Position.y + Src.Size.y))
            {
                var srcTileCoord = GetCellAutotileCoord((int)cellV.x, (int)cellV.y);
                Vector2 destTileCoord;
                int x = 0, y = 0;
                switch (filpDir)
                {
                    case FilpDirEnum.H:
                        destTileCoord = filpHDict[srcTileCoord];
                        x = (int)(Dest.Position.x + Src.Position.x + (Src.Size.x - cellV.x - 1));
                        y = (int)(Dest.Position.y + (cellV.y - Src.Position.y));
                        break;
                    case FilpDirEnum.V:
                        destTileCoord = srcTileCoord;
                        x = (int)(Dest.Position.x + (cellV.x - Src.Position.x));
                        y = (int)(Dest.Position.y + Src.Position.y + (Src.Size.y - cellV.y - 1));
                        break;
                    default:
                        destTileCoord = srcTileCoord;
                        x = (int)(Dest.Position.x + (cellV.x - Src.Position.x));
                        y = (int)(Dest.Position.y + (cellV.y - Src.Position.y));
                        break;
                }
                SetCell(x, y, GetCellv(cellV), false, false, false, destTileCoord);
            }
        }
    }
}