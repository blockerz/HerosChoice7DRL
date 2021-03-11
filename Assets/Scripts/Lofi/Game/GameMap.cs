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

        GameMapSection[,] mapSections;
        GameObject mapSectionPrefab;
        GameObject playerPrefab;
        GameMapThemes themes;
        Map map;
        GameMapSection startSection;

        private void Awake()
        {
            mapSectionPrefab = (GameObject)Resources.Load("prefabs/GameMapSection", typeof(GameObject));
            playerPrefab = (GameObject)Resources.Load("prefabs/Player", typeof(GameObject));
            TileWidth = 17;
            TileHeight = 11;

            themes = new GameMapThemes();
            themes.CreateThemes();

        }

        internal void SpawnPlayer()
        {
            int startRegion = map.regionCriticalPath[0];
            Region region;
            map.regions.TryGetValue(startRegion, out region);
            if (region != null)
            {
                Section section = region.sections[MapFactory.RandomGenerator.Next(0, region.sections.Count - 1)];

                startSection = mapSections[section.OriginX, section.OriginY];
                startSection.preventEnemySpawns = true;

                Vector3 startPos = startSection.GetRandomOpenTile() + startSection.transform.position;
                GameManager.instance.player = Instantiate(playerPrefab);
                GameManager.instance.player.name = "Player";
                GameManager.instance.player.transform.position = startPos;

                UpdateSectionDifficultiesBasedOnStart();

                //startSection.gameObject.SetActive(true);
            }
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
                var path = new DijkstraShortestPath(map.sectionGraph, map.GetSectionIndex(section.Section.OriginX, section.Section.OriginY));

                if (path == null)
                    return 1;

                return (int)path.DistanceTo(map.GetSectionIndex(startSection.Section.OriginX, startSection.Section.OriginY));
            }
            catch(Exception e)
            {
                Debug.LogError("Difficuly calculation failed for:" + section.name);
                return 1;
            }
        }

        public void CreateMap(Map map)
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
                    mapSection.Section = map.GetSection(x, y);
                    mapSection.Initialize(TileWidth, TileHeight, themes.GetThemeForRegion(mapSection.Section));
                    mapSection.name = "Section: " + x + ", " + y;
                    mapSection.transform.position = new Vector3(x * TileWidth, y * TileHeight, 0);
                    //mapSection.gameObject.SetActive(false);
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