using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lofi.Maps
{
    public class RegionConnection
    {
        public Region From { get; set; }
        public Region To { get; set; }
        public SectionConnection Connect { get; set; }

        public RegionConnection(Region from, Region to, SectionConnection connection)
        {
            From = from;
            To = to;
            Connect = connection;
        }
    }
}