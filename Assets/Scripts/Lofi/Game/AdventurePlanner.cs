
using Lofi.Maps;
using RogueSharp.Algorithms;
using RogueSharp.Random;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Game
{
    public class AdventurePlanner
    {
        

        public static Map PlanOverworld()
        {
            Map overworldMap;
            //map = MapFactory.GenerateMap(5, 4, 3, 1, true);
            //MapFactory.RandomGenerator = new DotNetRandom(408483593); // used to resolve region border issues
            //MapFactory.RandomGenerator = new DotNetRandom(421535328); // single connection region
            Debug.Log("Seed: " + MapFactory.RandomGenerator.Save().Seed[0]);
            overworldMap = MapFactory.GenerateMap(16, 8, 3, 2);
            //overworldMap = MapFactory.GenerateMap(5, 4, 3, 1, true);

            DetermineCriticalPath(overworldMap);

            DetermineDungeonLocations(overworldMap);

            return overworldMap;
        }

        private static void DetermineDungeonLocations(Map overworldMap)
        {
            int dungeonCount = 5;
            int attempts = 10;

            List<Section> dungeons = new List<Section>();

            Section selected;

            do
            {
                selected = overworldMap.regions[overworldMap.startSection.RegionID].GetRandomSectionInRegion();
            } while (selected.SectionID == overworldMap.startSection.SectionID && attempts-- > 0);

            dungeons.Add(selected);

            foreach(var region in overworldMap.regions)
            {
                if (overworldMap.startSection.RegionID != region.Key
                    && overworldMap.endSection.RegionID != region.Key)
                {
                    selected = region.Value.GetRandomSectionInRegion();
                    dungeons.Add(selected);
                }
            }

            dungeons.Add(overworldMap.endSection);

            GameManager.instance.dungeonSections = dungeons;

        }

        public static bool DetermineCriticalPath(Map map)
        {
            map.regionCriticalPath = new List<int>();

            try
            {
                int preferredMaxSize = 2;
                Dictionary<int, Region> selectedRegions;

                do
                {
                    selectedRegions = map.FilterRegionsBySize(10, int.MaxValue);

                    if (selectedRegions.Count < 2)
                    {
                        selectedRegions = map.FilterRegionsBySize(1, int.MaxValue);
                    }

                    selectedRegions = map.FilterRegionsByConnections(1, preferredMaxSize, selectedRegions);
                    preferredMaxSize++;
                }
                while ((selectedRegions.Count < 2) && preferredMaxSize < 10);

                Region[] regions = new Region[selectedRegions.Values.Count];
                selectedRegions.Values.CopyTo(regions, 0);
                Region startRegion = regions[MapFactory.RandomGenerator.Next(0, selectedRegions.Count - 1)];
                map.regionCriticalPath.Add(startRegion.ID);
                selectedRegions.Remove(startRegion.ID);

                int attempts = 20;
                do
                {
                    map.startSection = startRegion.sections[MapFactory.RandomGenerator.Next(0, startRegion.sections.Count - 1)];
                }
                while (map.startSection.TileID == 0 && attempts-- > 0);

                Region endRegion;
                do
                {

                    regions = new Region[selectedRegions.Values.Count];
                    selectedRegions.Values.CopyTo(regions, 0);
                    endRegion = regions[MapFactory.RandomGenerator.Next(0, selectedRegions.Count - 1)];
                    selectedRegions.Remove(endRegion.ID);

                    if (!startRegion.connectedRegions.Contains(endRegion))
                        break;
                }
                while (selectedRegions.Count > 0);

                IEnumerable<DirectedEdge> path = DijkstraShortestPath.FindPath(map.regionGraph, startRegion.ID, endRegion.ID);

                foreach (var node in path)
                {
                    //Debug.Log($"Node: {node.To}");
                    map.regionCriticalPath.Add(node.To);
                }


                map.pathFromStartSection = new DijkstraShortestPath(map.sectionGraph, map.GetSectionIndex(map.startSection.OriginX, map.startSection.OriginY));

                Section furthestSection = null;
                int furthest = 0;

                foreach(var section in endRegion.sections)
                {
                    int distance = (int)map.pathFromStartSection.DistanceTo(section.SectionID);
                    if (furthest < distance)
                    {
                        furthestSection = section;
                        furthest = distance;
                    }
                }

                map.endSection = furthestSection;
                map.pathFromEndSection = new DijkstraShortestPath(map.sectionGraph, map.GetSectionIndex(map.endSection.OriginX, map.endSection.OriginY));

                furthestSection = null;
                furthest = 0;

                foreach (var section in startRegion.sections)
                {
                    int distance = (int)map.pathFromEndSection.DistanceTo(section.SectionID);
                    if (furthest < distance)
                    {
                        furthestSection = section;
                        furthest = distance;
                    }
                }

                map.startSection = furthestSection;
                map.pathFromStartSection = new DijkstraShortestPath(map.sectionGraph, map.GetSectionIndex(map.startSection.OriginX, map.startSection.OriginY));

                String debug = "Start Region: " + startRegion.ID + " End Region: " + endRegion.ID + "\n";
                debug += "Start Section: " + map.startSection.SectionID + " End Section: " + map.endSection.SectionID + "\n";

                //foreach (var region in selectedRegions)
                //{
                //    debug += "Region: " + region.Key + ": \n";
                //    foreach (var con in region.Value.connectedRegions)
                //    {
                //        debug += "  Connection: " + con.ID + "\n";
                //    }
                //}
                Debug.Log(debug);
            }
            catch (Exception e)
            {
                Debug.LogError("Error dermining Critical Path: " + map.endSection.SectionID + ": " + map.endSection.OriginX + ", " + map.endSection.OriginY + " : " + map.GetSectionIndex(map.startSection.OriginX, map.startSection.OriginY));
                Debug.LogException(e);
                return false;
            }
            return true;
        }

        internal static Map PlanDungeon()
        {
            Map dungeon;
            //map = MapFactory.GenerateMap(5, 4, 3, 1, true);
            //MapFactory.RandomGenerator = new DotNetRandom(408483593); // used to resolve region border issues
            //MapFactory.RandomGenerator = new DotNetRandom(421535328); // single connection region
            Debug.Log("Seed: " + MapFactory.RandomGenerator.Save().Seed[0]);
            dungeon = MapFactory.GenerateMap(5, 4, 3, 1, true);
            //dungeon = MapFactory.GenerateMap(MapFactory.RandomGenerator.Next(5, 7), MapFactory.RandomGenerator.Next(4, 6), 3, 1, true);

            DetermineCriticalPath(dungeon);

            return dungeon;
        }
    }
}