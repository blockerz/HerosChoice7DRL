using System.Collections;
using System.Collections.Generic;

namespace Lofi.Maps
{
    public class Atlas
    {
        private Dictionary<string, Map> maps;

        public Atlas()
        {
            maps = new Dictionary<string, Map>();
        }

        public bool CreateMap(string name, int width, int height)
        {
            Map map = MapFactory.GenerateMap(width, height, 3, 2);
            maps.Add(name, map);

            return true;
        }
    }
}