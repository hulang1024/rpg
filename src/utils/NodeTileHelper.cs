using Godot;
using System;

public class NodeTileHelper
{
    public static void ReplaceChild(TileMap tileMap, string tileName, PackedScene packedScene)
    {
        ReplaceChild(tileMap, tileName, () => packedScene.Instance());
    }

    /// <summary>
    /// 将TileMap里某个Tile替换成场景实例
    /// </summary>
    /// <param name="tileMap"></param>
    /// <param name="tileName">占位Tile的name</param>
    /// <param name="nodeBuilder">场景实例创建函数</param>
    public static void ReplaceChild(TileMap tileMap, string tileName, Func<dynamic> nodeBuilder)
    {
        var tileId = tileMap.TileSet.FindTileByName(tileName);
        if (tileId == -1) return;

        foreach (Vector2 pos in tileMap.GetUsedCellsById(tileId))
        {
            var node = nodeBuilder() as Node2D;
            node.Position = tileMap.MapToWorld(pos);
            tileMap.AddChild(node);
            tileMap.SetCellv(pos, -1);
        }
    }
}