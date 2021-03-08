using RogueSharp.Algorithms;
using System;
using System.Collections;
using System.Collections.Generic;


namespace Lofi.Maps
{
    public class Map
    {
        private static int MapIndex = 1; 
        private static int MapIDMultiplier = 100000; 

        public int MapID { get; }
        public int SectionWidth { get; }
        public int SectionHeight { get; }
        public int SectionCount { get { return (sections != null) ? sections.Count : 0; } }

        private Dictionary<int, Section> sections;
        public Dictionary<int, List<SectionConnection>> sectionConnections;
        public Dictionary<int, Region> regions;
        public List<RegionConnection> regionConnections;
        public EdgeWeightedDigraph sectionGraph;
        public EdgeWeightedDigraph regionGraph;

        public Map (int width, int height)
        {
            MapID = MapIndex++;
            SectionWidth = width;
            SectionHeight = height;
            sections = new Dictionary<int, Section>();
            sectionConnections = new Dictionary<int, List<SectionConnection>>();
            regions = new Dictionary<int, Region>();
            regionConnections = new List<RegionConnection>();

            InitializeSections();
        }

        public void InitializeSections()
        {
            sections.Clear();

            for (int width, height = 0; height < SectionHeight; height++ )
            {
                for (width = 0; width < SectionWidth; width++)
                {
                    Section section = new Section(width,height, GetSectionID(width,height));
                    sections.Add(GetSectionIndex(width, height),section);
                }
            }
        }

        public int ClampX(int x)
        {
            if (x < 0)
                return 0;

            if (x >= SectionWidth)
                return SectionWidth - 1;

            return x;
        }

        public int ClampY(int y)
        {
            if (y < 0)
                return 0;

            if (y >= SectionWidth)
                return SectionWidth - 1;

            return y;
        }

        public void CreateRegionGraph()
        {
            List<(int,int)> edges = new List<(int, int)>();
            int verts = 0;

            foreach(var border in regionConnections)
            {
                edges.Add((border.From.ID, border.To.ID));
                verts = Math.Max(verts, border.From.ID);
                verts = Math.Max(verts, border.To.ID);
            }

            regionGraph = new EdgeWeightedDigraph(verts+1);

            foreach(var edge in edges)
            {
                regionGraph.AddEdge(new DirectedEdge(edge.Item1, edge.Item2, 1.0));
            }
        }
        
        public void CreateSectionGraph()
        {
            List<(int,int)> edges = new List<(int, int)>();

            foreach(var section in sectionConnections)
            {
                foreach (var connection in section.Value)
                {
                    edges.Add((GetSectionIndex(connection.From.OriginX, connection.From.OriginY),
                        GetSectionIndex(connection.To.OriginX, connection.To.OriginY)));
                }
            }

            sectionGraph = new EdgeWeightedDigraph(sections.Count+1);

            foreach(var edge in edges)
            {
                sectionGraph.AddEdge(new DirectedEdge(edge.Item1, edge.Item2, 1.0));
            }
        }

        public bool BorderConnectionExists(Region from, Region to, bool eitherDirection = false)
        {
            foreach(var border in regionConnections)
            {
                if (border.From.ID == from.ID &&
                    border.To.ID == to.ID)
                    return true;
                if (eitherDirection && border.From.ID == to.ID &&
                    border.To.ID == from.ID)
                    return true;
            }
            return false;
        }

        public Dictionary<int, Region> FilterRegionsBySize(int minSize, int maxSize, Dictionary<int, Region> startingFilterList = null)
        {
            Dictionary<int, Region> results = new Dictionary<int, Region>(); 

            if (startingFilterList == null)
            {
                startingFilterList = regions;
            }

            foreach(var region in startingFilterList)
            {
                if(region.Value.sections.Count >= minSize &&
                    region.Value.sections.Count <= maxSize)
                {
                    results.Add(region.Key,region.Value);
                }
            }

            return results;
        }

