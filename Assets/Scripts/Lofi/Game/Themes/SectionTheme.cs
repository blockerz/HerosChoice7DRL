using Lofi.Maps;
using System;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "NewTheme", menuName = "ScriptableObjects/SectionTheme", order = 1)]
namespace Lofi.Game
{
    public class SectionTheme : ScriptableObject
    {
        public enum DefaultFileIndex
        {
            Background = 0,
            SingleWall = 1,
            Walls_DR = 2,
            Walls_DLR = 3,
            Walls_DL = 4,
            Walls_UR = 10,
            Walls_ULR = 11,
            Walls_UL = 12,

        }
        static GameObject mapTilePrefab;

        public string ThemeName { get; set; }
        public string SpriteDirectory { get; set; }
        public string DefaultTileFile { get; set; }
        public Sprite[] defaultSprites;

        public int doorWidth = 3;
        public int doorHeight = 3;
        public int maxSectionSize = int.MaxValue;

        public static List<Vector2Int> neighborsWDiagonals = new List<Vector2Int>
        {
                new Vector2Int(0, 1),
                new Vector2Int(1,1),
                new Vector2Int(1,0),
                new Vector2Int(1,-1),
                new Vector2Int(0, -1),
                new Vector2Int(-1,-1),
                new Vector2Int(-1,0),
                new Vector2Int(-1,1)
        };


        public Dictionary<byte, Rect> tilingZones;

        public SectionTheme()
        {


        }

        public void Initialize(string themeName = "Forest", string tileFileName = "ForestDefault", int maxSectSize = int.MaxValue)
        {
            if (mapTilePrefab == null)
                mapTilePrefab = (GameObject)Resources.Load("prefabs/GameMapTile", typeof(GameObject));

            this.maxSectionSize = maxSectSize;
            ThemeName = themeName;
            SpriteDirectory = "Sprites/Themes";
            DefaultTileFile = tileFileName;
            defaultSprites = Resources.LoadAll<Sprite>(GetDefaultTileFile());

            if(defaultSprites == null)
            {
                Debug.LogError("Failed to load: " + GetDefaultTileFile());
            }
        }



        public virtual string GetDefaultTileFile()
        {
            return SpriteDirectory + "/" + ThemeName + "/" + DefaultTileFile;
        }

        public virtual Sprite GetDefaultTileSpriteForIndex(DefaultFileIndex index)
        {
            //Debug.Log(SpriteDirectory + "/" + ThemeName + "/" + DefaultTileFile + "_" + (int)index);
            return defaultSprites[(int)index];
        }

        public virtual Sprite GetBackgroundSprite()
        {
            return GetDefaultTileSpriteForIndex(SectionTheme.DefaultFileIndex.Background);
        }

        public virtual Color GetBackgroundSpriteColor(int dungeonNumber = 0)
        {
            return Color.white;
        }

        //public virtual void FillSectionTiles(GameMapSection section)
        //{
        //    for (int y = 0; y < section.Height; y++)
        //    {
        //        for (int x = 0; x < section.Width; x++)
        //        {
        //            if (x == 0 || x == section.Width - 1 || y == 0 || y == section.Height - 1)
        //            {
        //                GameObject tile = section.GetTile(x, y);

        //                tile = Instantiate(mapTilePrefab, section.transform);
        //                tile.name = "Tile: " + x + ", " + y;
        //                tile.transform.position = new Vector3(x, y, -0.01f);
        //                tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(SectionTheme.DefaultFileIndex.SingleWall);
        //            }

        //        }

        //    }
        //}

        public void SetTilingZones(GameMapSection section)
        {
            tilingZones = new Dictionary<byte, Rect>();

            int westRegionWidth = (section.Width - doorWidth) / 2; // 7
            int eastRegionWidth = section.Width - doorWidth - westRegionWidth; // 7
            int northRegionHeight = (section.Height - doorHeight) / 2; // 4
            int southRegionHeight = section.Height - doorHeight - northRegionHeight; // 4

            byte[,] bitmasks =
            {
            {128, 64, 32},
            {1, 255, 16},
            {2, 4, 8}
        };

            Rect[,] zones =
    {
            {new Rect(0,section.Height-northRegionHeight,westRegionWidth,northRegionHeight),
                new Rect(0,section.Height-northRegionHeight-doorHeight, westRegionWidth, doorHeight),
                new Rect(0,0,westRegionWidth,southRegionHeight)},
            {new Rect(westRegionWidth,section.Height-northRegionHeight, doorWidth, northRegionHeight),
                new Rect(westRegionWidth,section.Height-northRegionHeight-doorHeight, doorWidth, doorHeight),
                new Rect(westRegionWidth,0, doorWidth, southRegionHeight)},
            {new Rect(westRegionWidth+doorWidth,section.Height-northRegionHeight, eastRegionWidth, northRegionHeight),
                new Rect(westRegionWidth+doorWidth,section.Height-northRegionHeight-doorHeight, eastRegionWidth, doorHeight),
                new Rect(westRegionWidth+doorWidth,0, eastRegionWidth, southRegionHeight)}
        };


            for (int y = 0; y < 3; y++)
            {
                for (int x = 0; x < 3; x++)
                {
                    tilingZones.Add(bitmasks[x, y], zones[x, y]);
                }
            }
        }

