using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Maps
{
    public class WangTileGenerator
    {

        public static byte[,] bitmasks =
        {
            {128, 64, 32},
            {1, 255, 16},
            {2, 4, 8}
        };

        public static byte[] NorthEdgeMask = { 128, 1, 2 };
        public static byte[] SouthEdgeMask = { 32, 16, 8 };
        public static byte[] EastEdgeMask = { 2, 4, 8 };
        public static byte[] WestEdgeMask = { 128, 64, 32 };

        public static byte[] AllTileIDs =
        {
            0,
            1, 4, 16, 64,
            5, 20, 80, 65,
            7, 28, 112, 193,
            17, 68,
            21, 84, 81, 69,
            23, 92, 113, 197,
            29, 116, 209, 71,
            31, 124, 241, 199,
            85,
            87, 93, 117, 213,
            95, 125, 245, 215,
            119, 221,
            127, 253, 247, 223,
            255
        };

        public static List<byte> matchException = new List<byte>
        {
            119, 221,
            127, 253, 247, 223
        };

        public static List<byte> corridorTiles = new List<byte>
        {
            1, 4, 16, 64,
            5, 20, 80, 65,
            17, 68,
            21, 84, 81, 69,
            85
        };

        public byte[] TileFilter { get; set; }

        public static bool EdgesMatch(Direction direction, byte source, byte neighbor)
        {
            byte[] sourceMask = null;
            byte[] neighborMask = null;

            switch (direction)
            {
                case Direction.North:
                    {
                        sourceMask = NorthEdgeMask;
                        neighborMask = SouthEdgeMask;
                    }
                    break;

                case Direction.South:
                    {
                        sourceMask = SouthEdgeMask;
                        neighborMask = NorthEdgeMask;
                    }
                    break;
                case Direction.East:
                    {
                        sourceMask = EastEdgeMask;
                        neighborMask = WestEdgeMask;
                    }
                    break;
                case Direction.West:
                    {
                        sourceMask = WestEdgeMask;
                        neighborMask = EastEdgeMask;
                    }
                    break;
                case Direction.Northwest:
                case Direction.Northeast:
                case Direction.Southeast:
                case Direction.Southwest:
                    return false;
            }

            if (sourceMask == null || neighborMask == null)
                return false;

            for (int b = 0; b < 3; b++)
            {
                //Debug.Log("sourceMask[b]: " + sourceMask[b]);
                //Debug.Log("source: " + source);
                //Debug.Log("(sourceMask[b] & source): " + (sourceMask[b] & source));
                //Debug.Log("neighborMask[b]: " + neighborMask[b]);
                //Debug.Log("neighbor: " + neighbor);
                //Debug.Log("(neighborMask[b] & neighbor) " + (neighborMask[b] & neighbor));

                bool s = (sourceMask[b] & source) != 0;
                bool n = (neighborMask[b] & neighbor) != 0;

                if (matchException.Contains(neighbor) && n == false && (b == 0 || b == 2))
                    continue;

                if (matchException.Contains(source) && s == false && (b == 0 || b == 2))
                    continue;

                if (s != n)
                    return false;
            }

            return true;
        }

        public static bool EdgeIsOpen(byte tile, Direction direction)
        {
            byte[] edgeMask = null;

            switch (direction)
            {
                case Direction.North:
                    {
                        edgeMask = NorthEdgeMask;
                    }
                    break;

                case Direction.South:
                    {
                        edgeMask = SouthEdgeMask;
                    }
                    break;
                case Direction.East:
                    {
                        edgeMask = EastEdgeMask;
                    }
                    break;
                case Direction.West:
                    {
                        edgeMask = WestEdgeMask;
                    }
                    break;
                case Direction.Northwest:
                case Direction.Northeast:
                case Direction.Southeast:
                case Direction.Southwest:
                    return false;
            }

            if (edgeMask == null)
                return false;

            for (int b = 0; b < 3; b++)
            {
                if ((edgeMask[b] & tile) != 0)
                    return true;
            }

            return false;
        }

        public static (byte, byte) MergeEdges(Direction direction, byte from, byte to)
        {
            byte newFrom = from;
            byte newTo = to;

            switch (direction)
            {
                case Direction.North:
                    {
                        newFrom = (byte)(from | 1);
                        newTo = (byte)(to | 16);
                    }
                    break;

                case Direction.South:
                    {
                        newFrom = (byte)(from | 16);
                        newTo = (byte)(to | 1);
                    }
                    break;
                case Direction.East:
                    {
                        newFrom = (byte)(from | 4);
                        newTo = (byte)(to | 64); ;
                    }
                    break;
                case Direction.West:
                    {
                        newFrom = (byte)(from | 64); ;
                        newTo = (byte)(to | 4); ;
                    }
                    break;
            }

            return (newFrom, newTo);
        }

        private Vector2Int[] neighbors =
        {
            new Vector2Int(0, 1),
            new Vector2Int(1,0),
            new Vector2Int(0,-1),
            new Vector2Int(-1,0)
        };

        Direction[] directions =
        {
            Direction.North,
            Direction.East,
            Direction.South,
            Direction.West
        };

        public WangTileGenerator()
        {
            TileFilter = AllTileIDs;
        }
        
        public WangTileGenerator(byte [] tileFilter)
        {
            TileFilter = tileFilter;
        }


        public WangTileMap CreateMapWithMinimalEmptySpace(int mapWidth, int mapHeight, WangTileMap startingTiles = null)
        {
            WangTileMap tempMap;
            int attempt = 0;
            int maxAttempts = 100;
            int allowedEmptyTiles = (int)Mathf.Round((mapWidth * mapHeight * .1f));

            if (startingTiles != null)
                allowedEmptyTiles += startingTiles.EmptyTiles;

            do
            {
                attempt++;
                tempMap = CreateMap(mapWidth, mapHeight, startingTiles);

                if (tempMap != null && attempt < maxAttempts && tempMap.EmptyTiles > allowedEmptyTiles)
                    tempMap = null;
            }
            while (tempMap == null && attempt <= maxAttempts);

            return tempMap;
        }

        public WangTileMap CreateMap(int mapWidth, int mapHeight, WangTileMap startingTiles = null)
        {
            WangTile[,] tiles = new WangTile[mapWidth, mapHeight];
            if (startingTiles != null)
            {
                for (int y = 0; y < startingTiles.Height; y++)
                {
                    for (int x = 0; x < startingTiles.Width; x++)
                    {
                        tiles[x, y] = startingTiles.tileMap[x, y];
                    }
                }
            }


            int startXtile, startYtile;
            List<WangTile> tilesVisited = new List<WangTile>();
            List<Vector2Int> pointsToBeVisited = new List<Vector2Int>();

            do
            {
                startXtile = MapFactory.RandomGenerator.Next(0, mapWidth-1);
                startYtile = MapFactory.RandomGenerator.Next(0, mapHeight-1);
            }
            while (tiles[startXtile, startYtile] != null);

            tilesVisited.Clear();


            WangTile tile = GetConstrainedTile(tiles, new Vector2Int(startXtile, startYtile));

            if (tile == null || tile.ID == 0)
                return null;

            tile.x = startXtile;
            tile.y = startYtile;

            tiles[startXtile, startYtile] = tile;
            tilesVisited.Add(tile);

            while (tilesVisited.Count > 0)
            {
                WangTile visitedTile = tilesVisited[tilesVisited.Count - 1];

                pointsToBeVisited.Clear();

                for (int p = 0; p < neighbors.Length; p++)
                {
                    Vector2Int pointToVisit = new Vector2Int(visitedTile.x + neighbors[p].x,
                        visitedTile.y + neighbors[p].y);

                    if (pointToVisit.x >= 0 &&
                        pointToVisit.x < mapWidth &&
                        pointToVisit.y >= 0 &&
                        pointToVisit.y < mapHeight)
                    {
                        if (tiles[pointToVisit.x, pointToVisit.y] == null)
                        {

                            if (EdgeIsOpen(visitedTile.ID, directions[p]))
                            {
                                pointsToBeVisited.Add(pointToVisit);
                            }
                        }
                    }
                }

                if (pointsToBeVisited.Count > 0)
                {
                    Vector2Int point;

                    int index = MapFactory.RandomGenerator.Next(0, pointsToBeVisited.Count-1);
                    point = pointsToBeVisited[index];


                    tile = GetConstrainedTile(tiles, point);

                    if (tile == null)
                        return null;

                    tile.x = point.x;
                    tile.y = point.y;

                    tiles[point.x, point.y] = tile;
                    tilesVisited.Add(tile);

                    //Debug.Log("Created Segment " + segment.ID + " at " + point.X + ", " + point.Y);
                }
                else
                {
                    tilesVisited.Remove(visitedTile);
                }

            }

            int emptyTiles = 0;
            int corridorTiles = 0;

            for (int y = 0; y < mapHeight; y++)
            {
                for (int x = 0; x < mapWidth; x++)
                {
                    if (tiles[x, y] == null) // || segments[x, y].ID == 0)
                    {

                        tile = new WangTile();
                        tile.ID = 0;
                        tile.x = x;
                        tile.y = y;

                        tiles[x, y] = tile;

                    }

                    if (tiles[x, y].ID == 0)
                    {
                        emptyTiles++;
                    }

                    if (WangTileGenerator.corridorTiles.Contains(tiles[x, y].ID))
                        corridorTiles++;
                }
            }

            WangTileMap result = new WangTileMap(mapWidth, mapHeight, tiles);
            result.EmptyTiles = emptyTiles;
            result.CorridorTiles = corridorTiles;

            return result;
        }

        private WangTile GetConstrainedTile(WangTile[,] existingTiles, Vector2Int pointToAdd)
        {

            List<byte> filter = new List<byte>(TileFilter);

            //string debug = "Neighbors for (" + pointToAdd.X + ", " + pointToAdd.Y + "): ";

            for (int p = 0; p < neighbors.Length; p++)
            {
                Vector2Int neighborPoint = neighbors[p] + pointToAdd;

                if (neighborPoint.x >= 0 &&
                    neighborPoint.x < existingTiles.GetLength(0) &&
                    neighborPoint.y >= 0 &&
                    neighborPoint.y < existingTiles.GetLength(1))
                {
                    if (existingTiles[neighborPoint.x, neighborPoint.y] != null)
                    {
                        //debug += directions[p] + " - " + existingSegments[neighborPoint.X, neighborPoint.Y].ID + " ";
                        filter = FilterTiles(filter, directions[p],
                            existingTiles[neighborPoint.x, neighborPoint.y].ID);
                    }
                }
                else
                {
                    filter = FilterTiles(filter, directions[p], 0);
                }

            }

            if (filter.Count > 0)
            {
                //debug += " Options available: ";
                //foreach (var f in filter)
                //{
                    //debug += f.ToString() + " ";
                //}

                if (filter.Count > 1 && filter.Contains(0))
                {
                    filter.Remove(0);
                }

                int e = 0;
                while (filter.Count > 1 && e < matchException.Count)
                {
                    filter.Remove(matchException[e++]);
                }


                byte selectedTile = filter[MapFactory.RandomGenerator.Next(0, filter.Count-1)];

                return new WangTile(selectedTile);
            }
            else
            {
                //filter.Add(0);
                return null;
            }
        }

        public List<byte> FilterTiles(List<byte> tiles, Direction direction, byte neighbor)
        {
            byte[] tempTiles = tiles.ToArray();

            //tiles.ForEach(delegate (byte b)// foreach (byte b in tiles. ToList())
            for(int i = 0; i < tempTiles.Length; i++)
            {
                if (!EdgesMatch(direction, tempTiles[i], neighbor))
                {
                    tiles.Remove(tempTiles[i]);
                }
            }

            return tiles;
        }
    }
}