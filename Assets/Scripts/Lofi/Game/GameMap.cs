using Lofi.Maps;
using RogueSharp.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class GameMap : MonoBehaviour
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public int TileWidth { get; set; }
        public int TileHeight { get; set; }

        internal GameMapSection[,] mapSections;
        GameObject mapSectionPrefab;

        GameMapThemes themes;
        public Map map;
        internal GameMapSection startSection;

        private void Awake()
        {
            mapSectionPrefab = (GameObject)Resources.Load("prefabs/GameMapSection", typeof(GameObject));
            TileWidth = 17;
            TileHeight = 11;

            themes = new GameMapThemes();
            themes.CreateThemes();

        }



        public void UpdateSectionDifficultiesBasedOnStart()
        {
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    GameMapSection mapSection = mapSections[x, y];
                    mapSection.difficulty = GetSectionDifficulty(mapSection);
                }
            }
        }

        public int GetSectionDifficulty(GameMapSection section)
        {
            try
            {
                return (int) map.pathFromStartSection.DistanceTo(section.Section.SectionID);
                //var path = new DijkstraShortestPath(map.sectionGraph, map.GetSectionIndex(section.Section.OriginX, section.Section.OriginY));

                //if (path == null)
                //    return 1;

                //return (int)path.DistanceTo(map.GetSectionIndex(startSection.Section.OriginX, startSection.Section.OriginY));
            }
            catch(Exception e)
            {
                Debug.LogError("Difficuly calculation failed for:" + section.name);
                return 1;
            }
        }

        public void CreateMap(Map map, bool dungeon = false)
        {
            this.map = map;
            Width = map.SectionWidth;
            Height = map.SectionHeight;
            mapSections = new GameMapSection[Width, Height];



            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    GameMapSection mapSection = mapSections[x, y] = Instantiate(mapSectionPrefab, this.transform).GetComponent<GameMapSection>();
                    mapSection.name = "Section: " + x + ", " + y;
                    mapSection.Section = map.GetSection(x, y);
                    mapSection.transform.position = new Vector3(x * TileWidth, y * TileHeight, 0);
   

                    if(dungeon)
                        mapSection.Initialize(TileWidth, TileHeight, themes.GetThemeForDungeon(mapSection.Section));
                    else
                        mapSection.Initialize(TileWidth, TileHeight, themes.GetThemeForRegion(mapSection.Section));
                    //mapSection.gameObject.SetActive(false);

                    if (mapSection.Section.SectionID == map.startSection.SectionID)
                        mapSection.preventEnemySpawns = true;

                    if (dungeon && mapSection.Section.SectionID == map.endSection.SectionID)
                    {
                        EnemyFactory.GetBossEnemy(mapSection);
                        mapSection.preventEnemySpawns = true;
                    }

                    // Make 0 tiles bllack to distinguish in editor
                    if (mapSection.Section.TileID == 0)
                        mapSection.gameObject.GetComponentInChildren<SpriteRenderer>().color = Color.black;
                }
            }
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}