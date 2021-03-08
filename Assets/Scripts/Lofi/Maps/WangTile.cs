using System.Collections.Generic;

namespace Lofi.Maps
{
    public class WangTile
    {
        public byte ID { get; set; }
        public int x { get; set; }
        public int y { get; set; }

        public WangTile()
        {
            
        }

        public WangTile(byte id)
        {
            ID = id;
        }
    }
}