        public Dictionary<int, Region> FilterRegionsByConnections(int minConnections, int maxConnections, Dictionary<int, Region> startingFilterList = null)
        {
            Dictionary<int, Region> results = new Dictionary<int, Region>();

            if (startingFilterList == null)
            {
                startingFilterList = regions;
            }

            if(regionGraph == null)
            {
                CreateRegionGraph();
            }

            foreach (var region in startingFilterList)
            {
                try
                {
                    if (regionGraph.OutDegree(region.Value.ID) >= minConnections &&
                        regionGraph.OutDegree(region.Value.ID) <= maxConnections)
                    {
                        results.Add(region.Key, region.Value);
                    }
                }
                catch(Exception e)
                { }
            }

            return results;
        }

        public int GetSectionIndex(int x, int y)
        {
            return y * SectionWidth + x;
        }
        
        public int GetSectionID(int x, int y)
        {
            return (MapID * MapIDMultiplier) + GetSectionIndex(x, y);
        }

        public bool isValidSectionIndex(int x, int y)
        {
            if (x < 0 || x >= SectionWidth)
                return false;
            if (y < 0 || y >= SectionHeight)
                return false;
            return true;
        }

        public Section GetSection(int x, int y)
        {
            Section section;

            if(isValidSectionIndex(x, y) && sections.TryGetValue(GetSectionIndex(x, y), out section))
            {
                return section;
            }

            return null;
        }

        public SectionConnection AddSectionConnection(Section sectionFrom, Section SectionTo, Direction direction)
        {
            SectionConnection connect;
            List<SectionConnection> connection;
            int key = GetSectionIndex(sectionFrom.OriginX, sectionFrom.OriginY);
                
            if (sectionConnections.TryGetValue(key, out connection))
            {
                connect = new SectionConnection(sectionFrom, SectionTo, direction);

                if (!connection.Contains(connect))
                    connection.Add(connect);
            }
            else
            {
                connection = new List<SectionConnection>();
                connect = new SectionConnection(sectionFrom, SectionTo, direction);
                connection.Add(connect);
                sectionConnections.Add(key, connection); 
            }
            return connect;
        }

        private SectionConnection GetSectionConnection(Section fromSection, Section toSection)
        {
            List<SectionConnection> connection;
            int key = GetSectionIndex(fromSection.OriginX, fromSection.OriginY);

            if (sectionConnections.TryGetValue(key, out connection))
            {
                foreach(var con in connection)
                {
                    if (con.From == fromSection && con.To == toSection)
                    {
                        return con;
                    }
                }
            }
            return null;
        }

        public void AddBorderConnection(Region fromRegion, Region toRegion, Section fromSection, Section toSection, Direction direction)
        {
            SectionConnection connect = GetSectionConnection(fromSection, toSection);
            RegionConnection border = new RegionConnection(fromRegion, toRegion, connect);
            regionConnections.Add(border);
        }


        public void AddRegion(Section section)
        {
            Region region;
            int key = section.RegionID;

            if (regions.TryGetValue(key, out region))
            {
                region.AddSection(section);
            }
            else
            {
                region = new Region(key);
                region.AddSection(section);
                regions.Add(key, region);
            }
        }

        internal void AddRegionConnection(Section section, Section neighborSection)
        {
            Region region;
            int key = section.RegionID;

            Region neighborRegion;
            int neighborKey = neighborSection.RegionID;


            if (regions.TryGetValue(key, out region))
            {
                if (regions.TryGetValue(neighborKey, out neighborRegion))
                {
                    if (!region.connectedRegions.Contains(neighborRegion))
                    {
                        region.connectedRegions.Add(neighborRegion);
                    }
                }
                else
                {
                    neighborRegion = new Region(neighborKey);
                    regions.Add(neighborKey, neighborRegion);
                    region.connectedRegions.Add(neighborRegion);
                }
            }

        }

        public void UpdateRegionBoundingBoxes()
        {
            foreach(var region in regions)
            {
                region.Value.UpdateBox();
            }
        }
    }
}