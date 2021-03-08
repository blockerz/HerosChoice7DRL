using RogueSharp.Random;
using RogueSharp.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Maps
{
    public class MapFactory
    {
        public static IRandom RandomGenerator { get; set; }

        static MapFactory()
        {
            RandomGenerator = new DotNetRandom();
        }

        //public static System.Random Rand = new System.Random();

        public static List<Vector2Int> neighbors = new List<Vector2Int>
        {
                new Vector2Int(0, 1),
                new Vector2Int(1,0),
                new Vector2Int(0,-1),
                new Vector2Int(-1,0)
        };

        public static Direction GetDirectionFromNeighbor(Vector2Int neighbor)
        {
            if (neighbor.Equals(neighbors[0]))
                return Direction.North;
            if (neighbor.Equals(neighbors[1]))
                return Direction.East;
            if (neighbor.Equals(neighbors[2]))
                return Direction.South;
            if (neighbor.Equals(neighbors[3]))
                return Direction.West;
            return Direction.Center;
        }

        public static Direction GetOppositeDirection(Direction direction)
        {
            if (direction.Equals(Direction.North))
                return Direction.South;
            if (direction.Equals(Direction.East))
                return Direction.West;
            if (direction.Equals(Direction.South))
                return Direction.North;
            if (direction.Equals(Direction.West))
                return Direction.East;
            return Direction.Center;
        }

        //public static Map GenerateOverWorld(int width, int height)
        //{
        //    Map map = new Map(width, height);
        //    CreateDistributedRectRegions(map, 3, 2);
        //    FillGaps(map);
        //    RemoveSingleSections(map);
        //    RelabelDistinctRegions(map);
        //    CreateRegions(map);
        //    ApplyWangTilingToRegions(map);
        //    FillEmptyWangTiles(map);
        //    ConnectRegions(map);
        //    CreateSectionConnections(map);
        //    return map;
        //}

        public static Map GenerateMap(int width, int height, int regionsWide, int regionsTall, bool corridorsOnly = false)
        {
            Map map = null;
            bool qualityCheckPassed = false;
            int attempts = 0;
            int maxAttempts = 100; 

            do
            {
                try
                {
                    attempts++;
                    map = new Map(width, height);
                    CreateDistributedRectRegions(map, regionsWide, regionsTall);
                    //CreateSingleRectRegions(map);
                    FillGaps(map);
                    RemoveSingleSections(map);
                    RelabelDistinctRegions(map);
                    CreateRegions(map);
                    ApplyWangTilingToRegions(map, corridorsOnly);
                    //ApplyWangTilingToDungeonMap(map);
                    if(!corridorsOnly)
                        FillEmptyWangTiles(map);
                    map.UpdateRegionBoundingBoxes();
                    CreateRegionConnections(map);
                    CreateBorderConnections(map);
                    CreateSectionConnections(map);
                    qualityCheckPassed = MapQualityChecks(map);
                    map.CreateRegionGraph();
                    Debug.Log(map.regionGraph);
                    //Debug.Log(new DijkstraShortestPath(map.regionGraph, 1).DistanceTo(7));
                    map.CreateSectionGraph();
                    Debug.Log(map.sectionGraph);
                    //Debug.Log(new DijkstraShortestPath(map.sectionGraph, 0).DistanceTo(191));

                }
                catch (Exception e)
                {
                    Debug.LogError("Map Generation Error");
                    Debug.LogException(e);
                    qualityCheckPassed = false;
                }

            } while (!qualityCheckPassed && attempts <= maxAttempts);

            return map;
        }

        private static bool MapQualityChecks(Map map)
        {
            bool passed = false;

            passed = ConfirmRegionConnections(map);

            return passed;
        }

        private static bool ConfirmRegionConnections(Map map)
        {
            bool connectionFound;

            foreach(var region in map.regions)
            {
                connectionFound = false;

                foreach (var connect in map.regionConnections)
                {
                    if (connect.From.ID == region.Key || connect.To.ID == region.Key)
                    {
                        connectionFound = true;
                        break;
                    }
                }

                if (!connectionFound)
                {
                    Debug.Log("No Border Connection Found for Region ID: " + region.Key);
                    return false;
                }
            }

            return true;
        }

        private static void CreateBorderConnections(Map map)
        {
            foreach (var r in map.regions)
            {
                Region region = r.Value;

                foreach (var neighbor in region.connectedRegions)
                {
                    if (!map.BorderConnectionExists(region, neighbor, true))
                    {
                        List<SectionConnection> potentialBorders = FindPotentialRegionBorders(map, region, neighbor);

                        if (potentialBorders != null && potentialBorders.Count > 0)
                        {
                            int select = RandomGenerator.Next(0, potentialBorders.Count-1);
                            SectionConnection border = potentialBorders[select];

                            var result = WangTileGenerator.MergeEdges(border.Direct, border.From.TileID, border.To.TileID);

                            border.From.TileID = result.Item1;
                            border.To.TileID = result.Item2;

                            map.AddSectionConnection(border.From, border.To, border.Direct);
                            map.AddSectionConnection(border.To, border.From, GetOppositeDirection(border.Direct));
                            map.AddBorderConnection(region, neighbor, border.From, border.To, border.Direct);
                            map.AddBorderConnection(neighbor, region, border.To, border.From, GetOppositeDirection(border.Direct));
                            //break;
                        }
                    }
                }
            }
        }

        private static List<SectionConnection> FindPotentialRegionBorders(Map map, Region region, Region neighbor)
        {
            List<SectionConnection> potentialConnections = new List<SectionConnection>();

            for (int y = (int)Mathf.Round(region.BBox.y); y < (int)Mathf.Round(region.BBox.y + region.BBox.height); y++)
            {
                for (int x = (int)Mathf.Round(region.BBox.x); x < (int)Mathf.Round(region.BBox.x + region.BBox.width); x++)
                {
                    Section regionSection = map.GetSection(x, y);

                    if (regionSection == null || regionSection.RegionID != region.ID)
                        continue;

                    foreach (var p in neighbors)
                    {
                        Section potentialBorder = map.GetSection(p.x + x, p.y + y);

                        if (potentialBorder == null)
                            continue;

                        if(regionSection.RegionID != potentialBorder.RegionID && potentialBorder.RegionID == neighbor.ID)
                        {
                            if (regionSection.TileID != 0 && potentialBorder.TileID != 0)
                                potentialConnections.Add(new SectionConnection(regionSection, potentialBorder, GetDirectionFromNeighbor(p)));
                        }
                    }
                }
            }
            return potentialConnections;
        }

        public static void ApplyWangTilingToDungeonMap(Map map)
        {
            WangTileGenerator generator = new WangTileGenerator(WangTileGenerator.corridorTiles.ToArray());
            WangTileMap wangTileMap = generator.CreateMapWithMinimalEmptySpace(
                                            map.SectionWidth, map.SectionHeight);

            for (int y = 0; y < wangTileMap.Height; y++)
            {
                for (int x = 0; x < wangTileMap.Width; x++)
                {
                    Section section = map.GetSection(x, y);
                    section.TileID = wangTileMap.tileMap[x, y].ID;
                }
            }
        }

        public static void ApplyWangTilingToRegions(Map map, bool corridorsOnly = false)
        {

            foreach (var r in map.regions)
            {
                Region region = r.Value;

                WangTileMap inputMap = CreateRegionInputMap(map, region);


                WangTileGenerator generator;

                if (corridorsOnly)
                    generator = new WangTileGenerator(WangTileGenerator.corridorTiles.ToArray());
                else
                    generator = new WangTileGenerator();

                WangTileMap wangTileMap = generator.CreateMapWithMinimalEmptySpace(
                        (int)Mathf.Round(region.BBox.width), (int)Mathf.Round(region.BBox.height),
                        inputMap);

                if (wangTileMap == null)
                    continue;

                int offsetX = (int)Mathf.Round(region.BBox.x);
                int offsetY = (int)Mathf.Round(region.BBox.y);

                for (int y = 0; y < wangTileMap.Height; y++)
                {
                    for (int x = 0; x < wangTileMap.Width; x++)
                    {
                        Section section = map.GetSection(offsetX + x, offsetY + y);
                        if (section != null && section.RegionID == region.ID)
                        {
                            section.TileID = wangTileMap.tileMap[x, y].ID;
                        }
                    }
                }
            }
        }

        public static void FillEmptyWangTiles(Map map)
        {
            Section section;

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    section = map.GetSection(w, h);
                    if (section.TileID == 0)
                    {
                        foreach(var neighbor in neighbors)
                        {
                            Section neighborSection = map.GetSection(w+neighbor.x, h+neighbor.y);

                            if (neighborSection != null && neighborSection.TileID != 0)
                            {
                                var result = WangTileGenerator.MergeEdges(GetDirectionFromNeighbor(neighbor), section.TileID, neighborSection.TileID);
                                section.TileID = result.Item1;
                                neighborSection.TileID = result.Item2;

                                if (section.RegionID != neighborSection.RegionID)
                                {
                                    var region = map.regions[section.RegionID];
                                    if (region != null)
                                        region.sections.Remove(section);

                                    section.RegionID = neighborSection.RegionID;
                                    map.AddRegion(section);
                                }
                                break;
                            }
                        }
                        
                    }
                }
            }
        }

        private static WangTileMap CreateRegionInputMap(Map map, Region region)
        {
            WangTileMap regionMap = new WangTileMap((int)Mathf.Round(region.BBox.width),
                            (int)Mathf.Round(region.BBox.height));

            int offsetX = (int)Mathf.Round(region.BBox.x);
            int offsetY = (int)Mathf.Round(region.BBox.y);

            for (int y = 0; y < regionMap.Height; y++)
            {
                for (int x = 0; x < regionMap.Width; x++)
                {
                    if (map.GetSection(offsetX + x, offsetY + y).RegionID != region.ID)
                    {
                        WangTile zeroTile = new WangTile(0);
                        zeroTile.x = x;
                        zeroTile.y = y;
                        regionMap.EmptyTiles++;
                        regionMap.tileMap[x, y] = zeroTile;
                    }
                }
            }
            return regionMap;
        }

        public static void CreateRandomRectRegion(Map map, int rects, int size, int regionID)
        {
            int attempts = 0;
            int radius = size / 4;
            Section startSection;

            startSection = SetRandomStartSection(map, regionID);

            if (startSection == null)
                return;

            CreateRandomRects(map, startSection, rects, size, regionID);
        }

        public static void CreateRandomRects(Map map, Section startSection, int rects, int size, int regionID)
        {
            int attempts = 0;
            int radius = size / 2;

            if (startSection == null)
                return;

            do
            {
                int randomX = RandomGenerator.Next(startSection.OriginX - radius, startSection.OriginX + radius-1);
                int randomY = RandomGenerator.Next(startSection.OriginY - radius, startSection.OriginY + radius-1);
                int randomWidth = RandomGenerator.Next(1, size-1);
                int randomHeight = RandomGenerator.Next(1, size-1);

                for (int x, y = randomY; y < randomY + randomHeight; y++)
                {
                    for (x = randomX; x < randomX + randomWidth; x++)
                    {
                        Section section = map.GetSection(x, y);

                        if (section != null)
                            section.RegionID = regionID;
                    }
                }
                attempts++;
            } while (attempts <= rects);
        }

        public static Section SetRandomStartSection(Map map, int regionID)
        {
            int attempts = 0;
            int randomX, randomY;
            Section section;

            do
            {
                randomX = RandomGenerator.Next(0, map.SectionWidth-1);
                randomY = RandomGenerator.Next(0, map.SectionHeight-1);

                section = map.GetSection(randomX, randomY);
                if (section != null && section.RegionID == 0)
                {
                    section.RegionID = regionID;
                }
                attempts++;
            } while ((section == null || section.RegionID == 0) && attempts < map.SectionCount);

            return section;
        }

        public static void CreateRandomBlobRegion(Map map, int size, int regionID)
        {

            int randomX, randomY;
            Section startSection;

            startSection = SetRandomStartSection(map, regionID);

            if (startSection == null)
                return;

            CreateRandomBlob(map, startSection, size, regionID);
        }

        public static void CreateRandomBlob(Map map, Section startSection, int size, int regionID)
        {

            int growth = 0;
            int radius = size / 2;
            int attempts = 0;
            int randomX, randomY;

            if (startSection == null)
                return;

            growth++;
            attempts = 0;

            do
            {
                randomX = RandomGenerator.Next(startSection.OriginX - radius, startSection.OriginX + radius-1);
                randomY = RandomGenerator.Next(startSection.OriginY - radius, startSection.OriginY + radius-1);

                Section newSection = map.GetSection(randomX, randomY);
                if (newSection != null && newSection.RegionID == 0)
                {
                    newSection.RegionID = regionID;
                    growth++;
                }
                attempts++;
            }
            while (growth <= size && attempts < size * size);
        }

        public static void CreateRectRegions(Map map, int regionsWide, int regionsTall)
        {
            int regionID = 1;
            int regionWidth = map.SectionWidth / regionsWide;
            int regionHeight = map.SectionHeight / regionsTall;

            for (int w, h = 0; h < regionsTall; h++)
            {
                for (w = 0; w < regionsWide; w++)
                {
                    for (int x, y = 0; y < regionHeight; y++)
                    {
                        for (x = 0; x < regionWidth; x++)
                        {
                            Section section = map.GetSection((regionWidth * w) + x, (regionHeight * h) + y);

                            if(section != null)
                                section.RegionID = regionID;
                        }
                    }
                    regionID++;
                }
            }
        }

        public static void CreateDistributedBlobRegions(Map map, int regionsWide, int regionsTall)
        {
            int regionID = 1;
            int regionWidth = map.SectionWidth / regionsWide;
            int regionHeight = map.SectionHeight / regionsTall;

            for (int w, h = 0; h < regionsTall; h++)
            {
                for (w = 0; w < regionsWide; w++)
                {

                    Section startSection = map.GetSection((regionWidth * w) + (regionWidth / 2) , (regionHeight * h) + (regionHeight / 2));

                    if (startSection != null)
                    {
                        startSection.RegionID = regionID;
                        CreateRandomBlob(map, startSection, regionWidth, regionID);
                    }
                    regionID++;
                }
            }
        }


        public static void CreateSingleRectRegions(Map map)
        {
            int regionID = 1;
            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {

                    Section section = map.GetSection(w, h);
                    section.RegionID = regionID;
                }
            }
        }

        public static void CreateDistributedRectRegions(Map map, int regionsWide, int regionsTall)
        {
            int regionID = 1;
            int regionWidth = map.SectionWidth / regionsWide;
            int regionHeight = map.SectionHeight / regionsTall;
            int rects = 4;

            for (int w, h = 0; h < regionsTall; h++)
            {
                for (w = 0; w < regionsWide; w++)
                {

                    Section startSection = map.GetSection((regionWidth * w) + (regionWidth / 2) , (regionHeight * h) + (regionHeight / 2));

                    if (startSection != null)
                    {
                        startSection.RegionID = regionID;
                        CreateRandomRects(map, startSection, rects, regionWidth, regionID);
                    }
                    regionID++;
                }
            }
        }

        public static void FillGaps(Map map)
        {
            Section section;
            int filled;

            do
            {
                filled = 0;
                for (int w, h = 0; h < map.SectionHeight; h++)
                {
                    for (w = 0; w < map.SectionWidth; w++)
                    {
                        section = map.GetSection(w, h);
                        if (section.RegionID == 0)
                        {
                            section.RegionID = GetFillerRegionForSection(map, section);
                            filled++;
                        }
                    }
                }
            } while (filled > 0);
        }

        public static void RemoveSingleSections(Map map)
        {
            Section section;
            List<int> regions = new List<int>();

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    section = map.GetSection(w, h);
                    regions.Clear();

                    foreach (var neighbor in neighbors)
                    {
                        var neighborSection = map.GetSection(section.OriginX + neighbor.x,
                                                            section.OriginY + neighbor.y);

                        if (neighborSection != null && neighborSection.RegionID == section.RegionID)
                        {
                            regions.Add(neighborSection.RegionID);
                        }
                    }

                    if (regions.Count == 0)
                    {
                        section.RegionID = GetFillerRegionForSection(map, section);
                    }
                }
            }
        }

        private static int GetFillerRegionForSection(Map map, Section section)
        {
            List<int> regions = new List<int>(); 

            foreach (var neighbor in neighbors)
            {
                var neighborSection = map.GetSection(section.OriginX + neighbor.x,
                                                    section.OriginY + neighbor.y);

                if(neighborSection != null && neighborSection.RegionID != 0)
                {
                    regions.Add(neighborSection.RegionID);
                }
            }

            if (regions.Count == 0)
                return 0;

            return regions[RandomGenerator.Next(0, regions.Count-1)];
        }

        public static void CreateRegions(Map map)
        {
            Section section; 

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    section = map.GetSection(w, h);
                    map.AddRegion(section);

                    //foreach (var neighbor in neighbors)
                    //{
                    //    var neighborSection = map.GetSection(section.OriginX + neighbor.x,
                    //                                        section.OriginY + neighbor.y);

                    //    if (neighborSection != null) 
                    //    {
                    //        if (neighborSection.RegionID != section.RegionID)
                    //        {
                    //            map.AddRegionConnection(section, neighborSection);
                    //        }
                    //    }
                    //}
                }
            }
        }        
        
        public static void CreateRegionConnections(Map map)
        {
            Section section; 

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    section = map.GetSection(w, h);
                    //map.AddRegion(section);

                    foreach (var neighbor in neighbors)
                    {
                        var neighborSection = map.GetSection(section.OriginX + neighbor.x,
                                                            section.OriginY + neighbor.y);

                        if (neighborSection != null) 
                        {
                            if (neighborSection.RegionID != section.RegionID)
                            {
                                map.AddRegionConnection(section, neighborSection);
                            }
                        }
                    }
                }
            }
        }

        public static void CreateSectionConnections(Map map)
        {
            Section section;

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    section = map.GetSection(w, h);
                    //map.AddRegion(section);

                    foreach (var neighbor in neighbors)
                    {
                        var neighborSection = map.GetSection(section.OriginX + neighbor.x,
                                                            section.OriginY + neighbor.y);

                        if (neighborSection != null && neighborSection.TileID != 0)
                        {
                            Direction direction = GetDirectionFromNeighbor(neighbor);
                            if (WangTileGenerator.EdgeIsOpen(section.TileID, direction) && 
                                WangTileGenerator.EdgesMatch(direction, section.TileID, neighborSection.TileID))
                            {
                                map.AddSectionConnection(section, neighborSection, direction);
                                //map.AddSectionConnection(neighborSection, section, GetOppositeDirectionFromNeighbor(neighbor));
                            }
                            //else
                            //{
                            //    map.AddRegionConnection(section, neighborSection);
                            //}
                        }
                    }
                }
            }
        }

        public static void RelabelDistinctRegions(Map map)
        {
            int[,] regions = new int[map.SectionWidth, map.SectionHeight];
            int ID = 1;
            //Section section;

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    if (regions[w, h] == 0)
                    {
                        FloodFill(map, regions, ID, w, h);
                        ID++;
                    }
                }
            }

            for (int w, h = 0; h < map.SectionHeight; h++)
            {
                for (w = 0; w < map.SectionWidth; w++)
                {
                    map.GetSection(w, h).RegionID = regions[w, h];
                }
            }
        }

        private static void FloodFill(Map map, int[,] regions, int ID, int w, int h)
        {
            if (regions[w, h] != 0)
                return;

            Section section;
            regions[w, h] = ID;

            section = map.GetSection(w, h);

            foreach (var neighbor in neighbors)
            {
                var neighborSection = map.GetSection(section.OriginX + neighbor.x,
                                                    section.OriginY + neighbor.y);

                if (neighborSection != null && neighborSection.RegionID == section.RegionID)
                {
                    //regions[neighborSection.OriginX, neighborSection.OriginY] = ID;
                    FloodFill(map, regions, ID, neighborSection.OriginX, neighborSection.OriginY);
                }
            }

            
        }
    }
}