        public virtual void FillSectionTiles(GameMapSection section, int dungeonNumber = 0)
        {
            bool fillClosedEdges = false;

            SetTilingZones(section);

            foreach (var zone in tilingZones)
            {
                if ((section.Section.TileID & zone.Key) == 0)
                {
                    //fillClosedEdges = (MapFactory.RandomGenerator.Next(0, 1) == 0);
                    for (int y = Mathf.RoundToInt(zone.Value.y); y < Mathf.RoundToInt(zone.Value.y + zone.Value.height); y++)
                    {
                        for (int x = Mathf.RoundToInt(zone.Value.x); x < Mathf.RoundToInt(zone.Value.x + zone.Value.width); x++)
                        {
                            if (fillClosedEdges || (x == 0 || x == section.Width - 1 || y == 0 || y == section.Height - 1)
                                || (x == 1 || x == section.Width - 2 || y == 1 || y == section.Height - 2)
                                || (x == 2 && y == 2)
                                || (x == 2 && y == section.Height - 3)
                                || (x == section.Width - 3 && y == 2)
                                || (x == section.Width - 3 && y == section.Height - 3)
                                )
                            {
                                InstantiateTile(section, x, y);
                            }

                        }

                    }
                }
            }

            //CreateRandomTilesIfOpen(section, 4);
            CreateGroveTilesIfOpen(section, MapFactory.RandomGenerator.Next(2, 8), MapFactory.RandomGenerator.Next(2, 5));

            for (int y = 0; y < section.Height; y++)
            {
                for (int x = 0; x < section.Width; x++)
                {
                    GameObject tile = section.GetTile(x, y);
                    if (tile != null)
                    {
                        DefaultFileIndex index = GetTileBasedOnNeighbors(x, y, section);
                        tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(index);
                    }
                }
            }


        }

        protected static void InstantiateTile(GameMapSection section, int x, int y)
        {
            GameObject tile = section.GetTile(x, y);

            tile = Instantiate(mapTilePrefab, section.transform);
            tile.name = "Tile: " + x + ", " + y;
            tile.transform.position = section.transform.position + new Vector3(x, y, -0.01f);
            section.SetTile(x, y, tile);
        }


        public void CreateGroveTilesIfOpen(GameMapSection section, int width = 2, int height = 2)
        {
            if (width <= 1 || height <= 1)
                return;

            int xpos = MapFactory.RandomGenerator.Next(1, section.Width - 2 - (width));
            int ypos = MapFactory.RandomGenerator.Next(1, section.Height - 2 - (height));

            for (int y = ypos; y < ypos + height * 2; y += 2)
            {
                for (int x = xpos; x < xpos + width * 2; x += 2)
                {
                    CreateSingleTileIfOpen(section, x, y);
                }
            }

        }

        private void CreateRandomTilesIfOpen(GameMapSection section, int number = 2)
        {
            if (number <= 0)
                return;

            int attempts = number + 10;

            do
            {
                int xpos = MapFactory.RandomGenerator.Next(2, section.Width - 3);
                int ypos = MapFactory.RandomGenerator.Next(2, section.Height - 3);

                if (CreateSingleTileIfOpen(section, xpos, ypos))
                    number--;
                attempts--;
            }
            while (number > 0 && attempts > 0);

        }

        public bool CreateSingleTileIfOpen(GameMapSection section, int x, int y)
        {
            if (x <= 1 || y <= 1 || x >= section.Width - 2 || y >= section.Height - 2)
                return false;

            foreach (var neighbor in neighborsWDiagonals)
            {
                if (section.GetTile(x + neighbor.x, y + neighbor.y) != null)
                    return false;
            }

            InstantiateTile(section, x, y);
            return true;
        }

        private DefaultFileIndex GetTileBasedOnNeighbors(int x, int y, GameMapSection section)
        {
            bool up = (section.GetTile(x, y + 1) != null) || y == section.Height - 1;
            bool down = (section.GetTile(x, y - 1) != null) || y == 0;
            bool left = (section.GetTile(x - 1, y) != null) || x == 0;
            bool right = (section.GetTile(x + 1, y) != null) || x == section.Width - 1;

            if (!up && !down && !left && !right)
                return DefaultFileIndex.SingleWall;

            if (up && down && left && right)
                return DefaultFileIndex.Walls_ULR;
            if (up && down && left)
                return DefaultFileIndex.Walls_ULR;
            if (up && down && right)
                return DefaultFileIndex.Walls_ULR;
            if (up && left && right)
                return DefaultFileIndex.Walls_ULR;
            if (down && left && right)
                return DefaultFileIndex.Walls_DLR;
            if (up && down)
                return DefaultFileIndex.Walls_ULR;
            if (up && left)
                return DefaultFileIndex.Walls_UL;
            if (up && right)
                return DefaultFileIndex.Walls_UR;
            if (down && left)
                return DefaultFileIndex.Walls_DL;
            if (down && right)
                return DefaultFileIndex.Walls_DR;
            if (down)
                return DefaultFileIndex.Walls_DLR;


            return SectionTheme.DefaultFileIndex.Walls_ULR;
        }
    }
}