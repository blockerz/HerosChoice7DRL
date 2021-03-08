using System;
using System.Collections;
using System.Collections.Generic;


namespace Lofi.Maps
{
    public class Section
    {
        public static byte[] TileIDs =
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

        public byte TileID { get; set; }

        //public byte SectionID{ 
        //    get => SectionID;
        //    set {
        //        if (ValidateSectionID(value))
        //        {
        //            SectionID = value;
        //        }

            //        SectionID = 0;
            //    }
            //}

        public int OriginX{ get; set; }
        public int OriginY{ get; set; }
        public int SectionID{ get; set; }
        public int RegionID{ get; set; }


        public Section(int originX, int originY,int sectionID, byte tileID = 0)
        {
            OriginX = originX;
            OriginY = originY;
            SectionID = sectionID;
            TileID = tileID;
        }

        public static bool ValidateSectionID(byte tileID)
        {
            foreach (byte b in TileIDs)
            { 
                if (b == tileID)
                    return true;
            }
            return false;
        }

        public override bool Equals(Object obj)
        {
            //Check for null and compare run-time types.
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                Section s = (Section)obj;
                return (OriginX == s.OriginX) && (OriginY == s.OriginY) && (RegionID == s.RegionID);
            }
        }

    }

}