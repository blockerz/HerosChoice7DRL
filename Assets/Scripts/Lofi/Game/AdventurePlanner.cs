
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
            MapFactory.RandomGenerator = new DotNetRandom(421535328); // single connection region
            Debug.Log("Seed: " + MapFactory.RandomGenerator.Save().Seed[0]);
            overworldMap = MapFactory.GenerateMap(16, 12, 3, 3);
            //overworldMap = MapFactory.GenerateMap(5, 4, 3, 1, true);

            DetermineCriticalPath(overworldMap);

            return overworldMap;
        }

        public static bool DetermineCriticalPath(Map map)
        {
            try
            {
                int preferredMaxSize = 2;
                Dictionary<int, Region> selectedRegions;// = map.FilterRegionsBySize(10, int.MaxValue);
                //Debug.Log("Filter by Size:" + selectedRegions.Count);
                //selectedRegions = map.FilterRegionsByConnections(1, preferredMaxSize, selectedRegions);
                //Debug.Log("Filter by Connection:" + selectedRegions.Count);
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
                selectedRegions.Remove(startRegion.ID);


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

                foreach(var node in path)
                {
                    Debug.Log($"Node: {node.To}");
                }

                String debug = "Start Region: " + startRegion.ID + " End Region: " + endRegion.ID + "\n";
                foreach (var region in selectedRegions)
                {
                    debug += "Region: " + region.Key + ": \n";
                    foreach (var con in region.Value.connectedRegions)
                    {
                        debug += "  Connection: " + con.ID + "\n";
                    }
                }
                Debug.Log(debug);
            }
            catch (Exception e)
            {
                Debug.LogError("Error dermining Critical Path");
                Debug.LogException(e);
                return false;
            }
            return true;
        }
    }
}