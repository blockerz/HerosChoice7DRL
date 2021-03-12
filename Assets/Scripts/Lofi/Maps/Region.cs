
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Maps
{
    public class Region
    {
        public int ID { get; set;}

        public List<Section> sections;
        public List<Region> connectedRegions;
        public Rect BBox;

        public Region(int id)
        {
            ID = id;
            sections = new List<Section>();
            connectedRegions = new List<Region>();
        }

        public Section GetRandomSectionInRegion()
        {
            return sections[MapFactory.RandomGenerator.Next(0, sections.Count - 1)];
        }

        public void UpdateBox()
        {
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = 0, maxY = 0;

            foreach(var section in sections)
            {
                minX = Mathf.Min(minX, section.OriginX);
                minY = Mathf.Min(minY, section.OriginY);
                maxX = Mathf.Max(maxX, section.OriginX);
                maxY = Mathf.Max(maxY, section.OriginY);
            }

            BBox = new Rect(minX, minY, (maxX - minX + 1), (maxY - minY + 1));
        }

        public void UpdateBox(Section section)
        {
            if (BBox.Equals(Rect.zero))
            {
                BBox = new Rect(section.OriginX, section.OriginY, 1, 1);
            }
            else
            {
                float newX, newY, newWidth, newHeight;

                if (section.OriginX < BBox.x)
                {
                    newX = section.OriginX;
                    newWidth = BBox.width + (BBox.x - section.OriginX);
                }
                else if (section.OriginX >= (BBox.x + BBox.width))
                {
                    newX = BBox.x;
                    newWidth = section.OriginX - BBox.x + 1;
                }
                else
                {
                    newX = BBox.x;
                    newWidth = BBox.width;
                }

                if (section.OriginY < BBox.y)
                {
                    newY = section.OriginY;
                    newHeight = BBox.height + (BBox.y - section.OriginY);
                }
                else if (section.OriginY >= (BBox.y + BBox.height))
                {
                    newY = BBox.y;
                    newHeight = section.OriginY - BBox.y + 1;
                }
                else
                {
                    newY = BBox.y;
                    newHeight = BBox.height;
                }

                BBox = new Rect(newX, newY, newWidth, newHeight);
            }
        }


        public void AddSection(Section section)
        {
            sections.Add(section);
            //UpdateBox(section);
            UpdateBox();
        }
    }
}