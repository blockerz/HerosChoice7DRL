using Lofi.Maps;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class DungeonTheme : SectionTheme
    {
        public enum DungeonDefaultFileIndex
        {
            SingleWall = 0,
            Wall_U = 1,
            Wall_R = 2,
            Wall_D = 3,
            Wall_L = 4,
            Stair = 5,
            Chasm = 6,
            Emblem = 9,


        }

        public DungeonTheme() : base()
        {
            ThemeName = "Dungeon";
            DefaultTileFile = "DungeonDefault";
            doorHeight = 1;
            doorWidth = 1;
        }

        public override Sprite GetBackgroundSprite()
        {
            return Resources.Load<Sprite>(SpriteDirectory + "/" + ThemeName + "/" + "DungeonTemplate1");
        }

        public override Color GetBackgroundSpriteColor(int dungeonNumber)
        {
            switch(dungeonNumber)
            {
                case 0:
                    {
                        return new Color(162 / 255f, 186 / 255f, 255 / 255f);
                    }
                case 1:
                    {
                        return new Color(56 / 255f, 105 / 255f, 0 / 255f);
                    }
                case 2:
                    {
                        return new Color(138 / 255f, 138 / 255f, 0 / 255f);
                    }
                case 3:
                    {
                        return new Color(195 / 255f, 113 / 255f, 0 / 255f);
                    }
                case 4:
                    {
                        return new Color(162 / 255f, 113 / 255f, 255 / 255f);
                    }
                case 5:
                    {
                        return new Color(178 / 255f, 178 / 255f, 178 / 255f);
                    }
            }
            return new Color(162/255f, 186/255f, 255/255f);
        }

        public override void FillSectionTiles(GameMapSection section, int dungeonNumber)
        {

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
                            if ((x == 0 || x == section.Width - 1 || y == 0 || y == section.Height - 1)
                                //|| (x == 1 || x == section.Width - 2 || y == 1 || y == section.Height - 2)
                                //|| (x == 2 && y == 2)
                                //|| (x == 2 && y == section.Height - 3)
                                //|| (x == section.Width - 3 && y == 2)
                                //|| (x == section.Width - 3 && y == section.Height - 3)
                                )
                            {
                                InstantiateTile(section, x, y);
                            }

                        }

                    }
                }
            }

            CreateGroveTilesIfOpen(section, MapFactory.RandomGenerator.Next(2, 8), MapFactory.RandomGenerator.Next(2, 5));

            for (int y = 0; y < section.Height; y++)
            {
                for (int x = 0; x < section.Width; x++)
                {
                    GameObject tile = section.GetTile(x, y);
                    if (tile != null)
                    {
                        if (x == 0 && y == 5)
                        {
                            tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(DungeonDefaultFileIndex.Wall_L);
                            tile.GetComponent<SpriteRenderer>().color = GetBackgroundSpriteColor(dungeonNumber);
                        }
                        else if (x == 16 && y == 5)
                        {
                            tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(DungeonDefaultFileIndex.Wall_R);
                            tile.GetComponent<SpriteRenderer>().color = GetBackgroundSpriteColor(dungeonNumber);
                        }
                        else if (x == 8 && y == 0)
                        {
                            tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(DungeonDefaultFileIndex.Wall_D);
                            tile.GetComponent<SpriteRenderer>().color = GetBackgroundSpriteColor(dungeonNumber);
                        }
                        else if (x == 8 && y == 10)
                        {
                            tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(DungeonDefaultFileIndex.Wall_U);
                            tile.GetComponent<SpriteRenderer>().color = GetBackgroundSpriteColor(dungeonNumber);
                        }
                        else if (x !=0 && y != 0 && x != section.Width-1 && y != section.Height-1)
                        {
                            tile.GetComponent<SpriteRenderer>().sprite = GetDefaultTileSpriteForIndex(DungeonDefaultFileIndex.SingleWall);
                            tile.GetComponent<SpriteRenderer>().color = GetBackgroundSpriteColor(dungeonNumber);
                        }
                    }
                }
            }
        }

        //private void CreateGroveTilesIfOpen(GameMapSection section, int width = 2, int height = 2)
        //{
        //    if (width <= 1 || height <= 1)
        //        return;

        //    int xpos = MapFactory.RandomGenerator.Next(1, section.Width - 2 - (width));
        //    int ypos = MapFactory.RandomGenerator.Next(1, section.Height - 2 - (height));

        //    for (int y = ypos; y < ypos + height * 2; y += 2)
        //    {
        //        for (int x = xpos; x < xpos + width * 2; x += 2)
        //        {
        //            CreateSingleTileIfOpen(section, x, y);
        //        }
        //    }

        //}

        public virtual Sprite GetDefaultTileSpriteForIndex(DungeonDefaultFileIndex index)
        {
            //Debug.Log(SpriteDirectory + "/" + ThemeName + "/" + DefaultTileFile + "_" + (int)index);
            return defaultSprites[(int)index];
        }
    }
}