using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Maps
{
    public class WangTileMap
    {
        public int Width { get; }
        public int Height { get; }
        public int EmptyTiles { get; set; }
        public int CorridorTiles { get; set; }

        public WangTile[,] tileMap;
        
        public WangTileMap(int width, int height)
        {
            Width = width;
            Height = height;
            tileMap = new WangTile[width, height];
        }        
        
        public WangTileMap(int width, int height, WangTile[,] tiles)
        {
            Width = width;
            Height = height;
            tileMap = tiles;
        }
    }
}